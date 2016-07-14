using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class RadialControl : Control
    {
        virtual public int direction { get; set; }

        public RadialControl(double value, double target, double delta) : base(value, target, delta) { }

        public override void update(double milliseconds)
        {
            double remaining = (this.target - this.value + 360) % 360;
            int sign = this.direction;
            if (sign == 0)
            {
                if (Math.Abs(remaining - 360) < remaining)
                    remaining -= 360;
                sign = remaining >= 0 ? 1 : -1;
            }
            double delta = sign * this.delta * milliseconds;
            this.value = Math.Abs(delta) < Math.Abs(remaining) ? (this.value + delta + 360) % 360 : this.target;
        }
    }
}
