using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleBallBot
{
    public class Ball
    {
        public string Name { get; set; }

        public Upgrades Balls { get; set; }

        public Upgrades Speed { get; set; }

        public Upgrades Power { get; set; }

        public int UpdateTimeout { get; set; }

    }
}
