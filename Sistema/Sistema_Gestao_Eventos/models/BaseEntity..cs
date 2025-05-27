using System.ComponentModel.DataAnnotations;

namespace GestaoEventos.API.Models
{
    /// <summary>
    /// Classe base para todas as entidades
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }
    }
}