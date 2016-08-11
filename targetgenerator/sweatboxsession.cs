using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metacraft.Vatsim.Network;
using Metacraft.Vatsim.Network.PDU;
using System.Threading;

namespace TargetGenerator
{
    class SweatboxSession : FSDSession
    {
        public Situation situation { get; set; }
        public Airspace airspace { get; set; }
        public WeatherSituation weather { get; set; }
        public Aircraft aircraft { get; set; }
        public Timer flightTimer { get; set; }
        public Timer blipTimer { get; set; }
        public event EventHandler<EventArgs> SessionReady;
        public bool identified { get; set; }

        public SweatboxSession(Situation situation, ClientProperties properties, Airspace airspace, WeatherSituation weather, Aircraft aircraft) : base(properties)
        {
            this.situation = situation;

            this.airspace = airspace;
            this.weather = weather;
            this.aircraft = aircraft;
            this.flightTimer = new Timer(this.flightTimerTick, null, Config.FLIGHT_INTERVAL, Timeout.Infinite);
            this.blipTimer = new Timer(this.blipTimerTick, null, Config.BLIP_INTERVAL, Timeout.Infinite);
            this.identified = false;

            this.IgnoreUnknownPackets = true;

            this.AcarsQueryReceived += SweatboxSession_AcarsQueryReceived;
            this.AcarsResponseReceived += SweatboxSession_AcarsResponseReceived;
            this.AddATCReceived += SweatboxSession_AddATCReceived;
            this.AddPilotReceived += SweatboxSession_AddPilotReceived;
            this.ATCMessageReceived += SweatboxSession_ATCMessageReceived;
            this.ATCPositionReceived += SweatboxSession_ATCPositionReceived;
            this.AuthChallengeReceived += SweatboxSession_AuthChallengeReceived;
            this.AuthResponseReceived += SweatboxSession_AuthResponseReceived;
            this.BroadcastMessageReceived += SweatboxSession_BroadcastMessageReceived;
            this.ClientIdentificationReceived += SweatboxSession_ClientIdentificationReceived;
            this.ClientQueryReceived += SweatboxSession_ClientQueryReceived;
            this.ClientQueryResponseReceived += SweatboxSession_ClientQueryResponseReceived;
            this.CloudDataReceived += SweatboxSession_CloudDataReceived;
            this.DeleteATCReceived += SweatboxSession_DeleteATCReceived;
            this.DeletePilotReceived += SweatboxSession_DeletePilotReceived;
            this.FlightPlanAmendmentReceived += SweatboxSession_FlightPlanAmendmentReceived;
            this.FlightPlanReceived += SweatboxSession_FlightPlanReceived;
            this.FlightStripReceived += SweatboxSession_FlightStripReceived;
            this.HandoffAcceptReceived += SweatboxSession_HandoffAcceptReceived;
            this.HandoffCancelledReceived += SweatboxSession_HandoffCancelledReceived;
            this.HandoffReceived += SweatboxSession_HandoffReceived;
            this.IHaveTargetReceived += SweatboxSession_IHaveTargetReceived;
            this.KillRequestReceived += SweatboxSession_KillRequestReceived;
            this.LandLineCommandReceived += SweatboxSession_LandLineCommandReceived;
            this.LegacyPlaneInfoResponseReceived += SweatboxSession_LegacyPlaneInfoResponseReceived;
            this.NetworkDisconnected += SweatboxSession_NetworkDisconnected;
            this.NetworkError += SweatboxSession_NetworkError;
            this.PilotPositionReceived += SweatboxSession_PilotPositionReceived;
            this.PingReceived += SweatboxSession_PingReceived;
            this.PlaneInfoRequestReceived += SweatboxSession_PlaneInfoRequestReceived;
            this.PlaneInfoResponseReceived += SweatboxSession_PlaneInfoResponseReceived;
            this.PointoutReceived += SweatboxSession_PointoutReceived;
            this.PongReceived += SweatboxSession_PongReceived;
            this.ProtocolErrorReceived += SweatboxSession_ProtocolErrorReceived;
            this.PushToDepartureListReceived += SweatboxSession_PushToDepartureListReceived;
            this.RadioMessageReceived += SweatboxSession_RadioMessageReceived;
            this.SecondaryVisCenterReceived += SweatboxSession_SecondaryVisCenterReceived;
            this.ServerIdentificationReceived += SweatboxSession_ServerIdentificationReceived;
            this.SharedStateReceived += SweatboxSession_SharedStateReceived;
            this.TemperatureDataReceived += SweatboxSession_TemperatureDataReceived;
            this.TextMessageReceived += SweatboxSession_TextMessageReceived;
            this.VersionRequestReceived += SweatboxSession_VersionRequestReceived;
            this.WallopReceived += SweatboxSession_WallopReceived;
            this.WeatherProfileRequestReceived += SweatboxSession_WeatherProfileRequestReceived;
            this.WindDataReceived += SweatboxSession_WindDataReceived;

            this.Connect("178.62.60.186", 6809);
        }

