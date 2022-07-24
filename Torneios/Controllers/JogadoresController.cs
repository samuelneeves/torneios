using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Jogadores.Controllers
{
    [ApiController]
    [Route("jogadores")]
    public class JogadoresController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<JogadoresController> _logger;
        public JogadoresController(ILogger<JogadoresController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetJogadores")]
        public ActionResult<IEnumerable<Jogador>> Get()
        {
            if (!_memoryCache.TryGetValue("Jogadores", out List<Jogador> jogadores) || jogadores.Count == 0)
            {
                jogadores = new JogadorAdapter().GetJogadores();
                _memoryCache.Set("Jogadores", jogadores, opcoesCache);
            }

            return jogadores;
        }

        [HttpGet("{id}")]
        public ActionResult<Jogador> GetById(int id)
        {
            Jogador? jogador = null;
            if (_memoryCache.TryGetValue("Jogadores", out List<Jogador> jogadores))
            {
                jogador = jogadores.FirstOrDefault(j => j.Id == id);
            }

            return jogador ?? new JogadorAdapter().GetJogadorById(id);
        }

        [HttpPost(Name = "PostJogador")]
        public ActionResult Post(Jogador jogador)
        {
            JogadorAdapter adapter = new JogadorAdapter();
            int newId;
            if (adapter.InsertJogador(jogador.Nome, jogador.Idade, jogador.Pais, jogador.TimeId, out newId) > 0)
            {
                Jogador novoJogador = adapter.GetJogadorById(newId);
                if (_memoryCache.TryGetValue("Jogadores", out List<Jogador> jogadores))
                {
                    if (jogadores.Count > 0)
                    {
                        jogadores.Add(novoJogador);
                        _memoryCache.Set("Jogadores", jogadores, opcoesCache);
                    }
                }
                return CreatedAtAction(nameof(GetById), new { id = novoJogador.Id }, novoJogador);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Jogador jogador)
        {
            if (jogador == null || jogador.Id != id)
                return BadRequest();
            int insert = new JogadorAdapter().UpdateJogador(id, jogador.Nome, jogador.Idade, jogador.Pais, jogador.TimeId);
            if (insert == -1)
            {
                return NotFound();
            }
            else if (insert > 0)
            {
                if (_memoryCache.TryGetValue("Jogadores", out List<Jogador> jogadores))
                {
                    if (jogadores.Count > 0)
                    {
                        jogadores = jogadores.Where(j => j.Id != jogador.Id).ToList();
                        jogadores.Add(jogador);
                        _memoryCache.Set("Jogadores", jogadores, opcoesCache);
                    }
                }
                return Ok();
            }            

            return StatusCode(500);
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, string? nome, int? idade, string? pais, int? timeId)
        {
            JogadorAdapter adapter = new JogadorAdapter();
            if (adapter.UpdateJogador(id, nome, idade, pais, timeId) > 0)
            {
                if (_memoryCache.TryGetValue("Jogadores", out List<Jogador> jogadores))
                {
                    if (jogadores.Count > 0)
                    {
                        jogadores = jogadores.Where(j => j.Id != id).ToList();
                        Jogador jogador = adapter.GetJogadorById(id);
                        jogadores.Add(jogador);
                        _memoryCache.Set("Jogadores", jogadores, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (new JogadorAdapter().DeleteJogador(id) > 0)
            {
                if (_memoryCache.TryGetValue("Jogadores", out List<Jogador> jogadores))
                {
                    if (jogadores.Count > 0)
                    {
                        jogadores = jogadores.Where(j => j.Id != id).ToList();
                        _memoryCache.Set("Jogadores", jogadores, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }
    }
}