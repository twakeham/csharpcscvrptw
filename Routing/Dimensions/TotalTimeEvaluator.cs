using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing.Dimensions {

    /// <summary>
    /// Travel time evaluator
    /// </summary>
    class TotalTimeEvaluator : Evaluator {

        // 60 kmh-1 in ms-1
        private const long speed = 17 ;
        private DistanceMatrix _matrix;

        public TotalTimeEvaluator (Location[] locations, DistanceMatrix matrix) : base(locations) {
            _matrix = matrix;
        }

        public override long Run (int origin, int destination) {
            long travelTime = _matrix.Distance(origin, destination) / speed;
            return _locations[origin].ServiceTime + travelTime;
        }
    }

}
