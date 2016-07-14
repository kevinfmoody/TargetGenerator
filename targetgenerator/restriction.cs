using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Restriction
    {
        public double value { get; set; }
        public double block { get; set; }

        public Restriction(double value, double block = 0)
        {
            this.value = value;
            this.block = block;
        }
    }
}
