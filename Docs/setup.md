# ⚙️ Setup do MSAuthentication

Este documento descreve como configurar e executar localmente o microserviço de autenticação.

---

## 📌 Pré-requisitos

- [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
- Banco de dados MySQL ou SQL Server
- Editor de código (Visual Studio 2022 ou VS Code)
- Git instalado

---

## 🚀 Passos para executar localmente

### 1. Clone o repositório

```bash
git clone https://github.com/vitgrocha/MsAuthentication.git
cd MsAuthentication
2. Configure o appsettings.json
Edite o arquivo appsettings.json com sua string de conexão:

Para MySQL:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=msauthdb;User=root;Password=sua_senha;"
}
Para SQL Server:

"ConnectionStrings": {
  "DefaultConnection": "Server=VITDEVSQL;Database=MsAuthenticationDB;User Id=sa;Password=sua_senha;"
}
3. Executar as migrations (EF Core)

dotnet ef database update
Isso criará as tabelas no banco de dados com base no seu modelo de dados.

4. Rodar a aplicação

dotnet run
A API será iniciada por padrão em:

http://localhost:5000


5. Testar via Swagger
Acesse o Swagger UI para testar a API:

http://localhost:5000/swagger


🔐 Autenticação via Token
A autenticação funciona com tokens JWT.
Use o cabeçalho:

Authorization: Bearer {seu_token_aqui}


🧪 Recursos inclusos
Login com email e senha

Registro de usuários

Recuperação e alteração de senha

Middleware de autenticação

Controle de perfis com enum UserRole

👩‍💻 Desenvolvido por: Vitória Gabriella
📁 Projeto: Eagle Monitoring
📆 Versão: 1.0 (Março/2024)