using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metacraft.Vatsim.Network;
using Metacraft.Vatsim.Network.PDU;

namespace TargetGenerator
{
    class Commands
    {
        public SweatboxSession session { get; set; }
        public PDURadioMessage radio { get; set; }

        public Commands(SweatboxSession session, PDURadioMessage radio)
        {
            this.session = session;
            this.radio = radio;
        }

        public void run(string[] tokens)
        {
            string firstToken = tokens[0];
            int index = 0;

            if (firstToken[firstToken.Length - 1] == ',')
            {
                if (this.session.aircraft.callsign !=
                    firstToken.Substring(0, firstToken.Length - 1))
                {
                    return;
                }
                index = 1;
            }

            while (index < tokens.Length)
            {
                if (!this.maybeRunCommand(tokens, ref index))
                {
                    // tokens[index] was ignored.
                }
                index++;
            }
        }

        private bool hasNumArguments(string[] tokens, int index, int arguments)
        {
            return index + arguments < tokens.Length;
        }

        private int parseAltitude(int altitude)
        {
            return altitude < 1000 ? altitude * 100: altitude;
        }

        private bool maybeRunCommand(string[] tokens, ref int index)
        {
            return this.maybeRunVisualSeparationCommand(tokens, ref index) ||
                this.maybeRunVectoringCommand(tokens, ref index) ||
                this.maybeRunAltitudeAssignmentCommand(tokens, ref index) ||
                this.maybeRunSpeedAdjustmentCommand(tokens, ref index);
        }

        private bool maybeRunVisualSeparationCommand(string[] tokens, ref int index)
        {
            switch (tokens[index])
            {
                case "MVS":
                    if (this.hasNumArguments(tokens, index, 1))
                    {
                        this.maintainVisualSeparation(tokens[index + 1]);
                        index += 1;
                    }
                    return true;
            }
            return false;
        }

