# 🧪 Documento de Testes – Microserviço de Autenticação

---

### 📋 Informações Gerais
- **Projeto:** MSAuthentication  
- **Responsável:** Vitória Gabriella  
- **Data:** 2025-06-14 

---

### 🚦 Testes Realizados

| 📝 Teste                  | 🔗 Endpoint                 | 📥 Entrada (Request)                                  | ✅ Resultado Esperado               | 🎯 Resultado Obtido               | ✔️ Status        | 💡 Observações                       |
|---------------------------|----------------------------|-----------------------------------------------------|-----------------------------------|---------------------------------|------------------|------------------------------------|
| Registro de usuário       | `POST /api/auth/register`  | `{ "email": "teste@exemplo.com", "password": "Senha123!" }` | Usuário criado com sucesso        | Usuário criado com sucesso       | ✅ OK            | -                                  |
| Login com senha correta    | `POST /api/auth/login`     | `{ "email": "teste@exemplo.com", "password": "Senha123!" }` | Retorna token + refresh token     | Retorna token + refresh token    | ✅ OK            | -                                  |
| Login com senha incorreta  | `POST /api/auth/login`     | `{ "email": "teste@exemplo.com", "password": "errada" }`     | Retorna Unauthorized              | Retorna Unauthorized             | ✅ OK            | -                                  |
| Recuperação de senha (email)| `POST /api/auth/password/forgot` | `{ "email": "teste@exemplo.com" }`                   | Envio de email de recuperação     | Email enviado (ou mock)          | ⚙️ Em desenvolvimento | Email ainda não implementado       |
| Verificação 2FA            | `POST /api/auth/verify-code` | `{ "email": "teste@exemplo.com", "code": "123456" }` | Retorna token JWT e refresh token | Retorna token JWT e refresh token| ⚙️ Em desenvolvimento | Ajustar envio do código por email  |

---

### 🛠️ Próximos Passos
- Finalizar envio do código 2FA por e-mail  
- Implementar validação de regras de segurança para senhas  
- Automatizar testes (unitários e integração)  

---

> **Dica:** Atualize esse documento sempre que fizer novos testes para manter tudo fresquinho! 🌸

