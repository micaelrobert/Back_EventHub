using System.ComponentModel.DataAnnotations;

namespace GestaoEventos.API.DTO.Login
{
    public class RegistroRequestDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 100 caracteres")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação da senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmacaoSenha { get; set; } = string.Empty;
    }
}