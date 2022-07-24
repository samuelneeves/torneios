using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RabbitMQ;

namespace TimeEmTorneios.Controllers
{
    [ApiController]
    [Route("timesEmTorneios")]
    public class TimesEmTorneiosController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<TimesEmTorneiosController> _logger;

        public TimesEmTorneiosController(ILogger<TimesEmTorneiosController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetTimeEmTorneios")]
        public ActionResult<IEnumerable<TimeEmTorneio>> Get()
        {
            if (!_memoryCache.TryGetValue("TimeEmTorneios", out List<TimeEmTorneio> timeEmTorneios) || timeEmTorneios.Count == 0)
            {
                timeEmTorneios = new TimeEmTorneioAdapter().GetTimesEmTorneios();
                _memoryCache.Set("TimeEmTorneios", timeEmTorneios, opcoesCache);
            }
            return timeEmTorneios;
        }

        [HttpGet("{timeId}")]
        public ActionResult<IEnumerable<TimeEmTorneio>> GetTimesEmTorneiosByTimeId(int timeId)
        {
            List<TimeEmTorneio>? timeEmTorneios = new List<TimeEmTorneio>();
            if (_memoryCache.TryGetValue("TimesEmTorneios", out List<TimeEmTorneio> timesEmTorneios))
            {
                timeEmTorneios.AddRange(timesEmTorneios.Where(t => t.TimeId == timeId));
            }

            return timeEmTorneios.Count > 0 ? timeEmTorneios : new TimeEmTorneioAdapter().GetTimesEmTorneioByTimeId(timeId);
        }

        [HttpGet("{torneioId}")]
        public ActionResult<IEnumerable<TimeEmTorneio>> GetTimesEmTorneiosByTorneioId(int torneioId)
        {
            List<TimeEmTorneio>? timeEmTorneios = new List<TimeEmTorneio>();
            if (_memoryCache.TryGetValue("TimesEmTorneios", out List<TimeEmTorneio> timesEmTorneios))
            {
                timeEmTorneios.AddRange(timesEmTorneios.Where(t => t.TorneioId == torneioId));
            }

            return timeEmTorneios.Count > 0 ? timeEmTorneios : new TimeEmTorneioAdapter().GetTimesEmTorneioByTorneioId(torneioId);
        }

        [HttpPost(Name = "PostTimeEmTorneio")]
        public ActionResult Post(TimeEmTorneio timeEmTorneio)
        {
            TimeEmTorneioAdapter adapter = new TimeEmTorneioAdapter();
            if (adapter.InsertTimeEmTorneio(timeEmTorneio.TorneioId, timeEmTorneio.TimeId) > 0)
            {
                TimeEmTorneio novoTimeEmTorneio = adapter.GetTimesEmTorneioByTimeIdETorneioId(timeEmTorneio.TimeId, timeEmTorneio.TorneioId);
                if (_memoryCache.TryGetValue("TimeEmTorneios", out List<TimeEmTorneio> timesEmTorneios))
                {
                    if (timesEmTorneios.Count > 0)
                    {
                        timesEmTorneios.Add(novoTimeEmTorneio);
                        _memoryCache.Set("TimeEmTorneios", timesEmTorneios, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpPatch()]
        public ActionResult MudarTimeDeTorneio(int timeId, int torneioId, int novoTorneioId)
        {
            TimeEmTorneioAdapter adapter = new TimeEmTorneioAdapter();
            if (adapter.MudarTimeDeTorneio(timeId, torneioId, novoTorneioId) > 0)
            {
                if (_memoryCache.TryGetValue("TimeEmTorneios", out List<TimeEmTorneio> timesEmTorneios))
                {
                    if (timesEmTorneios.Count > 0)
                    {
                        timesEmTorneios = timesEmTorneios.Where(j => j.TimeId != timeId && j.TorneioId != torneioId).ToList();
                        TimeEmTorneio timeEmTorneio = new TimeEmTorneio() { TimeId = timeId, TorneioId = novoTorneioId };
                        timesEmTorneios.Add(timeEmTorneio);
                        _memoryCache.Set("TimeEmTorneios", timesEmTorneios, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{torneioId}/{timeId}")]
        public ActionResult Delete(int torneioId, int timeId)
        {
            if (new TimeEmTorneioAdapter().RemoverTimeDeTorneio(torneioId, timeId) > 0)
            {
                if (_memoryCache.TryGetValue("TimeEmTorneios", out List<TimeEmTorneio> timeEmTorneios))
                {
                    if (timeEmTorneios.Count > 0)
                    {
                        timeEmTorneios = timeEmTorneios.Where(j => j.TorneioId != torneioId && j.TimeId == timeId).ToList();
                        _memoryCache.Set("TimeEmTorneios", timeEmTorneios, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }
    }
}