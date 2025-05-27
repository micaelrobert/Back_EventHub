using GestaoEventos.API.DTO;
using GestaoEventos.API.Models;
using GestaoEventos.API.Repositories;
using GestaoEventos.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using SixLabors.Fonts;

namespace GestaoEventos.API.Services
{
    /// <summary>
    /// Serviço para gerenciamento de ingressos
    /// </summary>
    public class IngressoService : IIngressoService
    {
        private readonly IIngressoRepository _ingressoRepository;
        private readonly IEventoRepository _eventoRepository;
        private readonly ILogger<IngressoService> _logger;

        public IngressoService(
            IIngressoRepository ingressoRepository,
            IEventoRepository eventoRepository,
            ILogger<IngressoService> logger)
        {
            _ingressoRepository = ingressoRepository;
            _eventoRepository = eventoRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosAsync()
        {
            try
            {
                var ingressosComEventos = await _ingressoRepository.GetTodosComEventoAsync();

                var ingressosDto = ingressosComEventos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Ingressos recuperados com sucesso",
                    Dados = ingressosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingressos");
                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }


        public async Task<ResponseDto<IngressoDto>> GetIngressoPorIdAsync(int id)
        {
            try
            {
                var ingresso = await _ingressoRepository.GetIngressoComEventoAsync(id);

                if (ingresso == null)
                {
                    return new ResponseDto<IngressoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Ingresso não encontrado"
                    };
                }

                return new ResponseDto<IngressoDto>
                {
                    Sucesso = true,
                    Mensagem = "Ingresso encontrado",
                    Dados = MapearParaDto(ingresso)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingresso por ID: {Id}", id);
                return new ResponseDto<IngressoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosPorEventoAsync(int eventoId)
        {
            try
            {
                var ingressos = await _ingressoRepository.GetIngressosPorEventoAsync(eventoId);
                var ingressosDto = ingressos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Ingressos do evento recuperados com sucesso",
                    Dados = ingressosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingressos por evento: {EventoId}", eventoId);
                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosPorEmailAsync(string email)
        {
            try
            {
                var ingressos = await _ingressoRepository.GetIngressosPorEmailAsync(email);
                var ingressosDto = ingressos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Ingressos do email recuperados com sucesso",
                    Dados = ingressosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingressos por email: {Email}", email);
                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IngressoDto>> ComprarIngressoAsync(ComprarIngressoDto comprarIngressoDto)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdAsync(comprarIngressoDto.EventoId);

                if (evento == null)
                {
                    return new ResponseDto<IngressoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Evento não encontrado"
                    };
                }

                // Validações de negócio
                if (!evento.Ativo)
                {
                    return new ResponseDto<IngressoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Evento não está ativo",
                        Erros = new List<string> { "Evento desativado" }
                    };
                }

                if (evento.DataEvento <= DateTime.Now)
                {
                    return new ResponseDto<IngressoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Não é possível comprar ingressos para eventos que já ocorreram",
                        Erros = new List<string> { "Evento já realizado" }
                    };
                }

                if (evento.IngressosVendidos >= evento.CapacidadeMaxima)
                {
                    return new ResponseDto<IngressoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Evento esgotado",
                        Erros = new List<string> { "Não há mais ingressos disponíveis" }
                    };
                }

                // Criar ingresso
                var ingresso = new Ingresso
                {
                    EventoId = comprarIngressoDto.EventoId,
                    NomeComprador = comprarIngressoDto.NomeComprador,
                    EmailComprador = comprarIngressoDto.EmailComprador,
                    TelefoneComprador = comprarIngressoDto.TelefoneComprador,
                    ValorPago = evento.PrecoIngresso,
                    DataCompra = DateTime.Now,
                    Ativo = true
                };

                var ingressoSalvo = await _ingressoRepository.AddAsync(ingresso);

                // Atualizar contador de ingressos vendidos
                evento.IngressosVendidos++;
                evento.DataAtualizacao = DateTime.Now;
                await _eventoRepository.UpdateAsync(evento);

                // Buscar ingresso com evento para retorno
                var ingressoComEvento = await _ingressoRepository.GetIngressoComEventoAsync(ingressoSalvo.Id);

                return new ResponseDto<IngressoDto>
                {
                    Sucesso = true,
                    Mensagem = "Ingresso comprado com sucesso",
                    Dados = MapearParaDto(ingressoComEvento!)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao comprar ingresso");
                return new ResponseDto<IngressoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<bool>> DevolverIngressoAsync(int id, DevolverIngressoDto devolverIngressoDto)
        {
            try
            {
                var ingresso = await _ingressoRepository.GetIngressoComEventoAsync(id);

                if (ingresso == null)
                {
                    return new ResponseDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Ingresso não encontrado"
                    };
                }

                if (!ingresso.Ativo)
                {
                    return new ResponseDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Ingresso já foi devolvido",
                        Erros = new List<string> { "Ingresso inativo" }
                    };
                }

                if (ingresso.Evento.DataEvento <= DateTime.Now)
                {
                    return new ResponseDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Não é possível devolver ingressos de eventos que já ocorreram",
                        Erros = new List<string> { "Evento já realizado" }
                    };
                }

                // Processar devolução
                ingresso.Ativo = false;
                ingresso.DataDevolucao = DateTime.Now;
                ingresso.MotivoDevolucao = devolverIngressoDto.MotivoDevolucao;
                ingresso.DataAtualizacao = DateTime.Now;

                await _ingressoRepository.UpdateAsync(ingresso);

                // Atualizar contador de ingressos vendidos
                var evento = ingresso.Evento;
                evento.IngressosVendidos--;
                evento.DataAtualizacao = DateTime.Now;
                await _eventoRepository.UpdateAsync(evento);

                return new ResponseDto<bool>
                {
                    Sucesso = true,
                    Mensagem = "Ingresso devolvido com sucesso",
                    Dados = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao devolver ingresso: {Id}", id);
                return new ResponseDto<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<IngressoDto>>> GetIngressosAtivosAsync()
        {
            try
            {
                var ingressos = await _ingressoRepository.GetIngressosAtivosAsync();
                var ingressosDto = ingressos.Select(MapearParaDto);

                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Ingressos ativos recuperados com sucesso",
                    Dados = ingressosDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingressos ativos");
                return new ResponseDto<IEnumerable<IngressoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
        public async Task<ResponseDto<byte[]>> GerarPdfIngressoAsync(int ingressoId, string emailUsuario)
        {
            try
            {
                var ingresso = await _ingressoRepository.GetIngressoComEventoAsync(ingressoId);

                if (ingresso == null || ingresso.EmailComprador != emailUsuario)
                {
                    return new ResponseDto<byte[]>
                    {
                        Sucesso = false,
                        Mensagem = "Ingresso não encontrado ou email não corresponde.",
                        Erros = new List<string> { "Verifique o ID e o email fornecido." }
                    };
                }

                var pdfBytes = await Task.Run(() => GerarPdfDeIngresso(ingresso));

                return new ResponseDto<byte[]>
                {
                    Sucesso = true,
                    Mensagem = "PDF gerado com sucesso",
                    Dados = pdfBytes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF para ingresso: {IngressoId}, Email: {EmailUsuario}", ingressoId, emailUsuario);
                return new ResponseDto<byte[]>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor ao gerar PDF",
                    Erros = new List<string> { ex.Message }
                };
            }
        }



        private static IngressoDto MapearParaDto(Ingresso ingresso)
        {
            return new IngressoDto
            {
                Id = ingresso.Id,
                EventoId = ingresso.EventoId,
                NomeEvento = ingresso.Evento?.Nome ?? string.Empty,
                NomeComprador = ingresso.NomeComprador,
                EmailComprador = ingresso.EmailComprador,
                TelefoneComprador = ingresso.TelefoneComprador,
                ValorPago = ingresso.ValorPago,
                DataCompra = ingresso.DataCompra,
                Ativo = ingresso.Ativo,
                DataDevolucao = ingresso.DataDevolucao,
                MotivoDevolucao = ingresso.MotivoDevolucao,

                // ✅ Adiciona o mapeamento completo do evento
                Evento = ingresso.Evento != null
                    ? new EventoDto
                    {
                        Id = ingresso.Evento.Id,
                        Nome = ingresso.Evento.Nome,
                        Descricao = ingresso.Evento.Descricao,
                        DataEvento = ingresso.Evento.DataEvento,
                        Local = ingresso.Evento.Local,
                        PrecoIngresso = ingresso.Evento.PrecoIngresso,
                        CapacidadeMaxima = ingresso.Evento.CapacidadeMaxima,
                        IngressosVendidos = ingresso.Evento.IngressosVendidos,
                        Ativo = ingresso.Evento.Ativo,
                        DataCriacao = ingresso.Evento.DataCriacao
                    }
                    : null!
            };
        }

    

    private byte[] GerarPdfDeIngresso(Ingresso ingresso)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12, XFontStyle.Regular);

            string conteudo = $@"Ingresso #{ingresso.Id}
Nome do Evento: {ingresso.Evento?.Nome}
Local: {ingresso.Evento?.Local}
Data do Evento: {ingresso.Evento?.DataEvento:dd/MM/yyyy HH:mm}
Nome do Comprador: {ingresso.NomeComprador}
Email: {ingresso.EmailComprador}
Telefone: {ingresso.TelefoneComprador}
Valor Pago: R$ {ingresso.ValorPago:F2}
Data da Compra: {ingresso.DataCompra:dd/MM/yyyy HH:mm}";

            // Divide o texto em linhas
            var linhas = conteudo.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            double x = 40;
            double y = 40;
            double lineHeight = font.GetHeight() + 4; // Altura da linha + espaçamento

            foreach (var linha in linhas)
            {
                gfx.DrawString(linha, font, XBrushes.Black, new XRect(x, y, page.Width - 80, 0), XStringFormats.TopLeft);
                y += lineHeight;
            }

            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }

    }

}