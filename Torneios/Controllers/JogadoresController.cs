using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Jogadores.Controllers
{
    [ApiController]
    [Route("jogadores")]
    public class JogadoresController : ControllerBase
    {
        private readonly ILogger<JogadoresController> _logger;

        public JogadoresController(ILogger<JogadoresController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetJogadores")]
        public ActionResult<IEnumerable<Jogador>> Get()
        {
            return new JogadorAdapter().GetJogadores();
        }

        [HttpGet("{id}")]
        public ActionResult<Jogador> GetById(int id)
        {
            return new JogadorAdapter().GetJogadorById(id);
        }

        [HttpPost(Name = "PostJogador")]
        public ActionResult Post(Jogador jogador)
        {
            JogadorAdapter adapter = new JogadorAdapter();
            int newId;
            if (adapter.InsertJogador(jogador.Nome, jogador.DataNascimento, jogador.Pais, jogador.TimeId, out newId) > 0)
            {
                Jogador novoJogador = adapter.GetJogadorById(newId);
                return CreatedAtAction(nameof(GetById), new { id = novoJogador.Id }, novoJogador);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Jogador jogador)
        {
            if (jogador == null || jogador.Id != id)
                return BadRequest();

            if (new JogadorAdapter().UpdateJogador(id, jogador.Nome, jogador.DataNascimento, jogador.Pais, jogador.TimeId) > 0)
                return Ok();

            return StatusCode(500);
        }
    }
}