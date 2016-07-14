using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Data
    {
        public static string[] aircraft = { "SWA294" };

        public static Stream QUABN3()
        {
            Path path = new Path();
            path.waypoints.Add(new Waypoint("QUABN", new Position(42.4818583333, -71.3757722222), 11000, 250));
            path.waypoints.Add(new Waypoint("RSVOR", new Position(42.461325, -71.156025)));
            path.waypoints.Add(new Waypoint("MISTK", new Position(42.4495694444, -71.0270138889), 7000, 220));
            path.waypoints.Add(new Waypoint("GRIFI", new Position(42.3117138889, -70.934925)));
            path.waypoints.Add(new Waypoint("GGABE", new Position(42.2748, -70.9509111111), 6000, 220));
            path.waypoints.Add(new Waypoint("JOBEE", new Position(42.1783222222, -70.9922638889), 6000, 210));

            return new Stream(path);
        }
    }
}
