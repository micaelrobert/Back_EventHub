using System;

namespace GestaoEventos.API.DTO.Login
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty; // <<< NOVO CAMPO
        public DateTime Expiracao { get; set; }
    }
}