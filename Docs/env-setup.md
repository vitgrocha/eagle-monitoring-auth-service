# Configuração das Variáveis de Ambiente

Este documento explica as variáveis de ambiente usadas no projeto e como configurar seu ambiente local.

---

## Arquivo `.env`

Copie o arquivo `.env.example` para `.env` na raiz do projeto e preencha os valores conforme indicado abaixo.

---

### JWT Configuration

- `JWT_KEY`: Chave secreta usada para gerar o token JWT.
- `JWT_ISSUER`: Emissor do token.
- `JWT_AUDIENCE`: Público-alvo do token.
- `JWT_EXPIRATION_MINUTES`: Tempo de expiração do token (em minutos).

### Database Connection String

- `AUTH_DB_CONNECTION`: String de conexão para o banco de dados de autenticação. Ajuste o servidor, usuário e senha conforme seu ambiente local.

### SMTP Settings (Envio de E-mails)

- `SMTP_HOST`: Host do servidor SMTP (exemplo: smtp.gmail.com).
- `SMTP_PORT`: Porta do servidor SMTP (exemplo: 587).
- `SMTP_USERNAME`: Usuário para autenticação SMTP (normalmente o e-mail).
- `SMTP_PASSWORD`: Senha do usuário SMTP (não compartilhe publicamente).
- `SMTP_FROM`: E-mail remetente.
- `SMTP_ENABLE_SSL`: Define se a conexão usa SSL (true ou false).


