# 🔐 MSAuthentication - Microserviço de Autenticação

Microserviço responsável pela autenticação e gerenciamento de acesso de usuários.  
Utiliza **ASP.NET Core**, **JWT**, **BCrypt** e segue o padrão **RESTful**.  
Desenvolvido para integração com ambientes baseados em **microsserviços**.

---

## 📌 Visão Geral

O `MSAuthentication` é responsável pelo processo de login, registro, alteração de senha, recuperação de conta e geração de tokens JWT com controle de perfis (roles).  
Atua como ponto central de autenticação em um sistema distribuído, desacoplado dos demais serviços.

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
├── Helpers/
│   └── PasswordHasher.cs
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

## 🤝 Contribuições

Contribuições são bem-vindas!  
Sinta-se à vontade para abrir issues ou enviar pull requests. 😄

---

## 📄 Licença

Este projeto está licenciado sob a **MIT License**.  
Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
