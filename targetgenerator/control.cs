using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    abstract class Control
    {
        virtual public double value { get; set; }
        virtual public double target { get; set; }
        virtual public double delta { get; set; }

        public bool assigned { get; set; }

        public Control(double value, double target, double delta, bool assigned = false)
        {
            this.value = value;
            this.target = target;
            this.delta = delta;

            this.assigned = assigned;
        }

        abstract public void update(double milliseconds);

        public string info()
        {
            int value = (int)this.value;
            int target = (int)this.target;
            if (value == target)
            {
                return String.Format("{0:D3}", value);
            }
            return String.Format("{0:D3} (-> {1:D3})", value, target);
        }
    }
}