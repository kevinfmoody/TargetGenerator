using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class ArrivalFileReader
    {
        public string filename { get; set; }

        public ArrivalFileReader(string filename = "")
        {
            this.filename = filename;
        }

        private void storeArrivalProcedure(ArrivalProcedure arrivalProcedure)
        {
            if (arrivalProcedure != null)
            {
                ArrivalProcedureStore.Instance.arrivalProcedures.Add(
                    arrivalProcedure.name, arrivalProcedure);
            }
        }

        public void readFile()
        {
            ArrivalProcedure arrivalProcedure = null;
            string line;
            ArrivalProcedure.SegmentType segmentType = ArrivalProcedure.SegmentType.EnrouteTransition;
            List<string> activeTransitions = new List<string>();
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Kevin Moody\Documents\visual studio 2015\projects\targetgenerator\targetgenerator\bin\debug\stars.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] tokens = line.Trim().Split(new char[] { ' ' });
                if (tokens.Length > 0)
                {
                    switch (tokens[0][0])
                    {
                        case '#':
                            storeArrivalProcedure(arrivalProcedure);
                            arrivalProcedure = new ArrivalProcedure(tokens[1]);
                            break;
                        case '@':
                            if (tokens[1].Any(char.IsDigit))
                            {
                                activeTransitions.Clear();
                                for (int i = 1; i < tokens.Length; i++)
                                {
                                    string transition = tokens[i];
                                    activeTransitions.Add(transition);
                                    if (!arrivalProcedure.terminalTransitions.ContainsKey(transition))
                                    {
                                        arrivalProcedure.terminalTransitions[transition] = new List<Waypoint>();
                                    }
                                }
                                segmentType = ArrivalProcedure.SegmentType.TerminalTransition;
                            }
                            else if (tokens[1] == "TERMINAL")
                            {
                                activeTransitions.Clear();
                                activeTransitions.Add("TERMINAL");
                                segmentType = ArrivalProcedure.SegmentType.Terminal;
                            }
                            else if (tokens[1] == "ENROUTE")
                            {
                                activeTransitions.Clear();
                                activeTransitions.Add("ENROUTE");
                                segmentType = ArrivalProcedure.SegmentType.Enroute;
                            }
                            else
                            {
                                activeTransitions.Clear();
                                for (int i = 1; i < tokens.Length; i++)
                                {
                                    string transition = tokens[i];
                                    activeTransitions.Add(transition);
                                    if (!arrivalProcedure.enrouteTransitions.ContainsKey(transition))
                                    {
                                        arrivalProcedure.enrouteTransitions[transition] = new List<Waypoint>();
                                    }
                                }
                                segmentType = ArrivalProcedure.SegmentType.EnrouteTransition;
                            }
                            break;
                        default:
                            string fixOrCourse = tokens[0];
                            if (fixOrCourse.Length == 3)
                            {
                                int course;
                                if (int.TryParse(fixOrCourse, out course))
                                {
                                    foreach (string transition in activeTransitions)
                                    {
                                        List<Waypoint> segment = arrivalProcedure.segment(segmentType, transition);
                                        segment[segment.Count - 1].course = course;
                                    }
                                    break;
                                }
                            }
                            Waypoint waypoint = new Waypoint(fixOrCourse, new Position());
                            for (int i = 1; i < tokens.Length; i++)
                            {
                                string token = tokens[i];
                                switch (token.Length)
                                {
                                    case 3:
                                        waypoint.airspeed = int.Parse(token);
                                        break;
                                    case 4:
                                        waypoint.updateMixOrMaxAltitude(int.Parse(token));
                                        break;
                                    case 5:
                                        if (token[0] == 'F')
                                        {
                                            waypoint.updateMixOrMaxAltitude(int.Parse(token.Substring(2)) * 100);
                                        }
                                        else
                                        {
                                            waypoint.updateMixOrMaxAltitude(int.Parse(token));
                                        }
                                        break;
                                }
                            }
                            foreach (string transition in activeTransitions)
                            {
                                List<Waypoint> segment = arrivalProcedure.segment(segmentType, transition);
                                segment.Add(waypoint);
                            }
                            break;
                    }
                }
            }
            storeArrivalProcedure(arrivalProcedure);
            file.Close();
        }
    }
}
