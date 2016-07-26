using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Rand
    {
        private static readonly Rand instance = new Rand();
        public static Rand Instance { get { return instance; } }

        private Random random;

        private Rand()
        {
            this.random = new Random();
        }

        public double getDouble()
        {
            return this.random.NextDouble();
        }
    }
}
