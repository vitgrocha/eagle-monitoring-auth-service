# 📘 API - MSAuthentication

Este documento descreve os principais endpoints REST disponíveis no microserviço de autenticação.  
Todos os endpoints seguem o padrão RESTful e retornam respostas em JSON.

---

## 🔑 Login

**POST** `/api/auth/login`

Autentica um usuário com **email e senha**.

### Corpo da requisição:

```json
{
  "email": "usuario@exemplo.com",
  "password": "senha123"
}

Resposta:

{
  "token": "jwt.token.aqui",
  "refreshToken": "refresh.token.aqui"
}

✅ Registro de Usuário
POST /api/auth/register

Cria um novo usuário no sistema.

Corpo da requisição:

{
  "email": "usuario@exemplo.com",
  "password": "senha123",
  "role": "Morador"
}

Resposta esperada:

{
  "message": "Usuário registrado com sucesso."
}

🔄 Alteração de Senha
POST /api/auth/change-password

Permite que o usuário altere sua senha atual.

Corpo da requisição:

{
  "oldPassword": "senhaAntiga",
  "newPassword": "senhaNova"
}

🚪 Logout
POST /api/auth/logout

Revoga o token atual do usuário (armazenado em cache).

🧪 Cabeçalhos e Autenticação
Para acessar endpoints protegidos, envie o cabeçalho abaixo:

Authorization: Bearer {token}