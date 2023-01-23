using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleBallBot
{
    public class PrestigeStat
    {
        public List<long> Times { get; set; }
        public long TotalTime { get; set; }
        public int Prestige { get; set; }
        public long AverageTime { get; set; }
        public long MaxTime { get; set; }
        public long MinTime { get; set; }
    }
}
