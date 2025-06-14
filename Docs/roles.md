# 🧑‍💼 Perfis de Acesso (Roles) no MSAuthentication

Este documento apresenta os perfis (roles) utilizados no sistema de autenticação, sua finalidade e como são aplicados no controle de acesso.

---

## 📌 O que são roles?

**Roles** representam os níveis de permissão de cada usuário no sistema.  
Cada usuário é atribuído a uma role específica no momento do cadastro, determinando quais ações ele pode realizar.

---

## 🧑‍🔧 Enum `UserRole`

O controle de perfis é centralizado no enum `UserRole`, conforme abaixo:

```csharp
public enum UserRole
{
    Admin,               // Full system access
    ThirdPartyAdmin,     // Management of third-party collaborators
    Gatekeeper,          // Controls entry and exit
    Resident,            // End user of the condominium
    Visitor              // Temporary or limited access
}

📝 Atribuição no Registro
Durante o registro (POST /api/auth/register), o usuário recebe uma role que define seu nível de acesso.

Exemplo de corpo de requisição:

{
  "email": "porteiro@condominio.com",
  "password": "SenhaSegura123!",
  "role": "Portaria"
}
⚠️ Apenas administradores devem ter permissão para cadastrar usuários com roles elevadas, como Admin.

🔒 Controle de Acesso por Role
A role do usuário é inserida no token JWT após o login.

O sistema pode verificar essa role para autorizar ou restringir o acesso a determinadas rotas.

Exemplo de verificação:

if (user.Role == UserRole.Admin)
{
    // Permitir acesso
}
🧪 Middleware de Autenticação
O middleware já implementado valida o token e identifica o usuário.

Em desenvolvimento:
🔐 Middleware de autorização por roles, que vai:

Impedir que usuários acessem endpoints fora do seu perfil

Permitir políticas como:

Apenas Admin e AdminTerceirizado podem criar usuários

Portaria acessa apenas endpoints de entrada e saída

Morador visualiza apenas dados próprios

🌱 Expansão futura
O enum UserRole pode ser expandido com novos perfis conforme o sistema crescer.
Sugestões:

Síndico

Segurança

PrestadorServico

TI

Regras claras de permissão devem ser definidas conforme novos papéis forem adicionados.

👩‍💻 Desenvolvido por: Vitória Gabriella
📁 Projeto: Eagle Monitoring
📆 Versão: 1.0 (Março/2024)


