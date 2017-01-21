using System;
using System.Collections.Generic;

using Google.OrTools.ConstraintSolver;

using Routing.Dimensions;


namespace Routing {

    class Solver {
        /// <summary>
        /// Vehicle routing solver using Google OR Tools and constraint programming.
        /// </summary>
        
        // limit deliveries to today - number of seconds in one day
        private const long _timeHorizon = 86400;

        private Location[] _locations;
        private Vehicle[] _vehicles;

        public Solver(Location[] locations, Vehicle[] vehicles) {
            _locations = locations;
            _vehicles = vehicles;
        }

        public bool Solve(DistanceMatrix distanceMatrix) {
            const long vehicleCapacity = 30;

            // or-tools solver
            RoutingModel routingModel = new RoutingModel(_locations.Length, _vehicles.Length);

            // set solution strategy to cheapest arc length
            RoutingSearchParameters search_params = RoutingModel.DefaultSearchParameters();
            search_params.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // distance matrix for cost of transit
            routingModel.SetArcCostEvaluatorOfAllVehicles(distanceMatrix);

            // constrain demand so that we don't overload vehicles
            DemandEvaluator demandEvaluator = new DemandEvaluator(_locations);
            routingModel.AddDimension(demandEvaluator, slack_max: 0, capacity: vehicleCapacity, fix_start_cumul_to_zero: true, name: "demand");
            RoutingDimension demandDimension = routingModel.GetDimensionOrDie("demand");

            // constrain job start and end times to today
            TotalTimeEvaluator totalTimeEvaluator = new TotalTimeEvaluator(_locations, distanceMatrix);
            routingModel.AddDimension(totalTimeEvaluator, slack_max: _timeHorizon, capacity: _timeHorizon, fix_start_cumul_to_zero: true, name: "time");
            RoutingDimension totalTimeDimension = routingModel.GetDimensionOrDie("time");

            // depot is represented by locations[0] so there is no need to add constraints to start and end time 
            // for this location.  for other locations, set range of times to be between the start and end timeframe
            // 
            for (int index = 1; index < _locations.Length; index ++) {
                totalTimeDimension.CumulVar(index).SetRange(_locations[index].StartTime, _locations[index].EndTime);
            }

            Assignment assignment = routingModel.SolveWithParameters(search_params);

            if (assignment == null)
                return false;

            // valid solutions found - populate vehicle runs
            for (int vehicleIndex = 0; vehicleIndex < _vehicles.Length; vehicleIndex ++) {

                long index = routingModel.Start(vehicleIndex);
                Vehicle vehicle = _vehicles[vehicleIndex];

                while(!routingModel.IsEnd(index)) {
                    Delivery delivery;
                    long locationIndex = routingModel.IndexToNode(index);
                    delivery.location = _locations[locationIndex];
                    
                    IntVar demand = demandDimension.CumulVar(index);
                    IntVar timeframe = totalTimeDimension.CumulVar(index);

                    delivery.startTime = assignment.Min(timeframe);
                    delivery.endTime = assignment.Max(timeframe);

                    vehicle.Deliveries.Add(delivery);

                }
            }

            return true;
        }

    }
}
