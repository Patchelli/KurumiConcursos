using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;
using KurumiConcursos.ApplicationService.Services.PrivateMaterialServices;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.Services;

public sealed class NextcloudWebDavService(HttpClient client, NextcloudOptions options) : INextcloudWebDavService
{
    private static readonly XNamespace Dav = "DAV:";

    public async Task<IList<NextcloudEntryResponse>> BrowseAsync(string? path, CancellationToken cancellationToken)
    {
        var normalized = NormalizePath(path);
        using var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), BuildUri(normalized));
        request.Headers.Add("Depth", "1");
        request.Content = new StringContent("""
            <?xml version="1.0" encoding="utf-8" ?>
            <d:propfind xmlns:d="DAV:"><d:prop><d:displayname/><d:resourcetype/><d:getcontenttype/><d:getcontentlength/></d:prop></d:propfind>
            """, Encoding.UTF8, "application/xml");
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden) return [];
        response.EnsureSuccessStatusCode();
        var document = XDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        return document.Descendants(Dav + "response").Select(ParseEntry)
            .Where(item => item is not null && item.Path != normalized)
            .Select(item => item!).Where(item => item.IsDirectory || IsPdf(item.Name, item.MimeType)).OrderByDescending(item => item.IsDirectory)
            .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public async Task<NextcloudFileMetadata?> FindPdfAsync(string path, CancellationToken cancellationToken)
    {
        var normalized = NormalizePath(path);
        if (!normalized.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) return null;
        using var request = new HttpRequestMessage(HttpMethod.Head, BuildUri(normalized));
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var mime = response.Content.Headers.ContentType?.MediaType;
        return IsPdf(normalized, mime) ? new(Path.GetFileName(normalized), normalized, "application/pdf") : null;
    }

    public async Task<PrivateMaterialFile?> OpenPdfAsync(string path, CancellationToken cancellationToken)
    {
        var normalized = NormalizePath(path);
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildUri(normalized));
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            response.Dispose();
            return null;
        }
        response.EnsureSuccessStatusCode();
        if (!IsPdf(normalized, response.Content.Headers.ContentType?.MediaType))
        {
            response.Dispose();
            return null;
        }
        return new PrivateMaterialFile(Path.GetFileName(normalized),
            await response.Content.ReadAsStreamAsync(cancellationToken), response);
    }

    private NextcloudEntryResponse? ParseEntry(XElement response)
    {
        var href = response.Element(Dav + "href")?.Value;
        if (string.IsNullOrWhiteSpace(href)) return null;
        var uri = new Uri(href, UriKind.RelativeOrAbsolute);
        var serverPath = Uri.UnescapeDataString(uri.IsAbsoluteUri ? uri.AbsolutePath : href).TrimEnd('/');
        // A instalação pode estar em um subdiretório (por exemplo, /nextcloud).
        // O href retornado pelo WebDAV inclui esse trecho, enquanto o caminho salvo
        // no banco continua relativo à raiz de arquivos do usuário.
        var basePath = new Uri(options.Url, UriKind.Absolute).AbsolutePath.TrimEnd('/');
        var prefix = $"{basePath}/remote.php/dav/files/{options.User}";
        if (!serverPath.StartsWith(prefix, StringComparison.Ordinal)) return null;
        var path = NormalizePath(serverPath[prefix.Length..]);
        var property = response.Descendants(Dav + "prop").FirstOrDefault();
        var isDirectory = property?.Element(Dav + "resourcetype")?.Element(Dav + "collection") is not null;
        var name = property?.Element(Dav + "displayname")?.Value;
        return new NextcloudEntryResponse(string.IsNullOrWhiteSpace(name) ? Path.GetFileName(path) : name, path,
            isDirectory, property?.Element(Dav + "getcontenttype")?.Value,
            long.TryParse(property?.Element(Dav + "getcontentlength")?.Value, out var size) ? size : null);
    }

    private Uri BuildUri(string path) => new($"{options.Url.TrimEnd('/')}/remote.php/dav/files/{Uri.EscapeDataString(options.User)}{string.Join(string.Empty, path.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(segment => "/" + Uri.EscapeDataString(segment)))}");

    private string NormalizePath(string? path)
    {
        var candidate = Uri.UnescapeDataString(string.IsNullOrWhiteSpace(path) || path == "/" ? options.RootPath : path).Replace('\\', '/').Trim();
        if (!candidate.StartsWith('/')) candidate = "/" + candidate;
        if (candidate.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(segment => segment is "." or ".."))
            throw new InvalidOperationException("Caminho do Nextcloud invalido.");
        var root = options.RootPath.Trim().Trim('/');
        var normalized = "/" + string.Join('/', candidate.Split('/', StringSplitOptions.RemoveEmptyEntries));
        if (!string.IsNullOrEmpty(root) && normalized != "/" + root && !normalized.StartsWith("/" + root + "/", StringComparison.Ordinal))
            throw new InvalidOperationException("Caminho fora da pasta privada configurada.");
        return normalized;
    }

    private static bool IsPdf(string name, string? mime) => name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(mime, "application/pdf", StringComparison.OrdinalIgnoreCase);
}
