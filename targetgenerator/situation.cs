using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metacraft.Vatsim.Network;

namespace TargetGenerator
{
    class Situation
    {
        ClientProperties properties;

        public Dictionary<string, SweatboxSession> sessions { get; set; }

        public Airspace airspace { get; set; }
        public WeatherSituation weather { get; set; }

        public bool paused { get; set; }

        public Situation(ClientProperties properties, Airspace airspace, WeatherSituation weather)
        {
            this.properties = properties;

            //SweatboxSession anchor = new SweatboxSession(this, this.properties, this.airspace, this.weather);

            this.sessions = new Dictionary<string, SweatboxSession>();
            
            this.airspace = airspace;
            this.weather = weather;

            this.paused = false;
        }

        public void loadAircraftFromACSimAircraftFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            string[] parts;
            bool valid = true;

            string callsign = "";
            int beacon = 0;
            double latitude = 0;
            double longitude = 0;
            double heading = 0;
            double altitude = 0;
            double airspeed = 0;
            double climbDescentRate = 0;
            string flightRules = "";
            string type = "";
            double cruiseSpeed = 0;
            string departure = "";
            string estimatedDepartureTime = "";
            string actualDepartureTime = "";
            string cruiseAltitude = "";
            string arrival = "";
            string hoursEnroute = "";
            string minEnroute = "";
            string alternate = "";
            string route = "";
            string remarks = "";

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                switch (i % 4)
                {
                    case 0:
                        parts = line.Split(new char[] { ',' });
                        if (parts.Length == 8)
                        {
                            callsign = parts[0];
                            valid = int.TryParse(parts[1], out beacon);
                            valid = valid && double.TryParse(parts[2], out latitude);
                            valid = valid && double.TryParse(parts[3], out longitude);
                            valid = valid && double.TryParse(parts[4], out heading);
                            valid = valid && double.TryParse(parts[5], out altitude);
                            valid = valid && double.TryParse(parts[6], out airspeed);
                            valid = valid && double.TryParse(parts[7], out climbDescentRate);
                        }
                        else
                        {
                            valid = false;
                        }
                        break;
                    case 1:
                        parts = line.Split(new char[] { ',' });
                        if (valid && parts.Length == 11)
                        {
                            flightRules = parts[0];
                            type = parts[1];
                            valid = valid && double.TryParse(parts[2], out cruiseSpeed);
                            departure = parts[3];
                            estimatedDepartureTime = parts[4];
                            actualDepartureTime = parts[5];
                            cruiseAltitude = parts[6];
                            arrival = parts[7];
                            hoursEnroute = parts[8];
                            minEnroute = parts[9];
                            alternate = parts[10];
                        }
                        else
                        {
                            valid = false;
                        }
                        break;
                    case 2:
                        if (valid)
                        {
                            route = line;
                        }
                        break;
                    case 3:
                        if (valid)
                        {
                            remarks = line;
                            Aircraft ac = new Aircraft(this, callsign, new Position(latitude, longitude), airspeed, altitude, heading, type, departure, arrival);
                            this.addAircraft(ac);
                        }
                        break;
                }
            }
        }

        public void loadAircraftFromTWRTrainerAircraftFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            string[] parts;
            bool valid = true;

            string callsign = "";
            string type = "";
            string category = "";
            string flightRules = "";
            string departure = "";
            string arrival = "";
            string cuiseAltitude = "";
            string route = "";
            string remarks = "";
            int beacon = 0;
            string squawkMode = "";
            double latitude = 0;
            double longitude = 0;
            double altitude = 0;
            double airspeed = 0;
            double heading = 0;

            foreach (string line in lines)
            {
                parts = line.Split(new char[] { ':' });

                if (parts.Length == 16)
                {
                    callsign = parts[0];
                    type = parts[1];
                    category = parts[2];
                    flightRules = parts[3];
                    departure = parts[4];
                    arrival = parts[5];
                    cuiseAltitude = parts[6];
                    route = parts[7];
                    remarks = parts[8];
                    valid = int.TryParse(parts[9], out beacon);
                    squawkMode = parts[10];
                    valid = valid && double.TryParse(parts[11], out latitude);
                    valid = valid && double.TryParse(parts[12], out longitude);
                    valid = valid && double.TryParse(parts[13], out altitude);
                    valid = valid && double.TryParse(parts[14], out airspeed);
                    valid = valid && double.TryParse(parts[15], out heading);

                    if (valid)
                    {
                        Aircraft ac = new Aircraft(this, callsign, new Position(latitude, longitude), airspeed, altitude, heading, type, departure, arrival);
                        this.addAircraft(ac);
                    }
                }
            }
        }

        public void addAircraft(Aircraft aircraft)
        {
            this.sessions.Add(aircraft.callsign, new SweatboxSession(this, this.properties, this.airspace, this.weather, aircraft));
        }

        public void deleteAircraft(string callsign)
        {
            if (this.sessions.ContainsKey(callsign))
            {
                this.sessions[callsign].disconnect();
                this.sessions.Remove(callsign);
            }
        }
    }
}
