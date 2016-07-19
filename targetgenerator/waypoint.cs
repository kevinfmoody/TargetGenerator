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
        public double minimumAltitude { get; set; }
        public double maximumAltitude { get; set; }
        public double course { get; set; }

        public Waypoint(string identifier, Position position, double altitude = 0, double airspeed = 0)
        {
            this.identifier = identifier;
            this.position = position;
            this.altitude = altitude;
            this.airspeed = airspeed;
            this.minimumAltitude = 0;
            this.maximumAltitude = 0;
            this.course = 0;
        }

        public void updateMixOrMaxAltitude(double altitude)
        {
            if (this.altitude == 0)
            {
                this.altitude = altitude;
                this.minimumAltitude = altitude;
                this.maximumAltitude = altitude;
            }
            else if (altitude < this.altitude)
            {
                this.altitude = altitude;
                this.maximumAltitude = this.minimumAltitude;
                this.minimumAltitude = this.altitude;
            }
            else
            {
                this.maximumAltitude = altitude;
            }
        }

        public override string ToString()
        {
            string str = this.identifier;
            if (this.altitude != 0)
            {
                if (this.minimumAltitude == this.maximumAltitude)
                {
                    if (this.altitude >= 18000)
                    {
                        str += " / FL" + (this.altitude / 100);
                    }
                    else
                    {
                        str += " / " + this.altitude + "FT";
                    }
                }
                else
                {
                    if (this.minimumAltitude >= 18000)
                    {
                        str += " / FL" + (this.minimumAltitude / 100);
                    }
                    else
                    {
                        str += " / " + this.minimumAltitude + "FT";
                    }
                    if (this.maximumAltitude >= 18000)
                    {
                        str += " - FL" + (this.maximumAltitude / 100);
                    }
                    else
                    {
                        str += " - " + this.maximumAltitude + "FT";
                    }
                }
            }
            if (this.airspeed != 0)
            {
                str += " / " + this.airspeed + "KT";
            }
            if (this.course != 0)
            {
                str += " (THEN HDG " + this.course + ")";
            }
            str += " [" + this.position.latitude + ", " + this.position.longitude + "]";
            return str;
        }
    }
}
