using System.ComponentModel.DataAnnotations;

namespace GestaoEventos.API.DTO
{
    public class IngressoDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string NomeEvento { get; set; } = string.Empty;
        public string NomeComprador { get; set; } = string.Empty;
        public string EmailComprador { get; set; } = string.Empty;
        public string TelefoneComprador { get; set; } = string.Empty;
        public decimal ValorPago { get; set; }
        public DateTime DataCompra { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public string? MotivoDevolucao { get; set; }

        public EventoDto Evento { get; set; } = default!;
    }

    public class ComprarIngressoDto
    {
        [Required(ErrorMessage = "O ID do evento é obrigatório")]
        public int EventoId { get; set; }

        [Required(ErrorMessage = "O nome do comprador é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string NomeComprador { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email do comprador é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string EmailComprador { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone do comprador é obrigatório")]
        [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$", ErrorMessage = "Formato: (99) 99999-9999")]
        public string TelefoneComprador { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, 10, ErrorMessage = "Quantidade deve ser entre 1 e 10")]
        public int Quantidade { get; set; }
    }

    public class DevolverIngressoDto
    {
        public string MotivoDevolucao { get; set; } = string.Empty;
    }

    public class ResponseDto<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Dados { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}