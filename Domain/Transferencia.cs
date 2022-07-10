using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Transferencia
    {
        public int? Id { get; set; }
        public int? TimeOrigemId { get; set; }
        public int? TimeDestinoId { get; set; }
        public DateTime? Data { get; set; }
        public double? Valor { get; set; }
        public int JogadorId { get; set; }
    }
}
