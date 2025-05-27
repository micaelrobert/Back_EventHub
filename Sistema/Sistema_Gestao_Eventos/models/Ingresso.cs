using System.ComponentModel.DataAnnotations;

namespace GestaoEventos.API.Models
{
    /// <summary>
    /// Entidade que representa um ingresso
    /// </summary>
    public class Ingresso : BaseEntity
    {
        [Required(ErrorMessage = "ID do evento é obrigatório")]
        public int EventoId { get; set; }

        [Required(ErrorMessage = "Nome do comprador é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string NomeComprador { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "Email deve ter no máximo 150 caracteres")]
        public string EmailComprador { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string TelefoneComprador { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor pago é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal ValorPago { get; set; }

        public DateTime DataCompra { get; set; } = DateTime.Now;

        public bool Ativo { get; set; } = true;

        public DateTime? DataDevolucao { get; set; }

        [StringLength(500, ErrorMessage = "Motivo deve ter no máximo 500 caracteres")]
        public string? MotivoDevolucao { get; set; }

        // Navegação
        public virtual Evento Evento { get; set; } = null!;
    }
}