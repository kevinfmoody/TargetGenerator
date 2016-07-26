using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueNorth.Geographic;

namespace TargetGenerator
{
    class Aircraft
    {
        public enum NavMode
        {
            OFF,
            LOCALIZER,
            APPROACH,
            VISUAL,
            PATH
        };

        Random random;
        public Mutex mutex { get; set; }

        public Situation situation { get; set; }
        
        public string callsign { get; set; }
        public Position position { get; set; }
        public int beacon { get; set; }

        public LinearControl airspeed { get; set; }
        public LinearControl altitude { get; set; }
        public RadialControl heading { get; set; }

        public string type { get; set; }
        public string departure { get; set; }
        public string arrival { get; set; }
        
        public NavMode navMode { get; set; }

        public Runway runway { get; set; }

        public Path path { get; set; }

        public Aircraft(Situation situation, string callsign, Position position, double airspeed, double altitude, double heading, string type, string departure, string arrival)
        {
            this.situation = situation;

            this.random = new Random();
            this.mutex = new Mutex();

            this.callsign = callsign;
            this.position = position;
            this.beacon = 2108;

            this.airspeed = new LinearControl(airspeed, airspeed, 2.0 / 1000.0);
            this.altitude = new LinearControl(altitude, altitude, 1800.0 / 60.0 / 1000.0);
            this.heading = new RadialControl(heading, heading, 3.0 / 1000.0);

            this.type = type;
            this.departure = departure;
            this.arrival = arrival;

            this.navMode = NavMode.OFF;

            this.runway = null;

            this.path = null;
        }

        public void fly(WeatherSituation wx, double milliseconds)
        {
            if (!this.situation.paused)
            {
                this.updateAutopilotTargets(wx);
                this.airspeed.update(milliseconds);
                this.altitude.update(milliseconds);
                this.heading.update(milliseconds);
                this.updatePosition(wx, milliseconds);
            }
            this.deleteIfNecessary();
        }

        public bool isOnFinalApproach()
        {
            return this.navMode == NavMode.VISUAL || this.navMode == NavMode.APPROACH || this.navMode == NavMode.LOCALIZER;
        }

        public void deleteIfNecessary()
        {
            if (this.isOnFinalApproach() && this.altitude.value < this.runway.elevation + 100)
            {
                this.situation.deleteAircraft(this.callsign);
            }
        }

