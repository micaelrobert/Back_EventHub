namespace GestaoEventos.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Papel { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty;
    }
}