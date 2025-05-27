using GestaoEventos.API.Models;

namespace GestaoEventos.API.Repositories
{
    /// <summary>
    /// Interface específica para repositório de ingressos
    /// </summary>
    public interface IIngressoRepository : IRepository<Ingresso>
    {
        Task<IEnumerable<Ingresso>> GetIngressosPorEventoAsync(int eventoId);
        Task<IEnumerable<Ingresso>> GetIngressosPorEmailAsync(string email);
        Task<IEnumerable<Ingresso>> GetIngressosAtivosAsync();
        Task<Ingresso?> GetIngressoComEventoAsync(int id);
    }
}