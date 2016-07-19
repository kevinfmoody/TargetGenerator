using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class RouteParser
    {
        public static Path Parse(Airport departure, Airport arrival, string route)
        {
            string[] segments = route.Trim().ToUpper().Split(new char[] { ' ', '_', '-', '.', '/' });
            int i = segments.Length - 1;
            if (segments.Length == 0)
            {
                return null;
            }

            string lastSegment = segments[i];
            string arrivalProcedureName = "";
            string enrouteTransition = "";
            string terminalTransition = "";
            if ((lastSegment.Length == 3 || lastSegment.Length == 4) &&
                (lastSegment == departure.identifier || lastSegment == lastSegment.Substring(1)))
            {
                i--;
            }
            int min = Math.Max(0, i - 4);
            for (; i > min; i--)
            {
                string segment = segments[i];
                switch (segment.Length)
                {
                    case 2:
                        if (terminalTransition == "" && enrouteTransition == "" && char.IsDigit(segment[0]))
                        {
                            terminalTransition = segment;
                        }
                        break;
                    case 3:
                        if (terminalTransition == "" && enrouteTransition == "" && char.IsDigit(segment[0]))
                        {
                            terminalTransition = segment;
                        }
                        else if (enrouteTransition == "" && segment.All(char.IsLetter))
                        {
                            enrouteTransition = segment;
                        }
                        break;
                    case 4:
                    case 6:
                        if (arrivalProcedureName == "" && segment.Any(char.IsDigit))
                        {
                            arrivalProcedureName = segment;
                        }
                        break;
                    case 5:
                        if (enrouteTransition == "" && segment.All(char.IsLetter))
                        {
                            enrouteTransition = segment;
                        }
                        break;
                }
            }

            if (arrivalProcedureName.Length == 0)
            {
                return null;
            }

            // Debug print
            if (enrouteTransition != "")
            {
                Console.Write(enrouteTransition + " ");
            }
            Console.Write(arrivalProcedureName);
            if (terminalTransition != "")
            {
                Console.Write(" " + terminalTransition);
            }
            Console.Write("\n");

            ArrivalProcedure arrivalProcedure = ArrivalProcedureStore.Instance.arrivalProcedure(arrivalProcedureName);
            if (arrivalProcedure == null)
            {
                return null;
            }

            return arrivalProcedure.path(enrouteTransition, terminalTransition);
        }
    }
}
