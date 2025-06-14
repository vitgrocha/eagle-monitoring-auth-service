# Arquitetura do MSAuthentication

O MSAuthentication é um microserviço desenvolvido em ASP.NET Core 7.0, responsável pela autenticação dos usuários e emissão de tokens JWT.

## Componentes principais

- **Controllers:** recebem as requisições HTTP.
- **Services:** lógica de negócio, como geração de token, hashing de senha.
- **Data:** contexto do banco de dados e migrations.
- **Middleware:** para validação e proteção dos endpoints.
- **Config:** configurações específicas, como CORS e JWT.

## Fluxo básico

1. Usuário envia credenciais via endpoint `/api/auth/login`.
2. Serviço valida as credenciais, gera JWT e refresh token.
3. JWT é usado para acessar recursos protegidos.
4. Middleware intercepta e valida tokens em cada requisição.
