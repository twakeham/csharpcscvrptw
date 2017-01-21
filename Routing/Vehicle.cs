using System;
using System.Collections.Generic;


namespace Routing {

    public struct Delivery {
        public Location location;
        public long startTime;
        public long endTime;
    }

    class Vehicle {

        public List<Delivery> Deliveries { get; set; }
        public long TotalDistanceTravelled { get; set; }
        public long TotalCapacity { get; set; }

        public Vehicle() {
            Deliveries = new List<Delivery>();
        }
        
    }
}
