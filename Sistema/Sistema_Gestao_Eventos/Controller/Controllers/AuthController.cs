using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using GestaoEventos.API.Data;
using GestaoEventos.API.Models;
using GestaoEventos.API.DTO.Login;
using GestaoEventos.API.DTO;
using GestaoEventos.API.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace GestaoEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EventosContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(EventosContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseDto<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 401)]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<LoginResponseDto> { Sucesso = false, Mensagem = "Requisição inválida." });
            }

            _logger.LogInformation("Tentativa de login para o email: {Email}", loginRequest.Email);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequest.Email.ToLower());

            if (usuario == null || !PasswordHasher.VerifyPasswordHash(loginRequest.Senha, usuario.PasswordHash, usuario.PasswordSalt))
            {
                _logger.LogWarning("Falha no login para o email: {Email} - Credenciais inválidas.", loginRequest.Email);
                return Unauthorized(new ResponseDto<LoginResponseDto> { Sucesso = false, Mensagem = "Email ou senha inválidos." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["Jwt:Key"] ?? "ESTE_EH_UM_SEGREDO_MUITO_LONGO_E_SEGURO_PARA_EVENTHUB_PELO_MENOS_32_CARACTERES";
            var key = Encoding.ASCII.GetBytes(keyString);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.NomeUsuario),
                new Claim(ClaimTypes.Role, usuario.Papel)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "EventHubAPI",
                Audience = _configuration["Jwt:Audience"] ?? "EventHubApp"
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            _logger.LogInformation("Login bem-sucedido para o email: {Email}, Papel: {Papel}, NomeUsuario: {NomeUsuario}", usuario.Email, usuario.Papel, usuario.NomeUsuario);

            return Ok(new ResponseDto<LoginResponseDto>
            {
                Sucesso = true,
                Mensagem = "Login realizado com sucesso!",
                Dados = new LoginResponseDto
                {
                    Token = tokenString,
                    Email = usuario.Email,
                    Papel = usuario.Papel,
                    NomeUsuario = usuario.NomeUsuario,
                    Expiracao = tokenDescriptor.Expires ?? DateTime.UtcNow.AddHours(8)
                }
            });
        }

        [HttpPost("registrar")]
        [ProducesResponseType(typeof(ResponseDto<object>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 400)]
        public async Task<ActionResult<ResponseDto<object>>> Registrar([FromBody] RegistroRequestDto registroRequest)
        {
            _logger.LogInformation("Tentativa de registro para o email: {Email} e NomeUsuario: {NomeUsuario}", registroRequest.Email, registroRequest.NomeUsuario);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "Dados de registro inválidos.",
                    Erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var usuarioExistenteEmail = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == registroRequest.Email.ToLower());
            if (usuarioExistenteEmail)
            {
                _logger.LogWarning("Tentativa de registro para email já existente: {Email}", registroRequest.Email);
                return BadRequest(new ResponseDto<object> { Sucesso = false, Mensagem = "Este email já está cadastrado." });
            }

            var usuarioExistenteNome = await _context.Usuarios.AnyAsync(u => u.NomeUsuario.ToLower() == registroRequest.NomeUsuario.ToLower());
            if (usuarioExistenteNome)
            {
                _logger.LogWarning("Tentativa de registro para NomeUsuario já existente: {NomeUsuario}", registroRequest.NomeUsuario);
                return BadRequest(new ResponseDto<object> { Sucesso = false, Mensagem = "Este nome de usuário já está em uso." });
            }

            PasswordHasher.CreatePasswordHash(registroRequest.Senha, out byte[] passwordHash, out byte[] passwordSalt);

            var novoUsuario = new Usuario
            {
                Email = registroRequest.Email.ToLower(),
                NomeUsuario = registroRequest.NomeUsuario,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Papel = "Comum"
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Novo usuário registrado com sucesso: {Email}, NomeUsuario: {NomeUsuario}", novoUsuario.Email, novoUsuario.NomeUsuario);

            return Ok(new ResponseDto<object> { Sucesso = true, Mensagem = "Usuário registrado com sucesso!" });
        }
    }
}