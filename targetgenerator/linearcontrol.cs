using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class LinearControl : Control
    {
        public LinearControl(double value, double target, double delta) : base(value, target, delta) { }

        override public void update(double milliseconds)
        {
            double remaining = this.target - this.value;
            int sign = remaining >= 0 ? 1 : -1;
            double delta = sign * this.delta * milliseconds;
            this.value = Math.Abs(delta) < Math.Abs(remaining) ? this.value + delta : this.target;
        }
    }
}
