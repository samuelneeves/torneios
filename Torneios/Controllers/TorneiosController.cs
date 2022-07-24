using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RabbitMQ;

namespace Torneios.Controllers
{
    [ApiController]
    [Route("torneios")]
    public class TorneiosController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<TorneiosController> _logger;

        public TorneiosController(ILogger<TorneiosController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetTorneios")]
        public ActionResult<IEnumerable<Torneio>> Get()
        {
            if (!_memoryCache.TryGetValue("Torneios", out List<Torneio> torneios) || torneios.Count == 0)
            {
                torneios = new TorneioAdapter().GetTorneios();
                _memoryCache.Set("Torneios", torneios, opcoesCache);
            }
            return torneios;
        }

        [HttpGet("{id}")]
        public ActionResult<Torneio> GetById(int id)
        {
            Torneio? torneio = null;
            if (_memoryCache.TryGetValue("Torneios", out List<Torneio> torneios))
            {
                torneio = torneios.FirstOrDefault(j => j.Id == id);
            }

            return torneio ?? new TorneioAdapter().GetTorneioById(id);
        }

        [HttpPost(Name = "PostTorneio")]
        public ActionResult Post([FromBody] string nome)
        {
            TorneioAdapter adapter = new TorneioAdapter();
            int newId;
            if (adapter.InsertTorneio(nome, out newId) > 0)
            {
                Torneio novoTorneio = adapter.GetTorneioById(newId);
                if (_memoryCache.TryGetValue("Torneios", out List<Torneio> torneios))
                {
                    if (torneios.Count > 0)
                    {
                        torneios.Add(novoTorneio);
                        _memoryCache.Set("Torneios", torneios, opcoesCache);
                    }
                }
                return CreatedAtAction(nameof(GetById), new { id = novoTorneio.Id }, novoTorneio);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Torneio torneio)
        {
            if (torneio == null || torneio.Id != id)
                return BadRequest();

            int insert = new TorneioAdapter().UpdateTorneio(torneio);
            if (insert == -1)
            {
                return NotFound();
            }
            else if (insert > 0)
            {
                if (_memoryCache.TryGetValue("Torneios", out List<Torneio> torneios))
                {
                    if (torneios.Count > 0)
                    {
                        torneios = torneios.Where(j => j.Id != torneio.Id).ToList();
                        torneios.Add(torneio);
                        _memoryCache.Set("Torneios", torneios, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, string? nome)
        {
            TorneioAdapter adapter = new TorneioAdapter();
            if (adapter.UpdateTorneio(id, nome, true) > 0)
            {
                if (_memoryCache.TryGetValue("Torneios", out List<Torneio> torneios))
                {
                    if (torneios.Count > 0)
                    {
                        torneios = torneios.Where(j => j.Id != id).ToList();
                        Torneio torneio = adapter.GetTorneioById(id);
                        torneios.Add(torneio);
                        _memoryCache.Set("Torneios", torneios, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (new TorneioAdapter().DeleteTorneio(id) > 0)
            {
                if (_memoryCache.TryGetValue("Torneios", out List<Torneio> torneios))
                {
                    if (torneios.Count > 0)
                    {
                        torneios = torneios.Where(j => j.Id != id).ToList();
                        _memoryCache.Set("Torneios", torneios, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }

        [HttpPost("{id}/partidas/{partidaId}/eventos/{tipoEvento}")]
        public ActionResult PostEventoPartida(int id, int partidaId, string tipoEvento, [FromBody] string valor)
        {
            EventoAdapter adapter = new EventoAdapter();
            int newId;
            if (adapter.InsertEvento(tipoEvento, valor, DateTime.Now, null, null, partidaId, id, out newId) > 0)
            {
                Evento novoEvento = adapter.GetEventoById(newId);
                Producer.Send(tipoEvento, novoEvento);
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
                Producer.Send(tipoEvento, novoEvento);
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
                Producer.Send(tipoEvento, novoEvento);
                return CreatedAtAction(nameof(GetById), new { id = novoEvento.Id }, novoEvento);
            }

            return StatusCode(500);
        }
    }
}