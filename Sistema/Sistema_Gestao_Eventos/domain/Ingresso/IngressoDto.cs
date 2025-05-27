namespace GestaoEventos.API.DTO
{
    /// <summary>
    /// DTO para retorno de dados do ingresso
    /// </summary>
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
    }

    /// <summary>
    /// DTO para compra de ingresso
    /// </summary>
    public class ComprarIngressoDto
    {
        public int EventoId { get; set; }
        public string NomeComprador { get; set; } = string.Empty;
        public string EmailComprador { get; set; } = string.Empty;
        public string TelefoneComprador { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para devolução de ingresso
    /// </summary>
    public class DevolverIngressoDto
    {
        public string MotivoDevolucao { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resposta genérica
    /// </summary>
    public class ResponseDto<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Dados { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}