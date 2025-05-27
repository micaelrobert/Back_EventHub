using GestaoEventos.API.Models;

namespace GestaoEventos.API.Repositories
{
    public interface IIngressoRepository : IRepository<Ingresso>
    {
        Task<IEnumerable<Ingresso>> GetIngressosPorEventoAsync(int eventoId);
        Task<IEnumerable<Ingresso>> GetIngressosPorEmailAsync(string email);
        Task<IEnumerable<Ingresso>> GetIngressosAtivosAsync();
        Task<Ingresso?> GetIngressoComEventoAsync(int id);
        Task<IEnumerable<Ingresso>> GetTodosComEventoAsync(); // <-- ADICIONADO
    }
}
