using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Times.Controllers
{
    [ApiController]
    [Route("times")]
    public class TimesController : ControllerBase
    {
        private readonly ILogger<TimesController> _logger;

        public TimesController(ILogger<TimesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTimes")]
        public ActionResult<IEnumerable<Time>> Get()
        {
            return new TimeAdapter().GetTimes();
        }

        [HttpGet("{id}")]
        public ActionResult<Time> GetById(int id)
        {
            return new TimeAdapter().GetTimeById(id);
        }

        [HttpPost(Name = "PostTime")]
        public ActionResult Post([FromBody] string nome)
        {
            TimeAdapter adapter = new TimeAdapter();
            int newId;
            if (adapter.InsertTime(nome, out newId) > 0)
            {
                Time time = adapter.GetTimeById(newId);
                return CreatedAtAction(nameof(GetById), new { id = time.Id }, time);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Time time)
        {
            if (time == null || time.Id != id)
                return BadRequest();

            if (new TimeAdapter().UpdateTime(id, time.Nome) > 0)
                return Ok();

            return StatusCode(500);
        }
    }
}