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
            if (!_memoryCache.TryGetValue("Eventos", out List<Evento> eventos) || eventos.Count == 0)
            {
                eventos = new EventoAdapter().GetEventos();
                _memoryCache.Set("Eventos", eventos, opcoesCache);
            }

            return eventos;
        }

        [HttpGet("{id}")]
        public ActionResult<Evento> GetById(int id)
        {
            Evento? evento = null;
            if (_memoryCache.TryGetValue("Eventos", out List<Evento> eventos))
            {
                evento = eventos.FirstOrDefault(j => j.Id == id);
            }

            return evento ?? new EventoAdapter().GetEventoById(id);
        }
    }
}