using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Eventos.Controllers
{
    [ApiController]
    [Route("eventos")]
    public class EventosController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<EventosController> _logger;

        public EventosController(ILogger<EventosController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetEventos")]
        public ActionResult<IEnumerable<Evento>> Get()
        {
            if (!_memoryCache.TryGetValue("Eventoes", out List<Evento> eventos) || eventos.Count == 0)
            {
                eventos = new EventoAdapter().GetEventos();
                _memoryCache.Set("Eventoes", eventos, opcoesCache);
            }

            return eventos;
            return new EventoAdapter().GetEventos();
        }

        [HttpGet("{id}")]
        public ActionResult<Evento> GetById(int id)
        {
            Evento? evento = null;
            if (_memoryCache.TryGetValue("Eventoes", out List<Evento> eventos))
            {
                evento = eventos.FirstOrDefault(j => j.Id == id);
            }

            return evento ?? new EventoAdapter().GetEventoById(id);
        }

        [HttpPost(Name = "PostEvento")]
        public ActionResult Post(Evento evento)
        {
            EventoAdapter adapter = new EventoAdapter();
            int newId;
            if (adapter.InsertEvento(evento.Tipo, evento.Valor, evento.DataHora, evento.TimeId, evento.JogadorId, evento.PartidaId, evento.TorneioId, out newId) > 0)
            {
                Evento novoEvento = adapter.GetEventoById(newId); 
                if (_memoryCache.TryGetValue("Eventoes", out List<Evento> eventos))
                {
                    if (eventos.Count > 0)
                    {
                        eventos.Add(novoEvento);
                        _memoryCache.Set("Eventoes", eventos, opcoesCache);
                    }
                }
                return CreatedAtAction(nameof(GetById), new { id = novoEvento.Id }, novoEvento);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Evento evento)
        {
            if (evento == null || evento.Id != id)
                return BadRequest();

            int insert = new EventoAdapter().UpdateEvento(id, evento.Tipo, evento.Valor, evento.DataHora,
                evento.TimeId, evento.JogadorId, evento.PartidaId, evento.TorneioId);

            if (insert == -1)
            {
                return NotFound();
            }
            else if (insert > 0)
            {
                if (_memoryCache.TryGetValue("Eventoes", out List<Evento> eventos))
                {
                    if (eventos.Count > 0)
                    {
                        eventos = eventos.Where(j => j.Id != evento.Id).ToList();
                        eventos.Add(evento);
                        _memoryCache.Set("Eventoes", eventos, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, string? tipo, string? valor, DateTime? dataHora, int? timeId, int? jogadorId, int? partidaId, int? torneioId)
        {
            EventoAdapter adapter = new EventoAdapter();
            if (adapter.UpdateEvento(id, tipo, valor, dataHora, timeId, jogadorId, partidaId, torneioId, true) > 0)
            {
                if (_memoryCache.TryGetValue("Eventos", out List<Evento> eventos))
                {
                    if (eventos.Count > 0)
                    {
                        eventos = eventos.Where(j => j.Id != id).ToList();
                        Evento evento = adapter.GetEventoById(id);
                        eventos.Add(evento);
                        _memoryCache.Set("Eventos", eventos, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (new EventoAdapter().DeleteEvento(id) > 0)
            {
                if (_memoryCache.TryGetValue("Eventos", out List<Evento> eventos))
                {
                    if (eventos.Count > 0)
                    {
                        eventos = eventos.Where(j => j.Id != id).ToList();
                        _memoryCache.Set("Eventos", eventos, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }
    }
}