        public void addAircraft()
        {
            this.aircraft.mutex.WaitOne();
            PDUAddPilot addPilot = new PDUAddPilot(
                this.aircraft.callsign,
                "1097238",
                "201815",
                NetworkRating.OBS,
                ProtocolRevision.VatsimAuth,
                SimulatorType.MSCFS,
                this.aircraft.callsign
            );
            this.SendPDU(addPilot);
            PDUFlightPlan flightPlan = new PDUFlightPlan(this.aircraft.callsign, "SERVER", FlightRules.IFR, this.aircraft.type, "", this.aircraft.departure, "", "", "", this.aircraft.arrival, "", "", "", "", "", "/v/ Created by simGen", "");
            this.SendPDU(flightPlan);
            PDUTextMessage getBeaconCode = new PDUTextMessage(this.aircraft.callsign, "FP", this.aircraft.callsign + " GET");
            this.SendPDU(getBeaconCode);
            aircraft.mutex.ReleaseMutex();
        }

        /*public void addAnchor()
        {
            PDUAddATC addATC = new PDUAddATC(
                "SIM_GEN",
                "SIM_GEN",
                "1097238",
                "201815",
                NetworkRating.I1,
                ProtocolRevision.VatsimAuth
            );
            this.SendPDU(addATC);
            PDUATCPosition pos = new PDUATCPosition("SIM_GEN", 134700, NetworkFacility.APP, 150, NetworkRating.I1, 42, -71);
            this.SendPDU(pos);
            List<string> payload = new List<string>();
            payload.Add("SWA393");
            payload.Add("3701");
            PDUClientQuery setBeacon = new PDUClientQuery("SWA393", "@94385", ClientQueryType.SetBeaconCode, payload);
        }*/

        private void blipTimerTick(object state)
        {
            this.aircraft.mutex.WaitOne();
            PDUPilotPosition position = new PDUPilotPosition(
                this.aircraft.callsign,
                this.aircraft.beacon,
                true,
                false,
                NetworkRating.OBS,
                this.aircraft.position.latitude,
                this.aircraft.position.longitude,
                (int)this.aircraft.altitude.value,
                (int)this.aircraft.altitude.value,
                (int)this.aircraft.groundspeed(this.weather),
                0,
                0,
                (int)this.aircraft.heading.value
            );
            this.aircraft.mutex.ReleaseMutex();
            this.SendPDU(position);
            if (!this.identified)
            {
                this.identified = true;
                PDUTextMessage sayAdded = new PDUTextMessage(this.aircraft.callsign, "@18250", "With you at " + Math.Round(this.aircraft.altitude.value / 100.0) * 100);
                this.SendPDU(sayAdded);
            }
            this.blipTimer.Change(Config.BLIP_INTERVAL, Timeout.Infinite);
        }

