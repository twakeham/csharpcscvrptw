using Google.OrTools.ConstraintSolver;

namespace Routing.Dimensions {

    class Evaluator : NodeEvaluator2 {
        protected Location[] _locations;

        public Evaluator (Location[] locations) {
            _locations = locations;
        }
    }

}
