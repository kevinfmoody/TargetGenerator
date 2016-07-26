using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Metacraft.Vatsim.Network;

namespace TargetGenerator
{
    class Situation
    {
        struct AircraftSlot
        {
            public string type;
            public double distance;
        }

        enum ParseState
        {
            Normal,
            RepeatGroup
        }

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

        private int toggleParseState(int states, int state)
        {
            return states | state;
        }

        private bool hasParseState(int states, int state)
        {
            return (states & state) != 0;
        }

        private void parseAircraftPart(string part, List<AircraftSlot> slots,double distanceOffset)
        {
            AircraftSlot slot = new AircraftSlot();
            string[] aircraftParts = part.Split(new char[] { ':' });
            double[] sums = new double[aircraftParts.Length];
            double sum = 0;
            for (int i = 0; i < aircraftParts.Length; i++)
            {
                double ratio = 1;
                Match match = Regex.Match(aircraftParts[i], @"\d+");
                if (match.Success)
                {
                    ratio = double.Parse(match.Value);
                }
                sum += ratio;
                sums[i] = sum;
            }
            double choice = Rand.Instance.getDouble() * sum;
            for (int i = 0; i < sums.Length; i++)
            {
                if (sums[i] <= choice)
                {
                    slot.type = Regex.Match(aircraftParts[i], @"^\d").Value;
                    break;
                }
            }
            slot.distance = distanceOffset;
            slots.Add(slot);
        }

        private void parseDistancePart(string part, ref double distanceOffset)
        {
            double distance;
            int minMaxSeperatorIndex = part.IndexOf('-');
            if (minMaxSeperatorIndex != -1)
            {
                double minDistance = double.Parse(part.Substring(0, minMaxSeperatorIndex));
                double maxDistance = double.Parse(part.Substring(minMaxSeperatorIndex + 1));
                distance = minDistance + Rand.Instance.getDouble() * (maxDistance - minDistance);
            }
            else
            {
                distance = double.Parse(part);
            }
            distanceOffset += distance;
        }

        private void parseStreamPart(string part, List<AircraftSlot> slots,
            ref double distanceOffset)
        {
            if (part.Any(char.IsLetter))
            {
                this.parseAircraftPart(part, slots, distanceOffset);
            }
            else
            {
                this.parseDistancePart(part, ref distanceOffset);
            }
        }

        private void parseRepeatGroupParts(List<string> parts, int count, List<AircraftSlot> slots,
            ref double distanceOffset)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (string part in parts)
                {
                    parseStreamPart(part, slots, ref distanceOffset);
                }
            }
        }

        private void parseAircraftSlots(string[] parts, List<AircraftSlot> slots,
            ref double distanceOffset)
        {
            List<string> buffer = new List<string>();
            ParseState parseState = ParseState.Normal;
            for (int i = 3; i < parts.Length; i++)
            {
                string part = parts[i];
                switch (parseState)
                {
                    case ParseState.Normal:
                        if (part[0] == '(')
                        {
                            parseState = ParseState.RepeatGroup;
                            buffer.Clear();
                            buffer.Add(part.Substring(1));
                        }
                        else
                        {
                            this.parseStreamPart(part, slots, ref distanceOffset);
                        }
                        break;
                    case ParseState.RepeatGroup:
                        int end = part.IndexOf(')');
                        if (end != -1)
                        {
                            buffer.Add(part.Substring(0, end));
                            int count = int.Parse(part.Substring(end + 2));
                            this.parseRepeatGroupParts(buffer, count, slots,
                                ref distanceOffset);
                            parseState = ParseState.Normal;
                        }
                        else
                        {
                            buffer.Add(part);
                        }
                        break;
                }
            }
        }

        private void loadAircraftFromStreamParts(string[] parts)
        {
            if (parts.Length < 4)
            {
                return;
            }
            string airport = parts[1];
            string arrival = parts[2];
            double distanceOffset = 0;
            List<AircraftSlot> slots = new List<AircraftSlot>();
            this.parseAircraftSlots(parts, slots, ref distanceOffset);

            ArrivalProcedure procedure = ArrivalProcedureStore.Instance.arrivalProcedure(arrival);
            Path path = procedure.path();
            string terminalWaypoint = procedure.terminalWaypoint.identifier;
            int i = 1;
            string callsign = arrival.Substring(0, 3);
            foreach (AircraftSlot slot in slots)
            {
                Console.WriteLine(slot.distance + "NM");
                Path acPath = path.pathRelativeToWaypoint(terminalWaypoint, slot.distance + 5);
                Aircraft ac = new Aircraft(this, callsign + i, new Position(), 250, 5000, 240, "B737", "KJFK", "KBOS");
                ac.snapToPath(acPath);
                this.addAircraft(ac);
                i++;
            }
        }

        public void loadAircraftFromSituationFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(new char[] { ' ' });
                if (parts.Length == 0)
                {
                    return;
                }
                switch (parts[0])
                {
                    case "STREAM":
                        this.loadAircraftFromStreamParts(parts);
                        break;
                }
            }
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
