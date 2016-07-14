using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class Radial
    {
        public static double AngleBetween(double a, double b)
        {
            double remaining = (b - a + 360) % 360;
            if (Math.Abs(remaining - 360) < remaining)
                remaining -= 360;
            return remaining;
        }

        public static int Direction(double angle)
        {
            return angle >= 0 ? 1 : -1;
        }

        public static int Direction(double a, double b)
        {
            return Radial.Direction(Radial.AngleBetween(a, b));
        }
    }
}
