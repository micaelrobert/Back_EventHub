using System.ComponentModel.DataAnnotations;
using Models;

namespace GestaoEventos.API.Models
{
    /// <summary>
    /// Entidade que representa um evento
    /// </summary>
    public class Evento : BaseEntity
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data do evento é obrigatória")]
        public DateTime DataEvento { get; set; }

        [Required(ErrorMessage = "Local é obrigatório")]
        [StringLength(300, ErrorMessage = "Local deve ter no máximo 300 caracteres")]
        public string Local { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço do ingresso é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal PrecoIngresso { get; set; }

        [Required(ErrorMessage = "Capacidade máxima é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacidade deve ser maior que zero")]
        public int CapacidadeMaxima { get; set; }

        public int IngressosVendidos { get; set; } = 0;

        public bool Ativo { get; set; } = true;

        // Propriedade calculada
        public int IngressosDisponiveis => CapacidadeMaxima - IngressosVendidos;

        // Navegação
        public virtual ICollection<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }
}