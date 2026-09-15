# Materiais privados do Nextcloud

Os PDFs privados sao mantidos no Nextcloud. O Kurumi salva somente o nome, caminho WebDAV e tipo do arquivo na tabela `topic_material`; o conteudo nao e copiado para o banco nem exposto por link publico.

## Configuracao do servidor

No arquivo `.env` usado pelo `docker compose`, defina:

```dotenv
NEXTCLOUD_URL=https://nextcloud.seu-dominio.com
NEXTCLOUD_USER=kurumi-integration
NEXTCLOUD_APP_PASSWORD=senha-de-aplicativo-gerada-no-nextcloud
NEXTCLOUD_ROOT_PATH=/Kurumi
PRIVATE_MATERIALS_USERS=usuario1@dominio.com,usuario2@dominio.com,usuario3@dominio.com
```

`NEXTCLOUD_USER` deve ser, de preferencia, uma conta tecnica do Nextcloud que tenha somente leitura da pasta configurada. Crie a senha em **Configuracoes pessoais > Seguranca > Senhas de aplicativos** no Nextcloud; nunca use a senha principal no frontend ou em um arquivo versionado.

O container da API precisa conseguir acessar `NEXTCLOUD_URL`. Se o Nextcloud estiver em rede privada, configure DNS/rota entre os containers e o servidor. Use HTTPS com certificado valido; a API rejeita URL HTTP para evitar envio de credenciais sem criptografia.

Em instalacoes sob subdiretorio, informe a URL completa, por exemplo `https://servidor.exemplo.com/nextcloud`. A integracao monta o endpoint WebDAV automaticamente.

Depois de alterar o `.env`, recrie a API com `docker compose up -d --build api` (ou o nome equivalente do servico no seu ambiente). A migration cria `kurumi_concursos.topic_material` na inicializacao normal da aplicacao.

## Permissao

Os e-mails em `PRIVATE_MATERIALS_USERS` recebem a claim `PRIVATE_MATERIALS` na inicializacao. Eles precisam sair e entrar novamente para obter um JWT novo. A API exige essa claim em todos os endpoints de materiais privados; esconder a interface nao e a unica protecao.

Para revogar acesso, remova o e-mail da configuracao e remova a claim `permission=PRIVATE_MATERIALS` do usuario no banco ou pela futura tela administrativa. Tokens ja emitidos continuam validos ate expirar; para revogacao imediata, invalide a sessao/JWT conforme a politica operacional do ambiente.

## Verificacao funcional

1. Entre com um e-mail autorizado e abra um topico.
2. Em **Materiais privados**, use **Vincular material**, navegue na pasta permitida e escolha um PDF.
3. Use **Abrir**: o navegador recebe o stream autenticado da API, sem URL compartilhada do Nextcloud.
4. Entre com um e-mail sem a claim: a secao nao aparece e chamadas aos endpoints retornam `403` (com token valido).
