using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Metacraft.Vatsim.Network;
using Metacraft.Vatsim.Network.PDU;

namespace TargetGenerator
{
    public partial class Form1 : Form
    {
        private Situation situation;

        public Form1()
        {
            InitializeComponent();

            Airspace airspace = Airspace.Instance;
            airspace.loadFixes(Data.FixesFile);


            var reader = new ArrivalFileReader();
            try
            {
                reader.readFile();

                ArrivalProcedure procedure = ArrivalProcedureStore.Instance.arrivalProcedure("ROBUC2");
                Path path = procedure.path("22L");
                Console.Write(path.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Arrival Procedure File Format");
            }

            Console.WriteLine("~~~~~~~~~~~");

            String line;
            System.IO.StreamReader file = new System.IO.StreamReader(Data.RoutesFile);
            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine(line);
                RouteParser.Parse(new Airport(line.Substring(0, 4)), new Airport(line.Substring(5, 4)), line);
            }

            ClientProperties props = new ClientProperties("TRATG", new Version(0, 1), 0xC768, FSDSession.FlipEndian("1d8c14be2b5bce1fb9a09a1ba4cfb0a1"));
            Airport kbos = new Airport("KBOS");

            Position rwy27pos = new Position(42.36021591666667, -70.9877015);
            Position rwy9pos = new Position(42.3557537, -71.01289621666666);
            Runway rwy27 = new Runway("27", rwy27pos, rwy27pos.courseTo(rwy9pos), 14.5, true);
            kbos.runways.Add(rwy27.identifier, rwy27);

            Position rwy4rpos = new Position(42.35105845, -71.0117940833);
            Position rwy22lpos = new Position(42.3769002833, -70.9992911167);
            Runway rwy4r = new Runway("4R", rwy4rpos, rwy4rpos.courseTo(rwy22lpos), 18.8, true);
            kbos.runways.Add(rwy4r.identifier, rwy4r);

            Position rwy4lpos = new Position(42.3579885333, -71.0143385833);
            Position rwy22rpos = new Position(42.3782907167, -71.0045162);
            Runway rwy4l = new Runway("4L", rwy4lpos, rwy4lpos.courseTo(rwy22rpos), 13.9, false);
            kbos.runways.Add(rwy4l.identifier, rwy4l);

            Runway rwy22l = new Runway("22L", rwy22lpos, rwy22lpos.courseTo(rwy4rpos), 14.5, true);
            kbos.runways.Add(rwy22l.identifier, rwy22l);
            
            Runway rwy22r = new Runway("22R", rwy22rpos, rwy22rpos.courseTo(rwy4lpos), 15.2, false);
            kbos.runways.Add(rwy22r.identifier, rwy22r);

            airspace.airports.Add(kbos.identifier, kbos);
            airspace.streams.Add("QUABN", Data.QUABN3());

            Console.WriteLine("Loading fixes...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            stopwatch.Stop();
            Console.WriteLine("Fixes loaded in " + stopwatch.ElapsedMilliseconds + "ms!");

            WeatherSituation weather = new WeatherSituation();

            situation = new Situation(props, airspace, weather);

            Aircraft ac1 = new Aircraft(situation, "SWA394", new Position(42.453460, -69.57343), 250, 5000, 240, "B737", "KJFK", "KBOS");
            Aircraft ac2 = new Aircraft(situation, "JBU2291", new Position(42.423460, -69.47343), 250, 4000, 360, "A320", "KJFK", "KBOS");
            Aircraft ac3 = new Aircraft(situation, "AAL2910", new Position(42.463460, -69.37343), 250, 6000, 350, "MD90", "KJFK", "KBOS");
            Aircraft ac4 = new Aircraft(situation, "JBU43", new Position(42.473460, -69.17343), 250, 6000, 340, "E190", "KJFK", "KBOS");

            situation.addAircraft(ac1);
            //situation.addAircraft(ac2);
            //situation.addAircraft(ac3);
            //situation.addAircraft(ac4);
        }

        private void SituationFileTextBox_Enter(object sender, EventArgs e)
        {
            
        }

        private void BrowseFilesButton_Click(object sender, EventArgs e)
        {
            SituationFileDialog.ShowDialog();
            foreach (string filename in SituationFileDialog.FileNames)
            {
                string extension = filename.Length >= 3 ? filename.Substring(filename.Length - 3).ToUpper() : "";
                switch (extension)
                {
                    case "ACS":
                        this.situation.loadAircraftFromACSimAircraftFile(filename);
                        break;
                    case "AIR":
                        this.situation.loadAircraftFromTWRTrainerAircraftFile(filename);
                        break;
                    default:
                        break;
                }
            }
        }

        /*private void Session_SessionReady(object sender, EventArgs e)
        {
            Aircraft ac1 = new Aircraft("SWA393", new Position(42.453460, -70.57343), 250, 5000, 240, "B737", "KJFK", "KBOS");
            session[0].addAircraft(ac1);

            /*Aircraft ac2 = new Aircraft("JBU2291", new Position(42.423460, -70.47343), 250, 4000, 360, "A320", "KJFK", "KBOS");
            session.addAircraft(ac2);

            Aircraft ac3 = new Aircraft("AAL2910", new Position(42.463460, -70.37343), 250, 6000, 350, "MD90", "KJFK", "KBOS");
            session.addAircraft(ac3);

            Aircraft ac4 = new Aircraft("JBU43", new Position(42.473460, -70.17343), 250, 6000, 340, "E190", "KJFK", "KBOS");
            session.addAircraft(ac4);
        }
        private void Session_SessionReady2(object sender, EventArgs e)
        {
            /*Aircraft ac1 = new Aircraft("SWA393", new Position(42.453460, -70.57343), 250, 5000, 240, "B737", "KJFK", "KBOS");
            session.addAircraft(ac1);

            Aircraft ac2 = new Aircraft("JBU2291", new Position(42.423460, -70.47343), 250, 4000, 360, "A320", "KJFK", "KBOS");
            session[1].addAircraft(ac2);

            /*Aircraft ac3 = new Aircraft("AAL2910", new Position(42.463460, -70.37343), 250, 6000, 350, "MD90", "KJFK", "KBOS");
            session.addAircraft(ac3);

            Aircraft ac4 = new Aircraft("JBU43", new Position(42.473460, -70.17343), 250, 6000, 340, "E190", "KJFK", "KBOS");
            session.addAircraft(ac4);
        }*/

    }
}
