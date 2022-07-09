using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Partida
    {
        public int Id { get; set; }
        public int Time1Id { get; set; }
        public int Time2Id { get; set; }
        public double GolsTime1 { get; set; }  
        public double GolsTime2 { get; set; }
        public int TorneioId { get; set; }
    }
}
