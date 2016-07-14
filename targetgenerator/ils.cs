using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class ILS : Path
    {
        public ILS(Runway runway) : base()
        {
            double maxLocalizerAltitude = runway.elevation + Math.Sin(Position.DegreesToRadians(3)) * 60 / Position.KM_TO_NM * Position.FT_IN_KM;
            Position maxLocalizerPosition = runway.position.destinationPoint(runway.course + 180, 60);
            Position initialDecelPosition = runway.position.destinationPoint(runway.course + 180, 15);
            Position midDecelPosition = runway.position.destinationPoint(runway.course + 180, 10);
            Position finalDecelPosition = runway.position.destinationPoint(runway.course + 180, 5);
            this.waypoints.Add(new Waypoint(maxLocalizerPosition, new Restriction(maxLocalizerAltitude, 0)));
            this.waypoints.Add(new Waypoint(initialDecelPosition, null, new Restriction(170, 60)));
            this.waypoints.Add(new Waypoint(midDecelPosition, null, new Restriction(170, 40)));
            this.waypoints.Add(new Waypoint(finalDecelPosition, null, new Restriction(140)));
            this.waypoints.Add(new Waypoint(runway.position, new Restriction(runway.elevation), new Restriction(140)));
        }
    }
}
