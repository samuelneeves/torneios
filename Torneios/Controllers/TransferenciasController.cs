using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Transferencias.Controllers
{
    [ApiController]
    [Route("transferencias")]
    public class TransferenciasController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions opcoesCache = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<TransferenciasController> _logger;

        public TransferenciasController(ILogger<TransferenciasController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetTransferencias")]
        public ActionResult<IEnumerable<Transferencia>> Get()
        {
            if (!_memoryCache.TryGetValue("Transferencias", out List<Transferencia> transferencias) || transferencias.Count == 0)
            {
                transferencias = new TransferenciaAdapter().GetTransferencias();
                _memoryCache.Set("Transferencias", transferencias, opcoesCache);
            }

            return transferencias;
        }

        [HttpGet("{id}")]
        public ActionResult<Transferencia> GetById(int id)
        {
            Transferencia? transferencia = null;
            if (_memoryCache.TryGetValue("Transferencias", out List<Transferencia> transferencias))
            {
                transferencia = transferencias.FirstOrDefault(j => j.Id == id);
            }

            return transferencia ?? new TransferenciaAdapter().GetTransferenciaById(id);
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
                if (_memoryCache.TryGetValue("Transferencias", out List<Transferencia> transferencias))
                {
                    if (transferencias.Count > 0)
                    {
                        transferencias.Add(novaTransferencia);
                        _memoryCache.Set("Transferencias", transferencias, opcoesCache);
                    }
                }
                return CreatedAtAction(nameof(GetById), new { id = transferencia.Id }, transferencia);
            }

            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Transferencia transferencia)
        {
            if (transferencia == null || transferencia.Id != id)
                return BadRequest();

            int insert = new TransferenciaAdapter().UpdateTransferencia(id, transferencia.TimeOrigemId, transferencia.TimeDestinoId,
                transferencia.Data, transferencia.Valor, transferencia.JogadorId);
            if (insert == -1)
            {
                return NotFound();
            }
            else if (insert > 0)
            {
                if (_memoryCache.TryGetValue("Transferencias", out List<Transferencia> transferencias))
                {
                    if (transferencias.Count > 0)
                    {
                        transferencias = transferencias.Where(j => j.Id != transferencia.Id).ToList();
                        transferencias.Add(transferencia);
                        _memoryCache.Set("Transferencias", transferencias, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }
        [HttpPatch("{id}")]
        public ActionResult Patch(int id, int? timeOrigemId, int? timeDestinoId, DateTime? data, double? valor, int? jogadorId)
        {
            TransferenciaAdapter adapter = new TransferenciaAdapter();
            if (adapter.UpdateTransferencia(id, timeOrigemId, timeDestinoId, data, valor, jogadorId) > 0)
            {
                if (_memoryCache.TryGetValue("Transferencias", out List<Transferencia> transferencias))
                {
                    if (transferencias.Count > 0)
                    {
                        transferencias = transferencias.Where(j => j.Id != id).ToList();
                        Transferencia transferencia = adapter.GetTransferenciaById(id);
                        transferencias.Add(transferencia);
                        _memoryCache.Set("Transferencias", transferencias, opcoesCache);
                    }
                }
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (new TransferenciaAdapter().DeleteTransferencia(id) > 0)
            {
                if (_memoryCache.TryGetValue("Transferencias", out List<Transferencia> transferencias))
                {
                    if (transferencias.Count > 0)
                    {
                        transferencias = transferencias.Where(j => j.Id != id).ToList();
                        _memoryCache.Set("Transferencias", transferencias, opcoesCache);
                    }
                }
                return Ok();
            }

            return NotFound();
        }
    }
}