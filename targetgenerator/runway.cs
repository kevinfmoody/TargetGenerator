using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Runway
    {
        public string identifier { get; set; }
        public Position position { get; set; }
        public double course { get; set; }
        public double elevation { get; set; }
        public bool ils { get; set; }

        public Runway(string identifier, Position position, double course, double elevation, bool ils = false)
        {
            this.identifier = identifier;
            this.position = position;
            this.course = course;
            this.elevation = elevation;
            this.ils = ils;
        }
    }
}
