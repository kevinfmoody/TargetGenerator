using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Airspace
    {
        public Dictionary<string, Airport> airports { get; set; }
        public Dictionary<string, List<string>> airlines { get; set; }
        public Dictionary<string, Stream> streams { get; set; }

        public Airspace()
        {
            this.airports = new Dictionary<string, Airport>();
            this.airlines = new Dictionary<string, List<string>>();
            this.streams = new Dictionary<string, Stream>();

            this.airlines.Add("AAL", new List<string>(new string[] {
                "B738",
                "A321",
                "A319"
            }));
            this.airlines.Add("UAL", new List<string>(new string[] {
                "B738",
                "B739",
                "B733"
            }));
            this.airlines.Add("SWA", new List<string>(new string[] {
                "B737",
                "B733"
            }));
            this.airlines.Add("JBU", new List<string>(new string[] {
                "A320",
                "E190"
            }));
            this.airlines.Add("NKS", new List<string>(new string[] {
                "A320"
            }));
            this.airlines.Add("DAL", new List<string>(new string[] {
                "B752",
                "MD90"
            }));
            this.airlines.Add("ACA", new List<string>(new string[] {
                "E190"
            }));
        }

        public void randomAircraft(out string callsign, out string aircraft)
        {
            Random rand = new Random();
            KeyValuePair<string, List<string>> pair = this.airlines.ElementAt(rand.Next(0, this.airlines.Count));
            callsign = pair.Key + (rand.Next(0, 10) < 7 ? rand.Next(1, 1000) : rand.Next(1, 5000));
            aircraft = pair.Value.ElementAt(rand.Next(0, pair.Value.Count));
        }
    }
}
