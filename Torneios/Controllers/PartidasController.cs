using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Partidas.Controllers
{
    [ApiController]
    [Route("partidas")]
    public class PartidasController : ControllerBase
    {
        private readonly ILogger<PartidasController> _logger;

        public PartidasController(ILogger<PartidasController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPartidas")]
        public ActionResult<IEnumerable<Partida>> Get()
        {
            return new PartidaAdapter().GetPartidas();
        }

        [HttpGet("{id}")]
        public ActionResult<Partida> GetById(int id)
        {
            return new PartidaAdapter().GetPartidaById(id);
        }

        [HttpPost(Name = "PostPartida")]
        public ActionResult Post(Partida partida)
        {
            PartidaAdapter adapter = new PartidaAdapter();
            int newId;
            if (adapter.InsertPartida(partida.Time1Id, partida.Time2Id, 
                partida.GolsTime1, partida.GolsTime2, partida.TorneioId, out newId) > 0)
            {
                Partida novaPartida = adapter.GetPartidaById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novaPartida.Id }, novaPartida);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Partida partida)
        {
            if (partida == null || partida.Id != id)
                return BadRequest();

            if (new PartidaAdapter().UpdatePartida(id, partida.Time1Id, partida.Time2Id, 
                partida.GolsTime1, partida.GolsTime2, partida.TorneioId) > 0)
                return Ok();

            return StatusCode(500);
        }
    }
}