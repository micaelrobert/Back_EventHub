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

        // GET: api/ingressos
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

        // GET: api/ingressos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto<IngressoDto>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 404)] // Adicionado para clareza
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IngressoDto>>> GetIngresso(int id)
        {
            _logger.LogInformation("Buscando ingresso com ID: {Id}", id);
            var resultado = await _ingressoService.GetIngressoPorIdAsync(id);
            if (resultado.Sucesso && resultado.Dados != null) // Adicionado verificação de Dados != null
                return Ok(resultado);
            if (!resultado.Sucesso && resultado.Mensagem.Contains("não encontrado")) // Checagem mais robusta
                return NotFound(resultado);
            return StatusCode(500, resultado);
        }

        // GET: api/ingressos/evento/{eventoId}
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

        // GET: api/ingressos/email/{email}
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 404)] // Adicionado
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetIngressosPorEmail(string email)
        {
            _logger.LogInformation("Buscando ingressos do email: {Email}", email);
            // Supondo que o nome do método no seu IIngressoService seja GetIngressosPorEmailAsync
            var resultado = await _ingressoService.GetIngressosPorEmailAsync(email);
            if (resultado.Sucesso && resultado.Dados != null && resultado.Dados.Any())
                return Ok(resultado);
            if (resultado.Sucesso && (resultado.Dados == null || !resultado.Dados.Any()))
                return Ok(new ResponseDto<IEnumerable<IngressoDto>> { Sucesso = true, Dados = new List<IngressoDto>(), Mensagem = "Nenhum ingresso encontrado para este email." });

            return StatusCode(500, resultado);
        }

        // ========================================================
        // <<< NOVO ENDPOINT PARA USUÁRIO LOGADO >>>
        // ========================================================
        [HttpGet("meus")]
        [Authorize] // Só usuários logados podem acessar
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IngressoDto>>), 200)]
        [ProducesResponseType(typeof(ResponseDto<object>), 401)] // Não autorizado se token inválido/ausente
        public async Task<ActionResult<ResponseDto<IEnumerable<IngressoDto>>>> GetMeusIngressos()
        {
            var emailUsuarioLogado = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(emailUsuarioLogado))
            {
                // Isso não deveria acontecer se [Authorize] estiver funcionando e o token tiver a claim Email
                _logger.LogWarning("Tentativa de acesso a /meus sem email na claim do token.");
                return Unauthorized(new ResponseDto<object> { Sucesso = false, Mensagem = "Informação do usuário não encontrada no token." });
            }

            _logger.LogInformation("Buscando ingressos para o usuário logado: {Email}", emailUsuarioLogado);

            var resultado = await _ingressoService.GetIngressosPorEmailAsync(emailUsuarioLogado);

            // Retorna sucesso mesmo se não houver ingressos, mas com lista vazia e mensagem apropriada
            if (resultado.Sucesso)
            {
                if (resultado.Dados == null || !resultado.Dados.Any())
                {
                    return Ok(new ResponseDto<IEnumerable<IngressoDto>> { Sucesso = true, Dados = new List<IngressoDto>(), Mensagem = "Você ainda não possui ingressos." });
                }
                return Ok(resultado);
            }

            // Se _ingressoService.GetIngressosPorEmailAsync() retornar Sucesso = false
            return StatusCode(500, resultado);
        }
        // ========================================================

        // POST: api/ingressos/comprar
        [HttpPost("comprar")]
        [Authorize] // <<< ADICIONAR AUTORIZAÇÃO AQUI TAMBÉM
        [ProducesResponseType(typeof(ResponseDto<IngressoDto>), 201)]
        [ProducesResponseType(typeof(ResponseDto<object>), 400)]
        [ProducesResponseType(typeof(ResponseDto<object>), 500)]
        public async Task<ActionResult<ResponseDto<IngressoDto>>> ComprarIngresso([FromBody] ComprarIngressoDto comprarIngressoDto)
        {
            _logger.LogInformation("Comprando ingresso para evento ID: {EventoId} pelo usuário {Email}",
                comprarIngressoDto.EventoId, User.FindFirstValue(ClaimTypes.Email));

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<IngressoDto> // Corrigido o tipo genérico aqui
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos",
                    Erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            // Opcional: Você pode querer pegar o EmailComprador e NomeComprador do usuário logado
            // em vez de confiar cegamente no que vem do DTO, se o usuário estiver logado.
            // Ex: comprarIngressoDto.EmailComprador = User.FindFirstValue(ClaimTypes.Email);
            //     comprarIngressoDto.NomeComprador = User.FindFirstValue(ClaimTypes.Name); // Se você adicionou Name à claim

            var resultado = await _ingressoService.ComprarIngressoAsync(comprarIngressoDto);

            if (resultado.Sucesso && resultado.Dados != null) // Adicionado Dados != null
                return CreatedAtAction(nameof(GetIngresso), new { id = resultado.Dados.Id }, resultado);

            // Se o serviço de compra retornou sucesso false por uma regra de negócio (ex: evento lotado)
            if (!resultado.Sucesso && resultado.Erros != null && resultado.Erros.Any())
                return BadRequest(resultado);
            if (!resultado.Sucesso)
                return BadRequest(new ResponseDto<IngressoDto> { Sucesso = false, Mensagem = resultado.Mensagem });


            return StatusCode(500, resultado);
        }

        // POST: api/ingressos/{id}/devolver
        [HttpPost("{id}/devolver")]
        [Authorize] // <<< ADICIONAR AUTORIZAÇÃO AQUI TAMBÉM
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

            // Adicional: Verificar se o ingresso pertence ao usuário logado antes de devolver
            // Isso exigiria buscar o ingresso e comparar o EmailComprador com User.FindFirstValue(ClaimTypes.Email)
            // ou ter uma lógica no serviço que faça isso.

            var resultado = await _ingressoService.DevolverIngressoAsync(id, devolverIngressoDto);

            if (resultado.Sucesso)
                return Ok(resultado);
            if (!resultado.Sucesso && resultado.Mensagem.Contains("não encontrado"))
                return NotFound(resultado);
            if (!resultado.Sucesso && resultado.Erros != null && resultado.Erros.Any())
                return BadRequest(resultado);

            return StatusCode(500, resultado);
        }

        // GET: api/ingressos/ativos
        [HttpGet("ativos")]
        // [Authorize(Roles = "Admin")] // Exemplo: Se apenas admin puder ver todos os ingressos ativos
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
    
