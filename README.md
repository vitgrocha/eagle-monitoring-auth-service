# 🔐 eagle-monitoring-auth-service
[![CI](https://img.shields.io/github/actions/workflow/status/SEU_USUARIO/eagle-monitoring-auth-service/ci.yml?branch=main)]()
[![Release](https://img.shields.io/github/v/release/SEU_USUARIO/eagle-monitoring-auth-service)]()
[![License](https://img.shields.io/badge/license-MIT-blue.svg)]()


Microserviço responsável pela autenticação e gerenciamento de acesso de usuários.  
Utiliza **ASP.NET Core**, **JWT**, **BCrypt** e segue o padrão **RESTful**.  
Desenvolvido para integração com ambientes baseados em **microsserviços**.

---

## 📌 Sumário

- [Visão Geral](#visão-geral)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Como Rodar](#como-rodar)
- [Endpoints](#endpoints)
- [Modelos de Resposta/Erro](#modelos-de-respostaerro)
- [Testes](#testes)
- [Contribuição](#contribuição)
- [Licença](#licença)

---

## Visão Geral
- **Responsabilidade única:** autenticar usuários e emitir **tokens JWT** contendo `roles/claims`.
- **Autorização:** este serviço **não** gerencia políticas/permissões — outros serviços/gateway validam o **role** presente no JWT.
- **Pronto para microsserviços:** comunicação via HTTP, documentação via **Swagger/OpenAPI** e healthcheck.

**Swagger:** `http://localhost:PORT/swagger` (ex.: `http://localhost:5259/swagger`)

---

## Tecnologias
- ASP.NET Core
- Entity Framework Core
- BCrypt (hash de senhas)
- JWT (bearer tokens)
- MySQL ou SQL Server (via connection string)
- Swagger / OpenAPI

---
## 📁 Estrutura do Projeto

```
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
├── Data/
│   └── AuthDbContext.cs
├── Program.cs
├── appsettings.json
└── MSAuthentication.csproj
```

---

## 🔑 Perfis de Acesso (Roles)

Perfis de acesso definidos no enum `UserRole.cs`:

```csharp
public enum UserRole
{
    Admin = 0,
    AdminTerceirizado = 1,
    Portaria = 2,
    Morador = 3,
    Visitante = 4
}
```

Esses perfis podem ser utilizados para controle de acesso via middleware ou policies.

---

## 📮 Endpoints da API

| Método | Rota                          | Descrição                           |
|--------|-------------------------------|--------------------------------------|
| POST   | `/api/auth/register`          | Registro de novo usuário             |
| POST   | `/api/auth/login`             | Login e geração do token JWT         |
| POST   | `/api/auth/logout`            | Logout (revogação do token)          |
| POST   | `/api/auth/change-password`   | Alteração de senha                   |
| POST   | `/api/auth/forgot-password`   | Envio de token para recuperação      |
| POST   | `/api/auth/validate-token`    | Validação do token de recuperação    |
| PUT    | `/api/auth/update-user`       | Atualização de dados do usuário      |

> 📚 **Swagger disponível em:**  
> [`http://localhost:{5259}/swagger`](http://localhost:5259/swagger)

---

## 🛠️ Como Executar Localmente

1. Clone o repositório:

```bash
git clone https://github.com/seu-usuario/MSAuthentication.git
cd MSAuthentication
```

2. Configure o arquivo `appsettings.json` com:
   - Connection string (MySQL ou SQL Server)
   - JWT config (chave secreta, tempo de expiração)

3. (Opcional) Execute as migrações:

```bash
dotnet ef database update
```

4. Rode o projeto:

```bash
dotnet run
```

---

## ⚠️ Observações

- Este serviço **não implementa autorização diretamente** — ele apenas inclui o `role` no JWT. A validação de permissões deve ser feita por outro serviço ou gateway.
- Compatível com **Redis** para controle de tokens revogados.
- Arquitetado para funcionar de forma **independente ou integrado via API Gateway**.

---

## 📄 Licença

Este projeto está licenciado sob a **MIT License**.  
Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
