using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Torneios.Controllers
{
    [ApiController]
    [Route("torneios")]
    public class TorneiosController : ControllerBase
    {
        private readonly ILogger<TorneiosController> _logger;

        public TorneiosController(ILogger<TorneiosController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTorneios")]
        public ActionResult<IEnumerable<Torneio>> Get()
        {
            return new TorneioAdapter().GetTorneios();
        }

        [HttpGet("{id}")]
        public ActionResult<Torneio> GetById(int id)
        {
            return new TorneioAdapter().GetTorneioById(id);
        }

        [HttpPost(Name = "PostTorneio")]
        public ActionResult Post([FromBody] string nome)
        {
            TorneioAdapter adapter = new TorneioAdapter();
            int newId;
            if (adapter.InsertTorneio(nome, out newId) > 0)
            {
                Torneio novoTorneio = adapter.GetTorneioById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novoTorneio.Id }, novoTorneio);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Torneio torneio)
        {
            if (torneio == null || torneio.Id != id)
                return BadRequest();

            if (new TorneioAdapter().UpdateTorneio(torneio) > 0)
                return Ok();

            return StatusCode(500);
        }

        [HttpPost("{id}/partidas/{partidaId}/eventos/{tipoEvento}")]
        public ActionResult PostEventoPartida(int id, int partidaId, string tipoEvento, [FromBody] string valor)
        {
            EventoAdapter adapter = new EventoAdapter();
            int newId;
            if (adapter.InsertEvento(tipoEvento, valor, DateTime.Now, null, null, partidaId, id, out newId) > 0)
            {
                Evento novoEvento = adapter.GetEventoById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novoEvento.Id }, novoEvento);
            }

            return StatusCode(500);
        }

        [HttpPost("{id}/partidas/{partidaId}/times/{timeId}/eventos/{tipoEvento}")]
        public ActionResult PostEventoTime(int id, int partidaId, int timeId, string tipoEvento, [FromBody] string valor)
        {
            EventoAdapter adapter = new EventoAdapter();
            int newId;
            if (adapter.InsertEvento(tipoEvento, valor, DateTime.Now, timeId, null, partidaId, id, out newId) > 0)
            {
                Evento novoEvento = adapter.GetEventoById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novoEvento.Id }, novoEvento);
            }

            return StatusCode(500);
        }

        [HttpPost("{id}/partidas/{partidaId}/times/{timeId}/jogadores/{jogadorId}/eventos/{tipoEvento}")]
        public ActionResult PostEventoJogador(int id, int partidaId, int timeId, int jogadorId, string tipoEvento, [FromBody] string valor)
        {
            EventoAdapter adapter = new EventoAdapter();
            int newId;
            if (adapter.InsertEvento(tipoEvento, valor, DateTime.Now, timeId, jogadorId, partidaId, id, out newId) > 0)
            {
                Evento novoEvento = adapter.GetEventoById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novoEvento.Id }, novoEvento);
            }

            return StatusCode(500);
        }
    }
}