using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiCatalogo.Services;

public class TokenService : ITokenService
{
    
    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
    {   
        // Obtém a chave secreta da configuração, ou lança exceção se não estiver definida
        var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ??
                  throw new InvalidOperationException("Invalid secret Key");
    
        // Converte a chave secreta em bytes
        var privateKey = Encoding.UTF8.GetBytes(key);
    
        // Cria as credenciais de assinatura usando HMAC-SHA256
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256Signature);
    
        // Define os parâmetros do token: claims, validade, emissor, audiência e credenciais
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT")
                .GetValue<double>("TokenValidityInMinutes")),
            Audience = _config.GetSection("JWT")
                .GetValue<string>("ValidAudience"),
            Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"),
            SigningCredentials = signingCredentials
        };
        // Instancia o gerador de tokens JWT
        var tokenHandler = new JwtSecurityTokenHandler();
    
        // Gera o token JWT com base nas configurações fornecidas
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
    
        // Retorna o token gerado
        return token;
    }
    public string GenerateRefreshToken()
    {
        // Cria um array de bytes para armazenar dados aleatórios
        var secureRandomBytes = new byte[128];

        // Instancia um gerador de números aleatórios criptograficamente seguros
        using var randomNumberGenerator = RandomNumberGenerator.Create();

        // Preenche o array com bytes aleatórios
        randomNumberGenerator.GetBytes(secureRandomBytes);

        // Converte os bytes aleatórios para uma string em Base64
        var refreshToken = Convert.ToBase64String(secureRandomBytes);

        // Retorna o token de refresh gerado
        return refreshToken;
    }
    
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config)
    {
        // Obtém a chave secreta da configuração, ou lança exceção se não estiver definida
        var secretKey = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Invalid key");

        // Define os parâmetros de validação do token (sem validar audiência, emissor ou expiração)
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false
        };

        // Instancia o manipulador de tokens JWT
        var tokenHandler = new JwtSecurityTokenHandler();

        // Valida o token e extrai o principal (identidade e claims)
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
            out SecurityToken securityToken);

        // Verifica se o token é realmente um JWT e se foi assinado usando o algoritmo esperado
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        // Retorna o principal extraído do token
        return principal;
    }
}