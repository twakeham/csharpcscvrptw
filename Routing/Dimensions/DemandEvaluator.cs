using Google.OrTools.ConstraintSolver;


namespace Routing.Dimensions {

    class DemandEvaluator : Evaluator {

        public DemandEvaluator (Location[] locations) : base(locations) { }

        public override long Run (int origin, int destination) {
            return _locations[origin].Demand;
        }
    }

}
