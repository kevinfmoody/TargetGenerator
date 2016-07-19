using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class SituationFileReader
    {
        public string filename { get; set; }

        public SituationFileReader(string filename = "")
        {
            this.filename = filename;
        }

        public void readFile()
        {
            /*ArrivalProcedure arrivalProcedure = null;
            string line;
            ArrivalProcedure.SegmentType segmentType = ArrivalProcedure.SegmentType.EnrouteTransition;
            List<string> activeTransitions = new List<string>();
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Kevin Moody\Documents\visual studio 2015\projects\targetgenerator\targetgenerator\bin\debug\stars.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] tokens = line.Trim().Split(new char[] { ' ' });
            }*/
        }
    }
}
