using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Controllers
{
    [ApiController]
    [Route("eventos")]
    public class EventosController : ControllerBase
    {
        private readonly ILogger<EventosController> _logger;

        public EventosController(ILogger<EventosController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetEventos")]
        public ActionResult<IEnumerable<Evento>> Get()
        {
            return new EventoAdapter().GetEventos();
        }

        [HttpGet("{id}")]
        public ActionResult<Evento> GetById(int id)
        {
            return new EventoAdapter().GetEventoById(id);
        }

        [HttpPost(Name = "PostEvento")]
        public ActionResult Post(Evento evento)
        {
            EventoAdapter adapter = new EventoAdapter();
            int newId;
            if (adapter.InsertEvento(evento.Tipo, evento.Valor, evento.DataHora, evento.TimeId, evento.JogadorId, evento.PartidaId, evento.TorneioId, out newId) > 0)
            {
                Evento novoEvento = adapter.GetEventoById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novoEvento.Id }, novoEvento);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Evento evento)
        {
            if (evento == null || evento.Id != id)
                return BadRequest();

            if (new EventoAdapter().UpdateEvento(id, evento.Tipo, evento.Valor, evento.DataHora, 
                evento.TimeId, evento.JogadorId, evento.PartidaId, evento.TorneioId) > 0)
                return Ok();

            return StatusCode(500);
        }
    }
}