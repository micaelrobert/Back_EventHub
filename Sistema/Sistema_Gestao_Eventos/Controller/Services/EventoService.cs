using GestaoEventos.API.DTO;
using GestaoEventos.API.Models;
using GestaoEventos.API.Repositories;
using GestaoEventos.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestaoEventos.API.Services
{

    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly ILogger<EventoService> _logger;

        public EventoService(IEventoRepository eventoRepository, ILogger<EventoService> logger)
        {
            _eventoRepository = eventoRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<IEnumerable<EventoDto>>> GetEventosAsync()
        {
            try
            {
                var eventos = await _eventoRepository.GetAllAsync();
                var eventosDto = eventos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Eventos recuperados com sucesso",
                    Dados = eventosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar eventos");
                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<EventoDto>> GetEventoPorIdAsync(int id)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdAsync(id);

                if (evento == null)
                {
                    return new ResponseDto<EventoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Evento não encontrado"
                    };
                }

                return new ResponseDto<EventoDto>
                {
                    Sucesso = true,
                    Mensagem = "Evento encontrado",
                    Dados = MapearParaDto(evento)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar evento por ID: {Id}", id);
                return new ResponseDto<EventoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<EventoDto>>> GetEventosAtivosAsync()
        {
            try
            {
                var eventos = await _eventoRepository.GetEventosAtivosAsync();
                var eventosDto = eventos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Eventos ativos recuperados com sucesso",
                    Dados = eventosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar eventos ativos");
                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<EventoDto>>> GetEventosComIngressosDisponiveisAsync()
        {
            try
            {
                var eventos = await _eventoRepository.GetEventosComIngressosDisponiveisAsync();
                var eventosDto = eventos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Eventos com ingressos disponíveis recuperados com sucesso",
                    Dados = eventosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar eventos com ingressos disponíveis");
                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<EventoDto>> CriarEventoAsync(CriarEventoDto criarEventoDto)
        {
            try
            {
               
                if (criarEventoDto.DataEvento <= DateTime.Now)
                {
                    return new ResponseDto<EventoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Data do evento deve ser futura",
                        Erros = new List<string> { "Data do evento inválida" }
                    };
                }

                var evento = new Evento
                {
                    Nome = criarEventoDto.Nome,
                    Descricao = criarEventoDto.Descricao,
                    DataEvento = criarEventoDto.DataEvento,
                    Local = criarEventoDto.Local,
                    PrecoIngresso = criarEventoDto.PrecoIngresso,
                    CapacidadeMaxima = criarEventoDto.CapacidadeMaxima,
                    Ativo = true
                };

                var eventoSalvo = await _eventoRepository.AddAsync(evento);

                return new ResponseDto<EventoDto>
                {
                    Sucesso = true,
                    Mensagem = "Evento criado com sucesso",
                    Dados = MapearParaDto(eventoSalvo)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar evento");
                return new ResponseDto<EventoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<EventoDto>> AtualizarEventoAsync(int id, AtualizarEventoDto atualizarEventoDto)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdAsync(id);

                if (evento == null)
                {
                    return new ResponseDto<EventoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Evento não encontrado"
                    };
                }

                
                if (atualizarEventoDto.DataEvento <= DateTime.Now)
                {
                    return new ResponseDto<EventoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Data do evento deve ser futura",
                        Erros = new List<string> { "Data do evento inválida" }
                    };
                }

                if (atualizarEventoDto.CapacidadeMaxima < evento.IngressosVendidos)
                {
                    return new ResponseDto<EventoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Capacidade não pode ser menor que ingressos já vendidos",
                        Erros = new List<string> { $"Ingressos vendidos: {evento.IngressosVendidos}" }
                    };
                }

                
                evento.Nome = atualizarEventoDto.Nome;
                evento.Descricao = atualizarEventoDto.Descricao;
                evento.DataEvento = atualizarEventoDto.DataEvento;
                evento.Local = atualizarEventoDto.Local;
                evento.PrecoIngresso = atualizarEventoDto.PrecoIngresso;
                evento.CapacidadeMaxima = atualizarEventoDto.CapacidadeMaxima;
                evento.Ativo = atualizarEventoDto.Ativo;
                evento.DataAtualizacao = DateTime.Now;

                var eventoAtualizado = await _eventoRepository.UpdateAsync(evento);

                return new ResponseDto<EventoDto>
                {
                    Sucesso = true,
                    Mensagem = "Evento atualizado com sucesso",
                    Dados = MapearParaDto(eventoAtualizado)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar evento: {Id}", id);
                return new ResponseDto<EventoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<bool>> DeletarEventoAsync(int id)
        {
            try
            {
                var evento = await _eventoRepository.GetEventoComIngressosAsync(id);

                if (evento == null)
                {
                    return new ResponseDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Evento não encontrado"
                    };
                }

                if (evento.Ingressos.Any(i => i.Ativo))
                {
                    return new ResponseDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Não é possível deletar evento com ingressos ativos",
                        Erros = new List<string> { "Existem ingressos vendidos para este evento" }
                    };
                }

                await _eventoRepository.DeleteAsync(id);

                return new ResponseDto<bool>
                {
                    Sucesso = true,
                    Mensagem = "Evento deletado com sucesso",
                    Dados = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar evento: {Id}", id);
                return new ResponseDto<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<EventoDto>>> BuscarEventosPorDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                var eventos = await _eventoRepository.GetEventosPorDataAsync(dataInicio, dataFim);
                var eventosDto = eventos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Eventos encontrados",
                    Dados = eventosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar eventos por data");
                return new ResponseDto<IEnumerable<EventoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        private static EventoDto MapearParaDto(Evento evento)
        {
            return new EventoDto
            {
                Id = evento.Id,
                Nome = evento.Nome,
                Descricao = evento.Descricao,
                DataEvento = evento.DataEvento,
                Local = evento.Local,
                PrecoIngresso = evento.PrecoIngresso,
                CapacidadeMaxima = evento.CapacidadeMaxima,
                IngressosVendidos = evento.IngressosVendidos,
                Ativo = evento.Ativo,
                DataCriacao = evento.DataCriacao
            };
        }
    }
}