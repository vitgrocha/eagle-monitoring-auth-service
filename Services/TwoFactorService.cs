using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class TwoFactorService
{
    // Armazena código temporário: email -> (codigo, data de expiração)
    private readonly ConcurrentDictionary<string, (string Code, DateTime Expiration)> _codes 
        = new();

    private readonly TimeSpan _codeValidity = TimeSpan.FromMinutes(5);

    public Task<string> GenerateCodeAsync(string email)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var expiration = DateTime.UtcNow.Add(_codeValidity);

        _codes[email] = (code, expiration);

        Console.WriteLine($"[2FA] Código para {email}: {code} (válido por 5 minutos)");

        return Task.FromResult(code);
    }

    public Task<bool> ValidateCodeAsync(string email, string code)
    {
        if (_codes.TryGetValue(email, out var info))
        {
            if (info.Expiration > DateTime.UtcNow && info.Code == code)
            {
                _codes.TryRemove(email, out _);
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }
}
