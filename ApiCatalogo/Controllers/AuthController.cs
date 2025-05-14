using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Models;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiCatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
    {
        // Busca o usuário no banco de dados pelo nome de usuário informado
        var user = await _userManager.FindByNameAsync(model.Username!);

        // Verifica se o usuário existe e se a senha fornecida está correta
        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            // Recupera as roles (perfis) associadas ao usuário
            var userRoler = await _userManager.GetRolesAsync(user);

            // Cria uma lista de claims (informações que estarão presentes no token)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!), // Nome do usuário
                new Claim(ClaimTypes.Email, user.Email!),   // Email do usuário
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Identificador único do token
            };

            // Adiciona as roles do usuário como claims
            foreach (var userRole in userRoler)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Gera o token JWT de acesso com as claims e configurações
            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

            // Gera um refresh token (normalmente uma string aleatória segura)
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Lê a duração do token de refresh a partir da configuração (em minutos)
            _ = int.TryParse(_configuration["JWT:TokenExpiryInMinutes"], out int refreshTokenExpiryInMinutes);

            // Atualiza o tempo de expiração do refresh token no usuário
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenExpiryInMinutes);

            // Persiste a atualização do usuário no banco de dados
            await _userManager.UpdateAsync(user);

            // Retorna a resposta com o token, o refresh token e o tempo de expiração
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token), // Token JWT formatado em string
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        // Caso as credenciais estejam incorretas, retorna não autorizado
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
    {
        // Verifica se já existe um usuário com o mesmo nome
        var userExists = await _userManager.FindByNameAsync(model.Username!);

        if (userExists != null)
        {
            // Retorna erro 500 se o usuário já existe
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDTO { Status = "Error", Message = "User already exists!" });
        }

        // Cria um novo objeto de usuário com as informações fornecidas
        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(), // Usado para segurança de tokens
            UserName = model.Username
        };

        // Tenta criar o usuário com a senha fornecida
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            // Retorna erro se a criação falhar (senha inválida, etc)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDTO { Status = "Error", Message = "User creation failed." });
        }
        // Retorna sucesso se o usuário for criado com sucesso
        return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
    }
    
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    {
        // Valida o corpo da requisição
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }

        // Obtém o access token e refresh token enviados
        string? accessToken = tokenModel.AccessToken
                              ?? throw new ArgumentNullException(nameof(tokenModel));

        string? refreshToken = tokenModel.RefreshToken
                               ?? throw new ArgumentException(nameof(tokenModel));

        // Recupera as claims do token expirado
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal == null)
        {
            return BadRequest("Invalid access token/refresh token");
        }

        // Obtém o nome do usuário a partir do token
        string username = principal.Identity.Name;

        // Busca o usuário no banco
        var user = await _userManager.FindByNameAsync(username!);

        // Verifica se o refresh token corresponde ao armazenado e se ainda está válido
        if (user == null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access token/refresh token");
        }

        // Gera um novo token de acesso e um novo refresh token
        var newAccessToken = _tokenService.GenerateAccessToken(
            principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Atualiza o refresh token do usuário no banco
        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        // Retorna os novos tokens
        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        // Busca o usuário pelo nome
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("Invalid user name");

        // Remove o refresh token do usuário, invalidando futuras renovações
        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        // Retorna 204 (sem conteúdo) indicando sucesso
        return NoContent();
    }
}
