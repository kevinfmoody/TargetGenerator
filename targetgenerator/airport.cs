using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Airport
    {
        public string identifier { get; set; }
        public Dictionary<string, Runway> runways { get; set; }

        public Airport(string identifier)
        {
            this.identifier = identifier;
            this.runways = new Dictionary<string, Runway>();
        }
    }
}
