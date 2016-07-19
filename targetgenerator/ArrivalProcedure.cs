using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class ArrivalProcedure
    {
        public enum SegmentType
        {
            EnrouteTransition,
            Enroute,
            Terminal,
            TerminalTransition
        }

        public string name { get; set; }
        public Dictionary<string, List<Waypoint>> enrouteTransitions { get; set; }
        public List<Waypoint> enroute { get; set; }
        public List<Waypoint> terminal { get; set; }
        public Dictionary<string, List<Waypoint>> terminalTransitions { get; set; }

        public ArrivalProcedure(string name)
        {
            this.name = name;

            this.enrouteTransitions = new Dictionary<string, List<Waypoint>>();
            this.enroute = new List<Waypoint>();
            this.terminal = new List<Waypoint>();
            this.terminalTransitions = new Dictionary<string, List<Waypoint>>();
        }

        public List<Waypoint> segment(SegmentType segmentType, string transition = "")
        {
            switch (segmentType)
            {
                case SegmentType.EnrouteTransition:
                    if (this.enrouteTransitions.ContainsKey(transition))
                    {
                        return this.enrouteTransitions[transition];
                    }
                    return null;
                case SegmentType.Enroute:
                    return this.enroute;
                case SegmentType.Terminal:
                    return this.terminal;
                case SegmentType.TerminalTransition:
                    if (this.terminalTransitions.ContainsKey(transition))
                    {
                        return this.terminalTransitions[transition];
                    }
                    return null;
                default:
                    return null;
            }
        }

        public Path path(string enrouteTransition = "", string terminalTransition = "")
        {
            Path path = new Path();
            if (enrouteTransition.Length != 0 && terminalTransition.Length == 0)
            {
                terminalTransition = enrouteTransition;
            }
            addEnroutePathSegment(path, enrouteTransition);
            addTerminalPathSegment(path, terminalTransition);
            return path;
        }

        public Path terminalPath(string terminalTransition = "")
        {
            Path path = new Path();
            addTerminalPathSegment(path, terminalTransition);
            return path;
        }

        private void addEnroutePathSegment(Path path, string enrouteTransition = "")
        {
            if (enrouteTransition.Length != 0 && this.enrouteTransitions.ContainsKey(enrouteTransition))
            {
                path.waypoints.AddRange(this.enrouteTransitions[enrouteTransition]);
            }
            path.waypoints.AddRange(this.enroute);
        }

        private void addTerminalPathSegment(Path path, string terminalTransition = "")
        {
            path.waypoints.AddRange(this.terminal);
            if (terminalTransition.Length != 0 && this.terminalTransitions.ContainsKey(terminalTransition))
            {
                path.waypoints.AddRange(this.terminalTransitions[terminalTransition]);
            }
        }
    }
}