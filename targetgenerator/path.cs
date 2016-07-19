using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Path
    {
        public List<Waypoint> waypoints { get; set; }

        private int _activeWaypoint;
        public int activeWaypoint
        {
            get
            {
                return this._activeWaypoint;
            }
            set
            {
                this._activeWaypoint = value;
                if (this.isValidWaypointIndex(value))
                {
                    this.activePosition = this.waypoints[value].position;
                }
            }
        }
        public Position activePosition { get; set; }

        public Path()
        {
            this.waypoints = new List<Waypoint>();
            this.activeWaypoint = 0;
        }

        public Waypoint legWaypoint()
        {
            return this.waypoints[this.activeWaypoint];
        }

        public double legCourse()
        {
            if (this.activeWaypoint == 0)
            {
                return this.waypoints[0].position.courseTo(this.waypoints[1].position);
            }
            else
            {
                return this.waypoints[this.activeWaypoint - 1].position
                    .courseTo(this.waypoints[this.activeWaypoint].position);
            }
        }

        public double nextAltitude()
        {
            for (int i = this.activeWaypoint; i < waypoints.Count; i++)
            {
                if (waypoints[i].altitude != 0)
                {
                    return waypoints[i].altitude + 1;
                }
            }
            return 0;
        }

        public double nextAirspeed()
        {
            for (int i = this.activeWaypoint; i < waypoints.Count; i++)
            {
                if (waypoints[i].airspeed != 0)
                {
                    return waypoints[i].airspeed + 1;
                }
            }
            return 0;
        }

        public void updateActivePosition(string identifier, double distance)
        {
            if (this.waypoints.Count < 2)
            {
                return;
            }

            int i = 0;
            for (; i < this.waypoints.Count; i++)
            {
                if (this.waypoints[i].identifier == identifier)
                {
                    break;
                }
            }
            if (i >= this.waypoints.Count)
            {
                return;
            }
            
            double distanceRemaining = distance;
            while (this.isValidWaypointIndex(i))
            {
                int nextIndex = i - 1;
                Waypoint current = this.waypoints[i];
                if (this.isValidWaypointIndex(nextIndex))
                {
                    Waypoint next = this.waypoints[nextIndex];
                    double legDistance = current.position.distanceTo(next.position);
                    if (legDistance < distanceRemaining)
                    {
                        distanceRemaining -= legDistance;
                    }
                    else
                    {
                        this.activeWaypoint = i;
                        this.activePosition = current.position.destinationPoint(
                            current.position.courseTo(next.position), distanceRemaining);
                    }
                }
                else
                {
                    this.activeWaypoint = i;
                    Waypoint previous = this.waypoints[i + 1];
                    this.activePosition = current.position.destinationPoint(
                        previous.position.courseTo(current.position), distanceRemaining);
                }
                i = nextIndex;
            }
        }

        private bool isValidWaypointIndex(int index)
        {
            return index >= 0 && index < this.waypoints.Count;
        }

        public Aircraft spawnAircraft(Situation situation, string callsign, string type,
            string departure, string arrival)
        {
            Waypoint first = waypoints[0];
            Waypoint second = waypoints[1];

            double airspeed = this.nextAirspeed() != 0 ? this.nextAirspeed() : 211;
            double altitude = this.nextAltitude() != 0 ? this.nextAltitude() : 6001;
            double heading = (first.position.courseTo(second.position)
                - first.position.magneticDeclination(altitude) + 360) % 360;

            Aircraft ac = new Aircraft(situation, callsign, first.position, airspeed,
                altitude, heading, type, departure, arrival);
            ac.engagePathMode(this);
            return ac;
        }

        public override string ToString()
        {
            string str = "";
            int i = 0;
            foreach (Waypoint waypoint in this.waypoints)
            {
                if (this.activeWaypoint == i)
                {
                    str += "<ACTIVE> ";
                }
                str += waypoint.ToString() + "\n";
                i++;
            }
            return str;
        }
    }
}
