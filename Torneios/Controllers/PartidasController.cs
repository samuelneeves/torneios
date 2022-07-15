using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Partidas.Controllers
{
    [ApiController]
    [Route("partidas")]
    public class PartidasController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<PartidasController> _logger;

        public PartidasController(ILogger<PartidasController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetPartidas")]
        public ActionResult<IEnumerable<Partida>> Get()
        {
            if (!_memoryCache.TryGetValue("Partidas", out List<Partida> partidas) || partidas.Count == 0)
            {
                partidas = new PartidaAdapter().GetPartidas();
                _memoryCache.Set("Partidas", partidas, opcoesCache);
            }

            return partidas;
        }

        [HttpGet("{id}")]
        public ActionResult<Partida> GetById(int id)
        {
            Partida? partida = null;
            if (_memoryCache.TryGetValue("Partidas", out List<Partida> partidas))
            {
                partida = partidas.FirstOrDefault(j => j.Id == id);
            }

            return partida ?? new PartidaAdapter().GetPartidaById(id);
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
                if (_memoryCache.TryGetValue("Partidas", out List<Partida> partidas))
                {
                    if (partidas.Count > 0)
                    {
                        partidas.Add(novaPartida);
                        _memoryCache.Set("Partidas", partidas, opcoesCache);
                    }
                }
                return CreatedAtAction(nameof(GetById), new { id = novaPartida.Id }, novaPartida);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Partida partida)
        {
            if (partida == null || partida.Id != id)
                return BadRequest();

            int insert = new PartidaAdapter().UpdatePartida(id, partida.Time1Id, partida.Time2Id,
                partida.GolsTime1, partida.GolsTime2, partida.TorneioId);

            if (insert == -1)
            {
                return NotFound();
            }
            else if (insert > 0)
            {
                if (_memoryCache.TryGetValue("Partidas", out List<Partida> partidas))
                {
                    if (partidas.Count > 0)
                    {
                        partidas = partidas.Where(j => j.Id != partida.Id).ToList();
                        partidas.Add(partida);
                        _memoryCache.Set("Partidas", partidas, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }
        [HttpPatch("{id}")]
        public ActionResult Patch(int id, int? time1Id, int? time2Id, int? golsTime1, int? golsTime2, int? torneioId)
        {
            PartidaAdapter adapter = new PartidaAdapter();
            if (adapter.UpdatePartida(id, time1Id, time2Id, golsTime1, golsTime2, torneioId) > 0)
            {
                if (_memoryCache.TryGetValue("Partidas", out List<Partida> partidaes))
                {
                    if (partidaes.Count > 0)
                    {
                        partidaes = partidaes.Where(j => j.Id != id).ToList();
                        Partida partida = adapter.GetPartidaById(id);
                        partidaes.Add(partida);
                        _memoryCache.Set("Partidas", partidaes, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (new PartidaAdapter().DeletePartida(id) > 0)
            {
                if (_memoryCache.TryGetValue("Partidas", out List<Partida> partidaes))
                {
                    if (partidaes.Count > 0)
                    {
                        partidaes = partidaes.Where(j => j.Id != id).ToList();
                        _memoryCache.Set("Partidas", partidaes, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }
    }
}