using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Transferencias.Controllers
{
    [ApiController]
    [Route("transferencias")]
    public class TransferenciasController : ControllerBase
    {
        private readonly ILogger<TransferenciasController> _logger;

        public TransferenciasController(ILogger<TransferenciasController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTransferencias")]
        public ActionResult<IEnumerable<Transferencia>> Get()
        {
            return new TransferenciaAdapter().GetTransferencias();
        }

        [HttpGet("{id}")]
        public ActionResult<Transferencia> GetById(int id)
        {
            return new TransferenciaAdapter().GetTransferenciaById(id);
        }

        [HttpPost(Name = "PostTransferencia")]
        public ActionResult Post(Transferencia transferencia)
        {
            TransferenciaAdapter adapter = new TransferenciaAdapter();
            int newId;
            if (adapter.InsertTransferencia(transferencia.TimeOrigemId, transferencia.TimeDestinoId, 
                transferencia.Data ?? DateTime.Now, transferencia.Valor ?? 0, transferencia.JogadorId, out newId) > 0)
            {
                Transferencia novaTransferencia = adapter.GetTransferenciaById(newId);
                return CreatedAtAction(nameof(GetById), new { id = transferencia.Id }, transferencia);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Transferencia transferencia)
        {
            if (transferencia == null || transferencia.Id != id)
                return BadRequest();

            if (new TransferenciaAdapter().UpdateTransferencia(id, transferencia.TimeOrigemId, transferencia.TimeDestinoId,
                transferencia.Data, transferencia.Valor, transferencia.JogadorId) > 0)
                return Ok();

            return StatusCode(500);
        }
    }
}