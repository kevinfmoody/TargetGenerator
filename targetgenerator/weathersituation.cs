using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Wind
    {
        public RadialControl direction { get; set; }
        public LinearControl velocity { get; set; }
        public LinearControl gust { get; set; }

        public Wind(double direction = 0.0, double velocity = 0.0, double gust = 0.0)
        {
            this.direction = new RadialControl(direction, direction, 1.0 / 1000.0);
            this.velocity = new LinearControl(velocity, velocity, 1.0 / 1000.0);
            this.gust = new LinearControl(gust, gust, 1.0 / 1000.0);
        }

        public void update(double milliseconds)
        {
            this.direction.update(milliseconds);
            this.velocity.update(milliseconds);
            this.gust.update(milliseconds);
        }

        public string info()
        {
            int direction = (int)this.direction.value;
            int velocity = (int)this.velocity.value;
            int gust = (int)this.gust.value;
            string wind = String.Format("{0:D3}{1:D2}", direction, velocity) + (gust > velocity ? String.Format("G{0:D2}KT", gust) : "KT");
            int directionTarget = (int)this.direction.target;
            int velocityTarget = (int)this.velocity.target;
            int gustTarget = (int)this.gust.target;
            string windTarget = String.Format("{0:D3}{1:D2}", directionTarget, velocityTarget) + (gustTarget > velocityTarget ? String.Format("G{0:D2}KT", gustTarget) : "KT");
            if (wind == windTarget)
            {
                return wind;
            }
            return String.Format("{0} (-> {1})", wind, windTarget);
        }
    }

    class WeatherStation
    {
        public string identifier { get; set; }
        public Position position { get; set; }
        public Wind wind { get; set; }

        public WeatherStation(string identifier, Position position, Wind wind)
        {
            this.identifier = identifier;
            this.position = position;
            this.wind = wind;
        }
    }

    class WeatherSituation
    {
        public Dictionary<string, WeatherStation> stations { get; set; }

        public WeatherSituation()
        {
            this.stations = new Dictionary<string, WeatherStation>();
            WeatherStation globalWx = new WeatherStation("GLOBAL", new Position(0.0, 0.0), new Wind());
            this.stations.Add(globalWx.identifier, globalWx);
        }

        public void update(double milliseconds)
        {
            foreach (WeatherStation station in stations.Values)
            {
                station.wind.update(milliseconds);
            }
        }

        public Wind windAtPosition(Position position)
        {
            Wind wind = new Wind();
            double closest = Double.MaxValue;
            foreach (WeatherStation station in stations.Values)
            {
                double distance = station.position.distanceTo(position);
                if (distance < closest)
                {
                    closest = distance;
                    wind = station.wind;
                }
            }
            return wind;
        }
    }
}
