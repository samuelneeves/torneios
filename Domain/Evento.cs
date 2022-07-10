using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Evento
    {
        public int? Id { get; set; }
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public DateTime? DataHora { get; set; }
        public int? TimeId { get; set; }
        public int? JogadorId { get; set; }
        public int? PartidaId { get; set; }
        public int? TorneioId { get; set; }
    }
}
