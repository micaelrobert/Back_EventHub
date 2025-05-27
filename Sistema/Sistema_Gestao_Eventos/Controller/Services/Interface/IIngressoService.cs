using GestaoEventos.API.DTO;

namespace GestaoEventos.API.Services.Interfaces
{
    /// <summary>
    /// Interface para serviços de ingressos
    /// </summary>
    public interface IIngressoService
    {
        Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosAsync();
        Task<ResponseDto<IngressoDto>> GetIngressoPorIdAsync(int id);
        Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosPorEventoAsync(int eventoId);
        Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosPorEmailAsync(string email);
        Task<ResponseDto<IngressoDto>> ComprarIngressoAsync(ComprarIngressoDto comprarIngressoDto);
        Task<ResponseDto<bool>> DevolverIngressoAsync(int id, DevolverIngressoDto devolverIngressoDto);
        Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosAtivosAsync();
        Task<ResponseDto<byte[]>> GerarPdfIngressoAsync(int ingressoId, string emailUsuario);

    }
}