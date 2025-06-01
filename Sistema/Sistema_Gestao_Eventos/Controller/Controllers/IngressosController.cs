using Microsoft.AspNetCore.Mvc;
using GestaoEventos.API.DTO;
using GestaoEventos.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoEventos.API.Models;

namespace GestaoEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class IngressosController : ControllerBase
    {
        private readonly IIngressoService _ingressoService;
        private readonly IPdfService _pdfService;
        private readonly ILogger<IngressosController> _logger;

        public IngressosController(IIngressoService ingressoService, IPdfService pdfService, ILogger<IngressosController> logger)
        {
            _ingressoService = ingressoService;
            _pdfService = pdfService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetIngressos()
        {
            _logger.LogInformation("Buscando todos os ingressos");
            var resultado = await _ingressoService.GetIngressosAsync();
            if (resultado.Sucesso)
                return Ok(resultado);
            return StatusCode(500, resultado);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto<IngressoDto>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IngressoDto>>> GetIngresso(int id)
        {
            _logger.LogInformation("Buscando ingresso com ID: {Id}", id);
            var resultado = await _ingressoService.GetIngressoPorIdAsync(id);
            if (resultado.Sucesso && resultado.Dados != null)
                return Ok(resultado);
            if (!resultado.Sucesso && resultado.Mensagem.Contains("não encontrado"))
                return NotFound(resultado);
            return StatusCode(500, resultado);
        }

        [HttpGet("evento/{eventoId}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetIngressosPorEvento(int eventoId)
        {
            _logger.LogInformation("Buscando ingressos do evento ID: {EventoId}", eventoId);
            var resultado = await _ingressoService.GetIngressosPorEventoAsync(eventoId);
            if (resultado.Sucesso)
                return Ok(resultado);
            return StatusCode(500, resultado);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetIngressosPorEmail(string email)
        {
            _logger.LogInformation("Buscando ingressos do email: {Email}", email);
            var resultado = await _ingressoService.GetIngressosPorEmailAsync(email);
            if (resultado.Sucesso && resultado.Dados != null && resultado.Dados.Any())
                return Ok(resultado);
            if (resultado.Sucesso && (resultado.Dados == null || !resultado.Dados.Any()))
                return Ok(new ResponseDto<IEnumerable<IngressoDto>> { Sucesso = true, Dados = new List<IngressoDto>(), Mensagem = "Nenhum ingresso encontrado para este email." });

            return StatusCode(500, resultado);
        }

        [HttpGet("meus")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 401)]
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetMeusIngressos()
        {
            var emailUsuarioLogado = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(emailUsuarioLogado))
            {
                _logger.LogWarning("Tentativa de acesso a /meus sem email na claim do token.");
                return Unauthorized(new ResponseDto<object> { Sucesso = false, Mensagem = "Informação do usuário não encontrada no token." });
            }

            _logger.LogInformation("Buscando ingressos para o usuário logado: {Email}", emailUsuarioLogado);

            var resultado = await _ingressoService.GetIngressosPorEmailAsync(emailUsuarioLogado);

            if (resultado.Sucesso)
            {
                if (resultado.Dados == null || !resultado.Dados.Any())
                {
                    return Ok(new ResponseDto<IEnumerable<IngressoDto>> { Sucesso = true, Dados = new List<IngressoDto>(), Mensagem = "Você ainda não possui ingressos." });
                }
                return Ok(resultado);
            }

            return StatusCode(500, resultado);
        }

        [HttpPost("comprar")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDto<IngressoDto>), 201)]
        [ProducesResponseType(typeof(ResponseDto<object>), 400)]
        [ProducesResponseType(typeof(ResponseDto<object>), 500)]
        public async Task<ActionResult<ResponseDto<IngressoDto>>> ComprarIngresso([FromBody] ComprarIngressoDto comprarIngressoDto)
        {
            _logger.LogInformation("Comprando ingresso para evento ID: {EventoId} pelo usuário {Email}",
                comprarIngressoDto.EventoId, User.FindFirstValue(ClaimTypes.Email));

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<IngressoDto>
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos",
                    Erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var resultado = await _ingressoService.ComprarIngressoAsync(comprarIngressoDto);

            if (resultado.Sucesso && resultado.Dados != null)
                return CreatedAtAction(nameof(GetIngresso), new { id = resultado.Dados.Id }, resultado);

            if (!resultado.Sucesso && resultado.Erros != null && resultado.Erros.Any())
                return BadRequest(resultado);
            if (!resultado.Sucesso)
                return BadRequest(new ResponseDto<IngressoDto> { Sucesso = false, Mensagem = resultado.Mensagem });

            return StatusCode(500, resultado);
        }

        [HttpPost("{id}/devolver")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDto<bool>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 400)]
        [ProducesResponseType(typeof(ResponseDto<object>), 404)]
        [ProducesResponseType(typeof(ResponseDto<object>), 500)]
        public async Task<ActionResult<ResponseDto<bool>>> DevolverIngresso(int id, [FromBody] DevolverIngressoDto devolverIngressoDto)
        {
            _logger.LogInformation("Devolvendo ingresso ID: {Id} pelo usuário {Email}", id, User.FindFirstValue(ClaimTypes.Email));

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<bool>
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos",
                    Erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var resultado = await _ingressoService.DevolverIngressoAsync(id, devolverIngressoDto);

            if (resultado.Sucesso)
                return Ok(resultado);
            if (!resultado.Sucesso && resultado.Mensagem.Contains("não encontrado"))
                return NotFound(resultado);
            if (!resultado.Sucesso && resultado.Erros != null && resultado.Erros.Any())
                return BadRequest(resultado);

            return StatusCode(500, resultado);
        }

        [HttpGet("ativos")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetIngressosAtivos()
        {
            _logger.LogInformation("Buscando ingressos ativos");
            var resultado = await _ingressoService.GetIngressosAtivosAsync();
            if (resultado.Sucesso)
                return Ok(resultado);
            return StatusCode(500, resultado);
        }


        [HttpGet("{id}/pdf")]
        [Authorize]
        [Produces("application/pdf")]
        public async Task<IActionResult> GerarPdfIngresso(int id)
        {
            var emailUsuario = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(emailUsuario))
            {
                _logger.LogWarning("Tentativa de gerar PDF sem email na claim do token.");
                return Unauthorized(new { Mensagem = "Informação do usuário não encontrada no token." });
            }

            var resultado = await _ingressoService.GerarPdfIngressoAsync(id, emailUsuario);

            if (!resultado.Sucesso)
            {
                if (resultado.Mensagem.Contains("não encontrado"))
                    return NotFound(new { resultado.Mensagem });

                return BadRequest(new { resultado.Mensagem });
            }

            var pdfBytes = resultado.Dados;
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                _logger.LogError("PDF gerado está vazio ou nulo para ingresso ID: {Id}", id);
                return StatusCode(500, new { Mensagem = "Erro ao gerar o PDF do ingresso." });
            }

            return File(pdfBytes, "application/pdf", $"ingresso_{id}.pdf");
        }
    }
}