        public void updateAutopilotTargets(WeatherSituation wx)
        {
            if (this.navMode == NavMode.LOCALIZER || this.navMode == NavMode.APPROACH)
            {
                double course = (this.heading.target + this.position.magneticDeclination(this.altitude.value) + 360) % 360;
                double deltaCourse = Radial.AngleBetween(course, this.runway.course);
                Console.WriteLine(this.heading.target);
                if (Math.Abs(deltaCourse) <= 45)
                {
                    double deltaPosition = Radial.AngleBetween(this.position.courseTo(this.runway.position), this.runway.course);
                    double distanceFromRunway = Math.Abs(Math.Cos(Position.DegreesToRadians(deltaPosition))) * this.position.distanceTo(this.runway.position);
                    double distanceFromLocalizer = Math.Abs(Math.Sin(Position.DegreesToRadians(deltaPosition))) * this.position.distanceTo(this.runway.position);
                    double runwayHeading = this.runway.course - this.runway.position.magneticDeclination(this.runway.elevation);
                    double interceptAngle = Position.RadiansToDegrees(Math.Atan(distanceFromLocalizer * 500 / this.groundspeed(wx)));
                    if ((Radial.Direction(deltaCourse) == Radial.Direction(deltaPosition) && interceptAngle < Math.Abs(deltaCourse)) || (interceptAngle < 30 && distanceFromLocalizer < 0.5))
                    {
                        this.heading.target = runwayHeading - interceptAngle * Radial.Direction(deltaPosition);
                    }
                    if (this.navMode == NavMode.APPROACH)
                    {
                        double glideslope = runway.elevation + 50 + Position.FT_IN_KM / Position.KM_TO_NM * Math.Tan(Position.DegreesToRadians(3)) * distanceFromRunway;
                        Console.WriteLine(glideslope);
                        if (distanceFromLocalizer < 0.5 && glideslope < this.altitude.value && this.altitude.value < glideslope + 500)
                        {
                            this.altitude.target = glideslope;
                        }
                    }
                    this.updateApproachAirspeed(distanceFromRunway);
                }
            }
            else if (this.navMode == NavMode.VISUAL)
            {
                double backcourse = this.runway.course + 180;
                Position finalApproachFix = this.runway.position.destinationPoint(backcourse, 7);
                double courseToFinal = this.position.courseTo(finalApproachFix);
                double deltaPosition = Radial.AngleBetween(courseToFinal, this.runway.course);
                double deltaPositionRunway = Radial.AngleBetween(this.position.courseTo(this.runway.position), this.runway.course);
                double distanceFromRunway = Math.Abs(Math.Cos(Position.DegreesToRadians(deltaPositionRunway))) * this.position.distanceTo(this.runway.position);
                double distanceFromLocalizer = Math.Abs(Math.Sin(Position.DegreesToRadians(deltaPositionRunway))) * this.position.distanceTo(this.runway.position);
                double minimumApproachDistance = this.altitude.value < this.runway.elevation + 3500 ? distanceFromRunway : (distanceFromRunway < 7 ? 7 - distanceFromRunway + 7 + distanceFromLocalizer : distanceFromRunway + distanceFromLocalizer);
                double altitude = runway.elevation + 50 + Position.FT_IN_KM / Position.KM_TO_NM * Math.Tan(Position.DegreesToRadians(3)) * minimumApproachDistance;
                if (altitude < this.altitude.value)
                {
                    this.altitude.target = altitude;
                }
                Position target;
                if (Math.Abs(deltaPosition) > 90)
                {
                    if (distanceFromLocalizer < Math.Max(2, distanceFromRunway / 7 * 3.5) && this.altitude.value < this.runway.elevation + 3500)
                    {
                        target = this.runway.position.destinationPoint(backcourse, distanceFromRunway - 1);
                    }
                    else
                    {
                        target = finalApproachFix.destinationPoint(this.runway.course + Radial.Direction(deltaPosition) * 90, 5);
                    }
                }
                else
                {
                    if (distanceFromLocalizer > 2)
                    {
                        target = finalApproachFix;
                    }
                    else
                    {
                        target = this.runway.position.destinationPoint(backcourse, distanceFromRunway - 2);
                    }
                }
                this.heading.target = this.position.courseTo(target) - this.position.magneticDeclination(this.altitude.value);
                this.updateApproachAirspeed(minimumApproachDistance);
            }
            else if (this.navMode == NavMode.PATH)
            {
                double legCourse = this.path.legCourse();
                Waypoint waypoint = this.path.legWaypoint();
                double course = (this.heading.target + this.position.magneticDeclination(this.altitude.value) + 360) % 360;
                double deltaCourse = Radial.AngleBetween(course, legCourse);

                double deltaPosition = Radial.AngleBetween(this.position.courseTo(waypoint.position), legCourse);
                double distanceFromRunway = Math.Cos(Position.DegreesToRadians(deltaPosition)) * this.position.distanceTo(waypoint.position);
                double distanceFromLocalizer = Math.Abs(Math.Sin(Position.DegreesToRadians(deltaPosition))) * this.position.distanceTo(waypoint.position);
                double runwayHeading = legCourse - waypoint.position.magneticDeclination(this.path.nextAltitude());
                double interceptAngle = Position.RadiansToDegrees(Math.Atan(distanceFromLocalizer * 500 / this.groundspeed(wx)));

                if (distanceFromRunway > 1)
                {
                    if ((Radial.Direction(deltaCourse) == Radial.Direction(deltaPosition) && interceptAngle < Math.Abs(deltaCourse)) || (interceptAngle < 30 && distanceFromLocalizer < 0.5))
                    {
                        this.heading.target = runwayHeading - interceptAngle * Radial.Direction(deltaPosition);
                    }
                    if (this.airspeed.target % 5 == 1)
                    {
                        if (this.path.nextAirspeed() != 0)
                        {
                            this.airspeed.target = this.path.nextAirspeed();
                        }
                    }
                    if (this.altitude.target % 100 == 1)
                    {
                        if (this.path.nextAltitude() != 0)
                        {
                            this.altitude.target = this.path.nextAltitude();
                        }
                    }
                }
                else
                {
                    this.path.activeWaypoint++;
                    if (this.path.activeWaypoint >= this.path.waypoints.Count)
                    {
                        this.navMode = NavMode.OFF;
                    }
                }
            }
            else
            {
                this.updateAutoAirspeed();
            }
        }

        public void updateApproachAirspeed(double distance)
        {
            if (distance < 5)
            {
                this.airspeed.target = 135;
            }
            else
            {
                this.updateAutoAirspeed();
            }
        }

        public void updateAutoAirspeed()
        {
            if (this.airspeed.target % 5 == 1)
            {
                double elevation = this.runway != null ? this.runway.elevation : 0;
                double airspeed = Math.Min(Math.Max(Math.Floor(17 + (this.altitude.value - elevation - 2500) / 3500.0 * 8) * 10, 170), 250) + 1;
                double target = this.airspeed.target % 5 == 0 ? this.airspeed.target + 1 : this.airspeed.target;
                this.airspeed.target = Math.Min(airspeed, target);
            }
        }

