using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing {
    public static class Metrics {
        
        public static double ManahattanDistance (Location loc1, Location loc2) {
            /// Square distance - deltaX + deltaY - more accurately models road
            /// distance in cities.
            return Math.Abs(loc2.X - loc1.X) + Math.Abs(loc2.Y - loc1.Y);
        }

        public static double EuclideanDistance(Location loc1, Location loc2) {
            /// Pythagorean distance - actual as-the-bird flies distance.  Not
            /// great for road transport.
            double dx = loc2.X - loc1.X;
            double dy = loc2.Y - loc1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double Radians(double degrees) {
            return degrees * Math.PI / 180;
        }

        private static double Haversine(double angle) {
            return Math.Pow(Math.Sin(angle / 2d), 2d);
        }

        public static double HaversineDistance(Location loc1, Location loc2) {
            /// Distance between two sets of lat/lng coordinates on Earth.
            double radius = 3959.0;

            // latitude
            double[] phi = new double[2] { Radians(loc1.X), Radians(loc2.X) };
            double deltaPhi = phi[1] - phi[0];

            // longitude
            double[] lambda = new double[2] { Radians(loc1.Y), Radians(loc2.Y) };
            double deltaLambda = lambda[1] - lambda[0];

            double a = Haversine(deltaPhi) + Math.Cos(phi[0]) * Math.Cos(phi[1]) + Haversine(deltaLambda);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return c * radius;            
        }

    }
}
