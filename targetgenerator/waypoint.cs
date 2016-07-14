using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Waypoint
    {
        public string identifier { get; set; }
        public Position position { get; set; }
        public double altitude { get; set; }
        public double airspeed { get; set; }

        public Waypoint(string identifier, Position position, double altitude = 0, double airspeed = 0)
        {
            this.identifier = identifier;
            this.position = position;
            this.altitude = altitude;
            this.airspeed = airspeed;
        }
    }
}
