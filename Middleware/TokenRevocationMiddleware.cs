using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public TokenRevocationMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authorization = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            var token = authorization["Bearer ".Length..].Trim();

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido.");
                return;
            }

            var tokenId = jwtToken.Id;

            if (!string.IsNullOrEmpty(tokenId))
            {
                if (_cache.TryGetValue(tokenId, out _))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token revogado.");
                    return;
                }
            }
        }

        // Continua pipeline
        await _next(context);
    }
}