        private bool maybeRunVectoringCommand(string[] tokens, ref int index)
        {
            int i;

            switch (tokens[index])
            {
                case "TLH":
                case "L":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.turnLeftHeading(i);
                        index += 1;
                    }
                    return true;
                case "TRH":
                case "R":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.turnRightHeading(i);
                        index += 1;
                    }
                    return true;
                case "FH":
                case "H":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.flyHeading(i);
                        index += 1;
                    }
                    return true;
                case "TDL":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.turnDegreesLeft(i);
                        index += 1;
                    }
                    return true;
                case "TDR":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.turnDegreesRight(i);
                        index += 1;
                    }
                    return true;
                case "FPH":
                    this.flyPresentHeading();
                    return true;
                case "DH":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.departHeading(tokens[index + 1], i);
                        index += 2;
                    }
                    return true;
                case "PD":
                case "CD":
                case "D":
                    if (this.hasNumArguments(tokens, index, 1))
                    {
                        this.proceedDirect(tokens[index + 1]);
                        index += 1;
                    }
                    return true;
                case "WAPD":
                    if (this.hasNumArguments(tokens, index, 1))
                    {
                        this.whenAbleProceedDirect(tokens[index + 1]);
                        index += 1;
                    }
                    return true;
                case "JRI":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.joinRadialInbound(tokens[index + 1], i);
                        index += 2;
                    }
                    return true;
                case "JRO":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.joinRadialOutbound(tokens[index + 1], i);
                        index += 2;
                    }
                    return true;
                case "JL":
                case "J":
                    if (this.hasNumArguments(tokens, index, 1))
                    {
                        this.joinLocalizer(tokens[index + 1]);
                        index += 1;
                    }
                    return true;
            }
            return false;
        }

        private bool maybeRunAltitudeAssignmentCommand(string[] tokens, ref int index)
        {
            int i, j;

            switch (tokens[index])
            {
                case "CM":
                case "DM":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.climbDescendMaintain(this.parseAltitude(i));
                        index += 1;
                    }
                    return true;
                case "CAM":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.crossAtMaintain(tokens[index + 1], this.parseAltitude(i));
                        index += 2;
                    }
                    return true;
                case "CAA":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.crossAtAbove(tokens[index + 1], this.parseAltitude(i));
                        index += 2;
                    }
                    return true;
                case "CAB":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.crossAtBelow(tokens[index + 1], this.parseAltitude(i));
                        index += 2;
                    }
                    return true;
                case "CATCM":
                case "CATDM":
                    if (this.hasNumArguments(tokens, index, 3) &&
                        int.TryParse(tokens[index + 2], out i) &&
                        int.TryParse(tokens[index + 3], out j))
                    {
                        this.crossAtThenClimbDescendMaintain(tokens[index + 1],
                            this.parseAltitude(i), this.parseAltitude(j));
                        index += 3;
                    }
                    return true;
                case "AM":
                case "ACM":
                case "ADM":
                    if (this.hasNumArguments(tokens, index, 2) &&
                        int.TryParse(tokens[index + 2], out i))
                    {
                        this.afterMaintain(tokens[index + 1], this.parseAltitude(i));
                        index += 2;
                    }
                    return true;
            }
            return false;
        }

        private bool maybeRunSpeedAdjustmentCommand(string[] tokens, ref int index)
        {
            int i;

            switch (tokens[index])
            {
                case "MPS":
                    this.maintainPresentSpeed();
                    return true;
                case "MPSL":
                    this.maintainPresentSpeedOrLess();
                    return true;
                case "MPSG":
                    this.maintainPresentSpeedOrGreater();
                    return true;
                case "M":
                case "S":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.maintainSpeed(i);
                        index += 1;
                    }
                    return true;
                case "ML":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.maintainSpeedOrLess(i);
                        index += 1;
                    }
                    return true;
                case "MG":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        this.maintainSpeedOrGreater(i);
                        index += 1;
                    }
                    return true;
                case "MMFS":
                    this.maintainMaximumForwardSpeed();
                    return true;
                case "MSPS":
                    this.maintainSlowestPracticalSpeed();
                    return true;
                case "RS":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        if (i > 50)
                        {
                            this.reduceSpeedTo(i);
                        }
                        else
                        {
                            this.reduceSpeedBy(i);
                        }
                        index += 1;
                    }
                    return true;
                case "IS":
                    if (this.hasNumArguments(tokens, index, 1) &&
                        int.TryParse(tokens[index + 1], out i))
                    {
                        if (i > 50)
                        {
                            this.increaseSpeedTo(i);
                        }
                        else
                        {
                            this.increaseSpeedBy(i);
                        }
                        index += 1;
                    }
                    return true;
                case "RNS":
                    this.resumeNormalSpeed();
                    return true;
            }
            return false;
        }

        private void NotImplemented()
        {
            this.session.SendPDU(new PDURadioMessage(this.session.aircraft.callsign, 
                this.radio.Frequencies, "Command not implemented."));
        }

        public void maintainVisualSeparation(string callsign)
        {
            NotImplemented();
        }

        public void turnLeftHeading(int heading)
        {
            this.session.aircraft.mutex.WaitOne();
            this.session.aircraft.turnLeftHeading(heading);
            this.session.aircraft.mutex.ReleaseMutex();
        }

        public void turnRightHeading(int heading)
        {
            this.session.aircraft.mutex.WaitOne();
            this.session.aircraft.turnRightHeading(heading);
            this.session.aircraft.mutex.ReleaseMutex();
        }

        public void flyHeading(int heading)
        {
            this.session.aircraft.mutex.WaitOne();
            this.session.aircraft.flyHeading(heading);
            this.session.aircraft.mutex.ReleaseMutex();
        }

        public void turnDegreesLeft(int degrees)
        {
            NotImplemented();
        }
        
        public void turnDegreesRight(int degrees)
        {
            NotImplemented();
        }

        public void flyPresentHeading()
        {
            NotImplemented();
        }

        public void departHeading(string waypoint, int heading)
        {
            NotImplemented();
        }
        
        public void proceedDirect(string waypoint)
        {
            NotImplemented();
        }
        
        public void whenAbleProceedDirect(string waypoint)
        {
            NotImplemented();
        }

        public void joinRadialInbound(string waypoint, int radial)
        {
            NotImplemented();
        }

        public void joinRadialOutbound(string waypoint, int radial)
        {
            NotImplemented();
        }

        public void joinLocalizer(string runway)
        {
            NotImplemented();
        }

        public void climbDescendMaintain(int altitude)
        {
            this.session.aircraft.mutex.WaitOne();
            this.session.aircraft.climbDescendMaintain(altitude);
            this.session.aircraft.mutex.ReleaseMutex();
        }

        public void crossAtMaintain(string waypoint, int altitude)
        {
            NotImplemented();
        }

        public void crossAtAbove(string waypoint, int altitude)
        {
            NotImplemented();
        }

        public void crossAtBelow(string waypoint, int altitude)
        {
            NotImplemented();
        }

        public void crossAtThenClimbDescendMaintain(string waypoint,
            int altitude, int afterAltitude)
        {
            this.crossAtMaintain(waypoint, altitude);
            this.afterMaintain(waypoint, afterAltitude);
        }

        public void afterMaintain(string waypoint, int altitude)
        {
            NotImplemented();
        }

        public void maintainPresentSpeed()
        {
            NotImplemented();
        }

        public void maintainPresentSpeedOrLess()
        {
            NotImplemented();
        }

        public void maintainPresentSpeedOrGreater()
        {
            NotImplemented();
        }

        public void maintainSpeed(int speed)
        {
            this.session.aircraft.mutex.WaitOne();
            this.session.aircraft.airspeed.target = speed;
            this.session.aircraft.mutex.ReleaseMutex();
        }
        
        public void maintainSpeedOrLess(int speed)
        {
            NotImplemented();
        }

        public void maintainSpeedOrGreater(int speed)
        {
            NotImplemented();
        }
        
        public void maintainMaximumForwardSpeed()
        {
            NotImplemented();
        }

        public void maintainSlowestPracticalSpeed()
        {
            NotImplemented();
        }
        
        public void reduceSpeedTo(int speed)
        {
            NotImplemented();
        }

        public void reduceSpeedBy(int knots)
        {
            NotImplemented();
        }

        public void increaseSpeedTo(int speed)
        {
            NotImplemented();
        }

        public void increaseSpeedBy(int knots)
        {
            NotImplemented();
        }

        public void resumeNormalSpeed()
        {
            NotImplemented();
        }
    }
}
