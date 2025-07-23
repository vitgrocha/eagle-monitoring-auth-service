using Microsoft.EntityFrameworkCore;
using MsAuthentication.Data;
using MsAuthentication.Services;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MsAuthentication.Config;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

// Carrega variáveis do .env na raiz do projeto
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Carrega appsettings.json e variáveis de ambiente (.env já carregado pelo DotNetEnv)
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configuração do banco de dados via connection string do appsettings ou .env
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbConnection")));

// Configurações JWT via variáveis de ambiente
var jwtConfig = new JwtConfig
{
    Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "fallback-key",
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "fallback-issuer",
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "fallback-audience",
    ExpirationInMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES"), out int expiration) ? expiration : 60
};
builder.Services.AddSingleton(jwtConfig);

builder.Services.AddMemoryCache();

// Configuração do CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Serviços de autenticação, email e 2FA
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<TwoFactorService>();
builder.Services.AddTransient<EmailService>();

// SMTP Settings a partir das variáveis de ambiente
var smtpSettings = new SmtpSettings
{
    Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "",
    Port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587,
    EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL"), out var ssl) && ssl,
    UserName = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "",
    Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "",
    From = Environment.GetEnvironmentVariable("SMTP_FROM") ?? ""
};

// ✅ Corrigido: atribuindo as propriedades manualmente
builder.Services.Configure<SmtpSettings>(options =>
{
    options.Host = smtpSettings.Host;
    options.Port = smtpSettings.Port;
    options.EnableSsl = smtpSettings.EnableSsl;
    options.UserName = smtpSettings.UserName;
    options.Password = smtpSettings.Password;
    options.From = smtpSettings.From;
});

// Configuração da autenticação JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
        };
    });

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MsAuthentication API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Insira o token JWT assim: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    app.Urls.Clear();
    app.Urls.Add("http://*:80");
}

// Pipeline de middlewares
app.UseCors(MyAllowSpecificOrigins);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsAuthentication API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseMiddleware<TokenRevocationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
