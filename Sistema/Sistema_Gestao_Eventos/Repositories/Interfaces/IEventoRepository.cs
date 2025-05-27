using GestaoEventos.API.Models;

namespace GestaoEventos.API.Repositories
{
    /// <summary>
    /// Interface específica para repositório de eventos
    /// </summary>
    public interface IEventoRepository : IRepository<Evento>
    {
        Task<IEnumerable<Evento>> GetEventosAtivosAsync();
        Task<IEnumerable<Evento>> GetEventosPorDataAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<Evento>> GetEventosComIngressosDisponiveisAsync();
        Task<Evento?> GetEventoComIngressosAsync(int id);
    }
}