using Microsoft.AspNetCore.Mvc;
using GestaoEventos.API.DTO;
using GestaoEventos.API.Services.Interfaces;

namespace GestaoEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly ILogger<EventosController> _logger;

        public EventosController(IEventoService eventoService, ILogger<EventosController> logger)
        {
            _eventoService = eventoService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<EventoDto>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<EventoDto>>>> GetEventos()
        {
            _logger.LogInformation("Buscando todos os eventos");

            var resultado = await _eventoService.GetEventosAsync();

            if (resultado.Sucesso)
                return Ok(resultado);

            return StatusCode(500, resultado);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto<EventoDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<EventoDto>>> GetEvento(int id)
        {
            _logger.LogInformation("Buscando evento com ID: {Id}", id);

            var resultado = await _eventoService.GetEventoPorIdAsync(id);

            if (resultado.Sucesso)
                return Ok(resultado);

            if (resultado.Mensagem.Contains("não encontrado"))
                return NotFound(resultado);

            return StatusCode(500, resultado);
        }

        [HttpGet("ativos")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<EventoDto>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<EventoDto>>>> GetEventosAtivos()
        {
            _logger.LogInformation("Buscando eventos ativos");

            var resultado = await _eventoService.GetEventosAtivosAsync();

            if (resultado.Sucesso)
                return Ok(resultado);

            return StatusCode(500, resultado);
        }

        [HttpGet("disponiveis")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<EventoDto>>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<IEnumerable<EventoDto>>>> GetEventosDisponiveis()
        {
            _logger.LogInformation("Buscando eventos com ingressos disponíveis");

            var resultado = await _eventoService.GetEventosComIngressosDisponiveisAsync();

            if (resultado.Sucesso)
                return Ok(resultado);

            return StatusCode(500, resultado);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseDto<EventoDto>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<EventoDto>>> CriarEvento([FromBody] CriarEventoDto criarEventoDto)
        {
            _logger.LogInformation("Criando novo evento: {Nome}", criarEventoDto.Nome);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<EventoDto>
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos",
                    Erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var resultado = await _eventoService.CriarEventoAsync(criarEventoDto);

            if (resultado.Sucesso)
                return CreatedAtAction(nameof(GetEvento), new { id = resultado.Dados!.Id }, resultado);

            if (resultado.Erros.Any())
                return BadRequest(resultado);

            return StatusCode(500, resultado);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseDto<EventoDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<EventoDto>>> AtualizarEvento(int id, [FromBody] AtualizarEventoDto atualizarEventoDto)
        {
            _logger.LogInformation("Atualizando evento ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<EventoDto>
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos",
                    Erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var resultado = await _eventoService.AtualizarEventoAsync(id, atualizarEventoDto);

            if (resultado.Sucesso)
                return Ok(resultado);

            if (resultado.Mensagem.Contains("não encontrado"))
                return NotFound(resultado);

            if (resultado.Erros.Any())
                return BadRequest(resultado);

            return StatusCode(500, resultado);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseDto<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ResponseDto<bool>>> DeletarEvento(int id)
        {
            _logger.LogInformation("Deletando evento ID: {Id}", id);

            var resultado = await _eventoService.DeletarEventoAsync(id);

            if (resultado.Sucesso)
                return Ok(resultado);

            if (resultado.Mensagem.Contains("não encontrado"))
                return NotFound(resultado);

            if (resultado.Erros.Any())
                return BadRequest(resultado);

            return StatusCode(500, resultado);
        }
    }
}