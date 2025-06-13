# MsAuthentication

# 🔐 MSAuthentication - Microserviço de Autenticação

Microserviço responsável pela autenticação e gerenciamento de acesso de usuários. Utiliza ASP.NET Core, JWT, BCrypt e segue o padrão RESTful. Desenvolvido para integração com ambientes baseados em microsserviços.

---

## 📌 Visão Geral

O **MSAuthentication** é responsável pelo processo de login, registro, alteração de senha, recuperação de conta e geração de tokens JWT com controle de perfis (roles). Ele atua como ponto central de autenticação no sistema distribuído, desacoplado dos demais serviços.

---

## 🚀 Tecnologias Utilizadas

- ✅ **ASP.NET Core 7.0+**
- ✅ **Entity Framework Core**
- ✅ **JWT Bearer Authentication**
- ✅ **BCrypt.Net** (hash de senhas)
- ✅ **AutoMapper**
- ✅ **Swagger (Swashbuckle)**
- ✅ **MySQL / SQL Server**
- ✅ **CORS Configurado**
- ✅ **Arquitetura RESTful**
---
## 📂 Estrutura do Projeto

MSAuthentication/
├── Controllers/
│   └── AuthController.cs
├── DTOs/
│   ├── RegisterRequest.cs
│   ├── LoginRequest.cs
│   ├── AuthResponse.cs
│   ├── ChangePasswordDto.cs
│   ├── ForgotPasswordDto.cs
│   └── UpdateUserDto.cs
├── Models/
│   └── User.cs
├── Enums/
│   └── UserRole.cs
├── Services/
│   ├── AuthService.cs
│   └── JwtTokenService.cs
├── Helpers/
│   └── PasswordHasher.cs
├── Data/
│   └── AuthDbContext.cs
├── Program.cs
├── appsettings.json
└── MSAuthentication.csproj
🔑 Perfis de Acesso
A autenticação é baseada em perfis (roles) definidos no enum UserRole.cs:

public enum UserRole
{
    Admin = 0,
    AdminTerceirizado = 1,
    Portaria = 2,
    Morador = 3,
    Visitante = 4
}
Esses perfis podem ser utilizados para controle de acesso via middleware ou policies.

📮 Endpoints da API
Método	Rota	Descrição
POST	/api/auth/register	Registro de um novo usuário
POST	/api/auth/login	Login e geração de token JWT
POST	/api/auth/logout	Logout (revogação via cache)
POST	/api/auth/change-password	Alteração de senha
POST	/api/auth/forgot-password	Envio de token de recuperação
POST	/api/auth/validate-token	Validação do token de recuperação
PUT	/api/auth/update-user	Atualização dos dados do usuário

📚 Documentação Swagger disponível em:
http://localhost:{5259}/swagger

🛠️ Como Executar Localmente
Clone o repositório:

git clone https://github.com/seu-usuario/MSAuthentication.git
cd MSAuthentication
Configure o appsettings.json:

Connection string para seu banco (MySQL ou SQL Server)

JWT: chave secreta, tempo de expiração, etc.

(Opcional) Execute as migrações:

dotnet ef database update
Execute o projeto:
dotnet run
---
⚠️ Observações
Esse serviço não é responsável por autorização (roles são incluídas no JWT, mas a lógica de permissões deve ser aplicada por outro serviço ou gateway).

Compatível com Redis para controle de tokens revogados.

Arquitetado para funcionar de forma independente ou integrado a uma arquitetura maior com API Gateway.
---
🤝 Contribuições
Contribuições são bem-vindas!
Sinta-se livre para abrir issues ou enviar pull requests. 😄

📄 Licença
Este projeto está licenciado sob a MIT License.
Consulte o arquivo LICENSE para mais detalhes.



