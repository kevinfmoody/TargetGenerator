using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Stream
    {   
        public Position position { get; set; }
        public double airspeed { get; set; }
        public double altitude { get; set; }
        public double heading { get; set; }

        public Path path { get; set; }

        public Stream(Position position, double airspeed, double altitude, double heading)
        {
            this.position = position;
            this.airspeed = airspeed;
            this.altitude = altitude;
            this.heading = heading;

            this.path = null;
        }

        public Stream(Path path) : this(path.legWaypoint().position, path.nextAirspeed(), path.nextAltitude(), (path.legCourse() - path.legWaypoint().position.magneticDeclination(path.nextAltitude()) + 360) % 360)
        {
            this.path = path;
        }
    }
}
