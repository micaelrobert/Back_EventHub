namespace GestaoEventos.API.DTO
{
    /// <summary>
    /// DTO para retorno de dados do evento
    /// </summary>
    public class EventoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataEvento { get; set; }
        public string Local { get; set; } = string.Empty;
        public decimal PrecoIngresso { get; set; }
        public int CapacidadeMaxima { get; set; }
        public int IngressosVendidos { get; set; }
        public int IngressosDisponiveis => CapacidadeMaxima - IngressosVendidos;
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    /// <summary>
    /// DTO para criação de evento
    /// </summary>
    public class CriarEventoDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataEvento { get; set; }
        public string Local { get; set; } = string.Empty;
        public decimal PrecoIngresso { get; set; }
        public int CapacidadeMaxima { get; set; }
    }

    /// <summary>
    /// DTO para atualização de evento
    /// </summary>
    public class AtualizarEventoDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataEvento { get; set; }
        public string Local { get; set; } = string.Empty;
        public decimal PrecoIngresso { get; set; }
        public int CapacidadeMaxima { get; set; }
        public bool Ativo { get; set; }
    }
}