using Microsoft.EntityFrameworkCore;
using GestaoEventos.API.Data;
using GestaoEventos.API.Models;

namespace GestaoEventos.API.Repositories
{
    /// <summary>
    /// Repositório específico para eventos
    /// </summary>
    public class EventoRepository : Repository<Evento>, IEventoRepository
    {
        public EventoRepository(EventosContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Evento>> GetEventosAtivosAsync()
        {
            return await _dbSet
                .Where(e => e.Ativo && e.DataEvento > DateTime.Now)
                .OrderBy(e => e.DataEvento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> GetEventosPorDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Where(e => e.DataEvento >= dataInicio && e.DataEvento <= dataFim)
                .OrderBy(e => e.DataEvento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> GetEventosComIngressosDisponiveisAsync()
        {
            return await _dbSet
                .Where(e => e.Ativo && e.DataEvento > DateTime.Now && e.IngressosVendidos < e.CapacidadeMaxima)
                .OrderBy(e => e.DataEvento)
                .ToListAsync();
        }

        public async Task<Evento?> GetEventoComIngressosAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Ingressos)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}