        private void flightTimerTick(object state)
        {
            this.weather.update(Config.FLIGHT_INTERVAL);
            this.aircraft.mutex.WaitOne();
            this.aircraft.fly(this.weather, Config.FLIGHT_INTERVAL);
            this.aircraft.mutex.ReleaseMutex();
            this.flightTimer.Change(Config.FLIGHT_INTERVAL, Timeout.Infinite);
        }

        private void SweatboxSession_WindDataReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUWindData> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_WeatherProfileRequestReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUWeatherProfileRequest> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_WallopReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUWallop> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_VersionRequestReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUVersionRequest> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_TextMessageReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUTextMessage> e)
        {
            //System.Windows.Forms.MessageBox.Show(e.PDU.Message);
        }

        private void SweatboxSession_TemperatureDataReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUTemperatureData> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_SharedStateReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUSharedState> e)
        {
            PDUSharedState state = e.PDU;
            if (state.Target == this.aircraft.callsign)
            {
                if (state.SharedStateType == SharedStateType.BeaconCode)
                {
                    int beacon;
                    if (int.TryParse(state.Value, out beacon))
                    {
                        this.aircraft.beacon = beacon;
                    }
                }
            }
        }

        private void SweatboxSession_ServerIdentificationReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUServerIdentification> e)
        {
            PDUServerIdentification serverIdent = e.PDU;
            PDUClientIdentification clientIdent = new PDUClientIdentification(serverIdent.To, this.ClientProperties.ClientID, this.ClientProperties.Name, this.ClientProperties.Version.Major, this.ClientProperties.Version.Minor, "1097238", "1097238", null);
            this.SendPDU(clientIdent);
            //if (this.aircraft != null)
            //{
                this.addAircraft();
            //}   
        }

        private void SweatboxSession_SecondaryVisCenterReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUSecondaryVisCenter> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_RadioMessageReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDURadioMessage> e)
        {
            PDURadioMessage radio = e.PDU;
            string[] tokens = radio.Message.ToUpper().Split(new char[] { ' ' });
            if (tokens.Length == 0 || tokens[0].Length == 0)
            {
                return;
            }

            Commands commands = new Commands(this, radio);
            commands.run(tokens);
            return;

            string[] parts = tokens;
            string firstPart = parts[0];
            if (firstPart.Length == 0)
            {
                return;
            }
            if (firstPart[firstPart.Length - 1] == ',')
            {
                string callsign = firstPart.Substring(0, firstPart.Length - 1);
                if (this.aircraft.callsign != callsign)
                {
                    return;
                }
                Aircraft aircraft = this.aircraft;
                Runway runway;
                string command = parts[1];
                PDURadioMessage response;
                switch (command)
                {
                    case "AA":
                        if (parts.Length >= 3)
                        {
                            Stream stream;
                            if (this.situation.airspace.streams.TryGetValue(parts[2], out stream))
                            {
                                List<double> distances = new List<double>();
                                switch (parts.Length)
                                {
                                    case 3:
                                        distances.Add(0);
                                        break;
                                    case 4:
                                        /*double distance;
                                        if (double.TryParse(parts[3], out distance))
                                        {
                                            distances.Add(distance);
                                        }*/
                                        double slots;
                                        Random rand = new Random();
                                        if (double.TryParse(parts[3], out slots))
                                        {
                                            double slot = (125 - 5 * (slots - 1)) / slots;
                                            for (double i = 0; i < slots; i++)
                                            {
                                                double offset = i * (slot + 5);
                                                distances.Add(offset + rand.NextDouble() * slot);
                                            }
                                        }
                                        break;
                                    case 5:
                                        double spacing, count;
                                        if (double.TryParse(parts[3], out spacing) && double.TryParse(parts[4], out count))
                                        {
                                            for (int i = 0; i < count; i++)
                                            {
                                                distances.Add(spacing * i);
                                            }
                                        }
                                        break;
                                }

                                double backcourse = (stream.heading + 180 + stream.position.magneticDeclination(stream.altitude)) % 360;
                                foreach (double distance in distances)
                                {
                                    string addCallsign, addAircraftType;
                                    do
                                    {
                                        this.airspace.randomAircraft(out addCallsign, out addAircraftType);
                                    }
                                    while (this.situation.sessions.ContainsKey(addCallsign));

                                    Position addPosition = stream.position.destinationPoint(backcourse, distance);
                                    Aircraft ac = new Aircraft(this.situation, addCallsign, addPosition, stream.airspeed, stream.altitude, stream.heading, addAircraftType, "", "KBOS");
                                    if (stream.path != null)
                                    {
                                        ac.engagePathMode(stream.path);
                                    }
                                    this.situation.addAircraft(ac);
                                }
                            }
                        }
                        break;
                    case "AS":
                        if (parts.Length >= 6)
                        {
                            string name = parts[2];
                            if ((runway = this.getRunway(parts[3])) != null)
                            {
                                Position streamStart = runway.position;
                                double backcourse = (runway.course + 180) % 360;
                                for (int i = 4; i < parts.Length - 1; i += 2)
                                {
                                    double turn, distance;
                                    if (double.TryParse(parts[i], out turn) && double.TryParse(parts[i + 1], out distance))
                                    {
                                        backcourse = (backcourse + turn + 360) % 360;
                                        streamStart = streamStart.destinationPoint(backcourse, distance);
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                                Stream stream = new Stream(streamStart, 210, 6000, (backcourse + 180 - streamStart.magneticDeclination(6000)) % 360);
                                this.situation.airspace.streams.Add(name, stream);
                                response = new PDURadioMessage(aircraft.callsign, radio.Frequencies, "STREAM ADDED: " + name);
                                this.SendPDU(response);
                            }
                        }
                        return;
                    default:
                        if (parts.Length == 2)
                        {
                            switch (command)
                            {
                                case "RNS":
                                    this.aircraft.mutex.WaitOne();
                                    this.aircraft.resumeNormalSpeed();
                                    this.aircraft.mutex.ReleaseMutex();
                                    break;
                                case "DEL":
                                    this.situation.deleteAircraft(this.aircraft.callsign);
                                    break;
                                case "WX":
                                    response = new PDURadioMessage(aircraft.callsign, radio.Frequencies, "WND: " + this.weather.windAtPosition(aircraft.position).info());
                                    this.SendPDU(response);
                                    break;
                                case "P":
                                    this.situation.paused = !this.situation.paused;
                                    response = new PDURadioMessage(aircraft.callsign, radio.Frequencies, this.situation.paused ? "PAUSED" : "UNPAUSED");
                                    this.SendPDU(response);
                                    break;
                                case "INFO":
                                    response = new PDURadioMessage(aircraft.callsign, radio.Frequencies, "SPD: " + aircraft.airspeed.info());
                                    this.SendPDU(response);
                                    response = new PDURadioMessage(aircraft.callsign, radio.Frequencies, "ALT: " + aircraft.altitude.info());
                                    this.SendPDU(response);
                                    response = new PDURadioMessage(aircraft.callsign, radio.Frequencies, "HDG: " + aircraft.heading.info());
                                    this.SendPDU(response);
                                    break;
                            }
                        }
                        else if (parts.Length == 3)
                        {
                            int arg;
                            switch (command)
                            {
                                case "TL":
                                    if (int.TryParse(parts[2], out arg) == false)
                                    {
                                        return;
                                    }
                                    aircraft.mutex.WaitOne();
                                    aircraft.turnLeftHeading(arg);
                                    aircraft.mutex.ReleaseMutex();
                                    break;
                                case "TR":
                                    if (int.TryParse(parts[2], out arg) == false)
                                    {
                                        return;
                                    }
                                    aircraft.mutex.WaitOne();
                                    aircraft.turnRightHeading(arg);
                                    aircraft.mutex.ReleaseMutex();
                                    break;
                                case "FH":
                                    if (int.TryParse(parts[2], out arg) == false)
                                    {
                                        return;
                                    }
                                    aircraft.mutex.WaitOne();
                                    aircraft.flyHeading(arg);
                                    aircraft.mutex.ReleaseMutex();
                                    break;
                                case "CM":
                                case "DM":
                                    if (int.TryParse(parts[2], out arg) == false)
                                    {
                                        return;
                                    }
                                    aircraft.mutex.WaitOne();
                                    aircraft.climbDescendMaintain(arg);
                                    aircraft.mutex.ReleaseMutex();
                                    break;
                                case "SPD":
                                    if (int.TryParse(parts[2], out arg) == false)
                                    {
                                        return;
                                    }
                                    aircraft.mutex.WaitOne();
                                    aircraft.airspeed.target = arg;
                                    aircraft.mutex.ReleaseMutex();
                                    break;
                                case "VA":
                                    if ((runway = this.getRunway(parts[2])) != null)
                                    {
                                        this.aircraft.mutex.WaitOne();
                                        this.aircraft.engageVisualApproach(runway);
                                        this.aircraft.mutex.ReleaseMutex();
                                    }
                                    break;
                                case "LOC":
                                    if ((runway = this.getRunway(parts[2])) != null)
                                    {
                                        if (runway.ils)
                                        {
                                            this.aircraft.mutex.WaitOne();
                                            this.aircraft.engageLocalizerMode(runway);
                                            this.aircraft.mutex.ReleaseMutex();
                                        }
                                    }
                                    break;
                                case "ILS":
                                    if ((runway = this.getRunway(parts[2])) != null)
                                    {
                                        if (runway.ils)
                                        {
                                            this.aircraft.mutex.WaitOne();
                                            this.aircraft.engageApproachMode(runway);
                                            this.aircraft.mutex.ReleaseMutex();
                                        }
                                    }
                                    break;
                            }
                        }
                        else if (parts.Length == 4)
                        {
                            int arg, arg2;
                            if (int.TryParse(parts[2], out arg) == false || int.TryParse(parts[3], out arg2) == false)
                            {
                                return;
                            }
                            switch (command)
                            {
                                case "WIND":
                                    this.weather.stations["GLOBAL"].wind.direction.target = arg;
                                    this.weather.stations["GLOBAL"].wind.velocity.target = arg2;
                                    break;
                            }
                        }
                        break;
                }
            }
            else
            {
                string command = firstPart;
                switch (command)
                {
                    default:
                        break;
                }
            }
        }

        private void SweatboxSession_PushToDepartureListReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPushToDepartureList> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_ProtocolErrorReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUProtocolError> e)
        {
            System.Windows.Forms.MessageBox.Show("ERROR: " + e.PDU.Message);
            throw new NotImplementedException();
        }

        private void SweatboxSession_PongReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPong> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_PointoutReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPointout> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_PlaneInfoResponseReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPlaneInfoResponse> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_PlaneInfoRequestReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPlaneInfoRequest> e)
        {
            PDUPlaneInfoRequest request = e.PDU;
            PDUPlaneInfoResponse response = new PDUPlaneInfoResponse(request.To, request.From, "B737", "", "", "");
            this.SendPDU(response);
            //throw new NotImplementedException();
        }

        private void SweatboxSession_PingReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPing> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_PilotPositionReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUPilotPosition> e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_NetworkError(object sender, NetworkErrorEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_NetworkDisconnected(object sender, NetworkEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_LegacyPlaneInfoResponseReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDULegacyPlaneInfoResponse> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_LandLineCommandReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDULandLineCommand> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_KillRequestReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUKillRequest> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_IHaveTargetReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUIHaveTarget> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_HandoffReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUHandoff> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_HandoffCancelledReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUHandoffCancelled> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_HandoffAcceptReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUHandoffAccept> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_FlightStripReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUFlightStrip> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_FlightPlanReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUFlightPlan> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_FlightPlanAmendmentReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUFlightPlanAmendment> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_DeletePilotReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUDeletePilot> e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_DeleteATCReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUDeleteATC> e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_CloudDataReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUCloudData> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_ClientQueryResponseReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUClientQueryResponse> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_ClientQueryReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUClientQuery> e)
        {
            PDUClientQuery query = e.PDU;
            switch (query.QueryType)
            {
                case ClientQueryType.SetBeaconCode:
                    if (this.aircraft.callsign == query.Payload[0])
                    {
                        int beacon;
                        if (int.TryParse(query.Payload[1], out beacon) && beacon != 0)
                        {
                            this.aircraft.beacon = beacon;
                        }
                    }
                    break;
                case ClientQueryType.Capabilities:
                    //List<string> caps = new List<string>();
                    //caps.Add("ATC=1");
                    //PDUClientQueryResponse response = new PDUClientQueryResponse(query.To, query.From, query.QueryType, caps);
                    //this.SendPDU(response);
                    return;
                case ClientQueryType.RealName:
                    List<string> name = new List<string>();
                    name.Add("Kevin Moody");
                    name.Add("");
                    name.Add(NetworkRating.I1.ToString());
                    PDUClientQueryResponse response = new PDUClientQueryResponse(query.To, query.From, query.QueryType, name);
                    this.SendPDU(response);
                    return;
                case ClientQueryType.COM1Freq:
                    //List<string> name = new List<string>();
                    //name.Add("Kevin Moody");
                    //PDUClientQueryResponse response = new PDUClientQueryResponse(query.To, query.From, query.QueryType, name);
                    //this.SendPDU(response);
                    return;
                case ClientQueryType.FlightPlan:
                    if (this.aircraft.callsign == query.To)
                    {
                        PDUFlightPlan flightPlan = new PDUFlightPlan(query.To, query.From, FlightRules.IFR, this.aircraft.type, "", this.aircraft.departure, "", "", "", this.aircraft.arrival, "", "", "", "", "", "", "");
                        this.SendPDU(flightPlan);
                    }
                    return;
                default:
                    Console.WriteLine(query.QueryType.ToString());
                    break;
            }
        }

        private void SweatboxSession_ClientIdentificationReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUClientIdentification> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_BroadcastMessageReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUBroadcastMessage> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_AuthResponseReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUAuthResponse> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_AuthChallengeReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUAuthChallenge> e)
        {
            //Console.WriteLine("auth challenge received");
            throw new NotImplementedException();
        }

        private void SweatboxSession_ATCPositionReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUATCPosition> e)
        {
            Console.WriteLine("CHECK: ATC POS RECEIVED");
        }

        private void SweatboxSession_ATCMessageReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUATCMessage> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_AddPilotReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUAddPilot> e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_AddATCReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUAddATC> e)
        {
            //throw new NotImplementedException();
        }

        private void SweatboxSession_AcarsResponseReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUMetarResponse> e)
        {
            throw new NotImplementedException();
        }

        private void SweatboxSession_AcarsQueryReceived(object sender, DataReceivedEventArgs<Metacraft.Vatsim.Network.PDU.PDUMetarRequest> e)
        {
            throw new NotImplementedException();
        }

        private Runway getRunway(string runway)
        {
            Runway rwy = null;
            this.aircraft.mutex.WaitOne();
            if (this.airspace.airports.ContainsKey(this.aircraft.arrival))
            {
                Airport arrival = this.airspace.airports[this.aircraft.arrival];
                if (arrival.runways.ContainsKey(runway))
                {
                    rwy = arrival.runways[runway];
                }
            }
            this.aircraft.mutex.ReleaseMutex();
            return rwy;
        }

        public void disconnect()
        {
            this.blipTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.flightTimer.Change(Timeout.Infinite, Timeout.Infinite);
            PDUDeletePilot deletePilot = new PDUDeletePilot(this.aircraft.callsign, "1097238");
            this.SendPDU(deletePilot);
        }
    }
}