        public void resumeNormalSpeed()
        {
            if (this.airspeed.target % 5 == 0)
            {
                this.airspeed.target++;
            }
        }

        public void resumeNormalAltitude()
        {
            if (this.altitude.target % 100 == 0)
            {
                this.altitude.target++;
            }
        }

        public void turnLeftHeading(double heading)
        {
            this.navMode = NavMode.OFF;
            this.heading.direction = -1;
            this.heading.target = heading;
        }

        public void turnRightHeading(double heading)
        {
            this.navMode = NavMode.OFF;
            this.heading.direction = 1;
            this.heading.target = heading;
        }

        public void flyHeading(double heading)
        {
            this.navMode = NavMode.OFF;
            this.heading.direction = 0;
            this.heading.target = heading;
        }

        public void forceHeading(double heading)
        {
            this.navMode = NavMode.OFF;
            this.heading.direction = 0;
            this.heading.value = heading;
            this.heading.target = heading;
        }

        public void climbDescendMaintain(double altitude)
        {
            if (this.navMode == NavMode.APPROACH)
            {
                this.navMode = NavMode.LOCALIZER;
            }
            this.altitude.target = altitude < 1000 ? altitude * 100 : altitude;
        }

        public void forceAltitude(double altitude)
        {
            if (this.navMode == NavMode.APPROACH)
            {
                this.navMode = NavMode.LOCALIZER;
            }
            this.altitude.value = altitude < 1000 ? altitude * 100 : altitude;
            this.altitude.target = altitude < 1000 ? altitude * 100 : altitude;
        }

        public void forceAirspeed(double airspeed)
        {
            this.airspeed.value = airspeed;
            this.airspeed.target = airspeed;
        }

        public void engageLocalizerMode(Runway runway)
        {
            this.navMode = NavMode.LOCALIZER;
            this.heading.direction = 0;
            this.runway = runway;
        }

        public void engageApproachMode(Runway runway)
        {
            this.navMode = NavMode.APPROACH;
            this.heading.direction = 0;
            this.runway = runway;
            this.resumeNormalSpeed();
        }

        public void engageVisualApproach(Runway runway)
        {
            this.navMode = NavMode.VISUAL;
            this.heading.direction = 0;
            this.runway = runway;
            this.resumeNormalSpeed();
        }

        public void engagePathMode(Path path)
        {
            this.navMode = NavMode.PATH;
            this.heading.direction = 0;
            this.path = path;
            this.resumeNormalSpeed();
            this.resumeNormalAltitude();
        }

        public void snapToPath(Path path)
        {
            this.path = path;
            double nextAltitude = path.nextAltitude();
            double nextAirspeed = path.nextAirspeed();
            double heading = (path.legCourse() - this.position.magneticDeclination(nextAltitude)
                + 360) % 360;

            this.position = path.activePosition;
            this.forceHeading(heading);
            if (nextAltitude != 0)
            {
                this.forceAltitude(nextAltitude);
            }
            if (nextAirspeed != 0)
            {
                this.forceAirspeed(nextAirspeed);
            }
            this.navMode = NavMode.PATH;
        }

        public double tas()
        {
            return this.altitude.value / 200 + this.airspeed.value;
        }

        public double groundspeed(WeatherSituation wx)
        {
            Wind wind = wx.windAtPosition(this.position);
            double tas = this.tas();
            double windRadians = Position.DegreesToRadians(wind.direction.value);
            double headingRadians = Position.DegreesToRadians(this.heading.value);
            return Math.Round(Math.Sqrt(Math.Pow(wind.velocity.value, 2) +
                   Math.Pow(tas, 2) - 2 * wind.velocity.value *
                   tas * Math.Cos(headingRadians - windRadians)));
        }

        public void updatePosition(WeatherSituation wx, double milliseconds)
        {
            Wind wind = wx.windAtPosition(this.position);
            double tas = this.tas();
            double windRadians = Position.DegreesToRadians(wind.direction.value);
            double headingRadians = Position.DegreesToRadians(this.heading.value);
            double groundspeed = Math.Round(Math.Sqrt(Math.Pow(wind.velocity.value, 2) +
                                 Math.Pow(tas, 2) - 2 * wind.velocity.value *
                                 tas * Math.Cos(headingRadians - windRadians)));
            double windCorrection = Math.Atan2(wind.velocity.value * Math.Sin(headingRadians - windRadians),
                                    tas - wind.velocity.value *
                                    Math.Cos(headingRadians - windRadians));
            double course = Position.RadiansToDegrees(headingRadians + windCorrection) % 360;
            double distance = groundspeed * milliseconds / Position.MS_IN_HR;
            this.position = this.position.destinationPoint(course + this.position.magneticDeclination(this.altitude.value), distance);
        }
    }
}
