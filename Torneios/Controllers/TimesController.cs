using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Times.Controllers
{
    [ApiController]
    [Route("times")]
    public class TimesController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<TimesController> _logger;

        public TimesController(ILogger<TimesController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetTimes")]
        public ActionResult<IEnumerable<Time>> Get()
        {
            if (!_memoryCache.TryGetValue("Times", out List<Time> times) || times.Count == 0)
            {
                times = new TimeAdapter().GetTimes();
                _memoryCache.Set("Times", times, opcoesCache);
            }

            return times;
        }

        [HttpGet("{id}")]
        public ActionResult<Time> GetById(int id)
        {
            Time? time = null;
            if (_memoryCache.TryGetValue("Times", out List<Time> times))
            {
                time = times.FirstOrDefault(j => j.Id == id);
            }

            return time ?? new TimeAdapter().GetTimeById(id);
        }

        [HttpPost(Name = "PostTime")]
        public ActionResult Post(Time time)
        {
            TimeAdapter adapter = new TimeAdapter();
            int newId;
            if (adapter.InsertTime(time.Nome, time.Localidade, out newId) > 0)
            {
                Time novoTime = adapter.GetTimeById(newId);
                if (_memoryCache.TryGetValue("Times", out List<Time> times))
                {
                    if (times.Count > 0)
                    {
                        times.Add(novoTime);
                        _memoryCache.Set("Times", times, opcoesCache);
                    }
                }
                return CreatedAtAction(nameof(GetById), new { id = novoTime.Id }, novoTime);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Time time)
        {
            if (time == null || time.Id != id)
                return BadRequest();

            int insert = new TimeAdapter().UpdateTime(id, time.Nome, time.Localidade);
            if (insert == -1)
            {
                return NotFound();
            }
            else if (insert > 0)
            {
                if (_memoryCache.TryGetValue("Times", out List<Time> times))
                {
                    if (times.Count > 0)
                    {
                        times = times.Where(j => j.Id != time.Id).ToList();
                        times.Add(time);
                        _memoryCache.Set("Times", times, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }
        [HttpPatch("{id}")]
        public ActionResult Patch(int id, string? nome, string? localidade)
        {
            TimeAdapter adapter = new TimeAdapter();
            if (adapter.UpdateTime(id, nome, localidade, true) > 0)
            {
                if (_memoryCache.TryGetValue("Times", out List<Time> times))
                {
                    if (times.Count > 0)
                    {
                        times = times.Where(j => j.Id != id).ToList();
                        Time time = adapter.GetTimeById(id);
                        times.Add(time);
                        _memoryCache.Set("Times", times, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (new TimeAdapter().DeleteTime(id) > 0)
            {
                if (_memoryCache.TryGetValue("Times", out List<Time> times))
                {
                    if (times.Count > 0)
                    {
                        times = times.Where(j => j.Id != id).ToList();
                        _memoryCache.Set("Times", times, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }
    }
}