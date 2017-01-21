using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing {
    public class Location {

        public double X { get; set; }
        public double Y { get; set; }
        public long Demand { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long ServiceTime {
            get {
                return 1200;
            }
        }
    }
}
