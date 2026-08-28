# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ./global.json ./
COPY ./Directory.Build.props ./
COPY ./KurumiConcursos.sln ./
COPY ./KurumiConcursos.Api/KurumiConcursos.Api.csproj ./KurumiConcursos.Api/
COPY ./KurumiConcursos.ApplicationService/KurumiConcursos.ApplicationService.csproj ./KurumiConcursos.ApplicationService/
COPY ./KurumiConcursos.Infra/KurumiConcursos.Infra.csproj ./KurumiConcursos.Infra/
COPY ./KurumiConcursos.Domain/KurumiConcursos.Domain.csproj ./KurumiConcursos.Domain/
COPY ./KurumiConcursos.UnitTests/KurumiConcursos.UnitTests.csproj ./KurumiConcursos.UnitTests/
COPY ./KurumiConcursos.Api/packages.lock.json ./KurumiConcursos.Api/
COPY ./KurumiConcursos.ApplicationService/packages.lock.json ./KurumiConcursos.ApplicationService/
COPY ./KurumiConcursos.Infra/packages.lock.json ./KurumiConcursos.Infra/
COPY ./KurumiConcursos.Domain/packages.lock.json ./KurumiConcursos.Domain/
COPY ./KurumiConcursos.UnitTests/packages.lock.json ./KurumiConcursos.UnitTests/

RUN dotnet restore KurumiConcursos.sln --locked-mode

COPY . ./

RUN dotnet test KurumiConcursos.UnitTests/KurumiConcursos.UnitTests.csproj \
    --no-restore --configuration Release
RUN dotnet publish KurumiConcursos.Api/KurumiConcursos.Api.csproj \
    --no-restore --configuration Release --output /app/publish /p:UseAppHost=false
RUN rm -f /app/publish/appsettings.Development.json

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN useradd --create-home appuser
USER appuser

ENTRYPOINT ["dotnet", "KurumiConcursos.Api.dll"]
