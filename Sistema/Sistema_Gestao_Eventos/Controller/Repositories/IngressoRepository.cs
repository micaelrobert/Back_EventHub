﻿using Microsoft.EntityFrameworkCore;
using GestaoEventos.API.Data;
using GestaoEventos.API.Models;

namespace GestaoEventos.API.Repositories
{
 
    public class IngressoRepository : Repository<Ingresso>, IIngressoRepository
    {
        public IngressoRepository(EventosContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ingresso>> GetIngressosPorEventoAsync(int eventoId)
        {
            return await _dbSet
                .Include(i => i.Evento)
                .Where(i => i.EventoId == eventoId)
                .OrderByDescending(i => i.DataCompra)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ingresso>> GetIngressosPorEmailAsync(string email)
        {
            return await _dbSet
                .Include(i => i.Evento)
                .Where(i => i.EmailComprador.ToLower() == email.ToLower())
                .OrderByDescending(i => i.DataCompra)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ingresso>> GetIngressosAtivosAsync()
        {
            return await _dbSet
                .Include(i => i.Evento)
                .Where(i => i.Ativo)
                .OrderByDescending(i => i.DataCompra)
                .ToListAsync();
        }

        public async Task<Ingresso?> GetIngressoComEventoAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Evento)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Ingresso>> GetTodosComEventoAsync()
        {
            return await _dbSet
                .Include(i => i.Evento)
                .OrderByDescending(i => i.DataCompra)
                .ToListAsync();
        }
    }
}
