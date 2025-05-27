using GestaoEventos.API.DTO;
using GestaoEventos.API.Models;

namespace GestaoEventos.API.Services.Interfaces
{
    /// <summary>
    /// Interface para serviços de eventos
    /// </summary>
    public interface IEventoService
    {
        Task<ResponseDto<IEnumerable<EventoDto>>> GetEventosAsync();
        Task<ResponseDto<EventoDto>> GetEventoPorIdAsync(int id);
        Task<ResponseDto<IEnumerable<EventoDto>>> GetEventosAtivosAsync();
        Task<ResponseDto<IEnumerable<EventoDto>>> GetEventosComIngressosDisponiveisAsync();
        Task<ResponseDto<EventoDto>> CriarEventoAsync(CriarEventoDto criarEventoDto);
        Task<ResponseDto<EventoDto>> AtualizarEventoAsync(int id, AtualizarEventoDto atualizarEventoDto);
        Task<ResponseDto<bool>> DeletarEventoAsync(int id);
        Task<ResponseDto<IEnumerable<EventoDto>>> BuscarEventosPorDataAsync(DateTime dataInicio, DateTime dataFim);
    }
}