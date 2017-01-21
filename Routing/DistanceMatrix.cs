using System;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;


namespace Routing {
    public abstract class DistanceMatrix : NodeEvaluator2 {

        protected Dictionary<int, Dictionary<int, long>> _matrix;
        protected Location[] _locations;

        public DistanceMatrix(Location[] locations) {
            _locations = locations;
            _matrix = new Dictionary<int, Dictionary<int, long>>();
        }

        public abstract bool Populate ();

        public abstract long Distance (int origin, int destination);

    }

    public class MetricDistanceMatrix : DistanceMatrix {

        /// <summary>
        /// Distance matrix based on a supplied distance metric
        /// </summary>

        private Func<Location, Location, double> _metric;

        public MetricDistanceMatrix(Location[] locations, Func<Location, Location, double> metric) : base(locations) {
            _metric = metric;
        }

        public override bool Populate() {
            for (int origin = 0; origin < _locations.Length; origin ++) {
                _matrix[origin] = new Dictionary<int, long>();
                for (int destination = 0; destination < _locations.Length; destination++) {
                    _matrix[origin][destination] = origin == destination ? 0 : (long) _metric(_locations[origin], _locations[destination]);
                }
            }
            return true;
        }

        public override long Distance (int origin, int destination) {
            return _matrix[origin][destination];
        }

        public override long Run (int x, int y) {
            return _matrix[x][y];
        }

    }

}
