# 🔐 Segurança no MSAuthentication

Este documento apresenta os mecanismos de segurança utilizados no microserviço de autenticação, incluindo criptografia de senhas, autenticação com JWT, controle de roles e middleware de proteção.

---

## 🧂 Hash de Senha

As senhas dos usuários são **criptografadas com BCrypt** antes de serem armazenadas no banco de dados.

- Biblioteca utilizada: `BCrypt.Net`
- O hash gerado é único mesmo para senhas iguais, pois inclui um salt aleatório.
- Durante o login, o sistema compara a senha informada com o hash armazenado utilizando `BCrypt.Verify`.

---

## 🔑 Autenticação com JWT

Após o login, o sistema gera um **token JWT (JSON Web Token)** assinado com uma chave secreta.

### Estrutura do Token:

- **Header**: Algoritmo e tipo de token (`HS256`)
- **Payload**: Informações do usuário (ID, email, role)
- **Signature**: Garante que o token não foi alterado

### Exemplo do payload:

```json
{
  "sub": "12345",
  "email": "usuario@exemplo.com",
  "role": "Admin",
  "exp": 1718129390
}
O token tem um tempo de expiração configurado (ex: 1h).

A assinatura é feita com a chave definida em appsettings.json.

🔁 Refresh Token (opcional)
Além do JWT, é gerado um refresh token que permite renovar a sessão sem precisar refazer o login.

Pode ser armazenado em cache ou no banco (futuramente).

Permite a estratégia de "Token Rotation" para segurança avançada.

🧪 Middleware de Autenticação
Foi criado um Middleware personalizado que:

Verifica a presença e validade do token JWT.

Decodifica o payload e injeta os dados do usuário no HttpContext.

Bloqueia o acesso caso o token esteja expirado ou inválido.

🧭 Controle de Acesso por Role
O sistema utiliza o enum UserRole para definir perfis de acesso:


public enum UserRole
{
    Admin,
    AdminTerceirizado,
    Portaria,
    Morador,
    Visitante
}
Cada usuário recebe uma role no momento do registro.

A role é incluída no token e pode ser usada para proteger endpoints.

⛔ Em desenvolvimento: Middleware de Autorização
Está sendo desenvolvido um middleware adicional para:

Verificar se a role do usuário possui permissão para acessar o endpoint solicitado.

Definir políticas de acesso por tipo de role (ex: apenas Admin pode criar usuários).


🛡️ Boas práticas aplicadas
Tokens curtos (JWTs) com expiração definida

Senhas nunca armazenadas em texto puro

Uso de middleware para blindagem automática das rotas

Roles centralizadas no UserRole enum

Planejamento para revogação de tokens via cache

👩‍💻 Desenvolvido por: Vitória Gabriella
📁 Projeto: Eagle Monitoring
📆 Versão: 1.0 (Março/2024)

