using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Torneios.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TorneiosController : ControllerBase
    {
        private readonly ILogger<TorneiosController> _logger;

        public TorneiosController(ILogger<TorneiosController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTorneios")]
        public IEnumerable<Torneio> Get()
        {
            return new TorneioAdapter().GetTorneios();
        }
    }
}