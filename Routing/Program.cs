using System;
using System.Collections.Generic;
using Velyo.Google.Services;

namespace Routing {
    class Program {

        /// <summary>
        /// Turns and address into lat/lng with google web services
        /// </summary>
        static private bool GeocodeAddress(string address, ref Location location) {
            GeocodingRequest request = new GeocodingRequest(address);
            GeocodingResponse response = request.GetResponse();

            if (response.Status != GeocodingResponseStatus.OK)
                return false;
            
            location.X = response.Results?[0].Geometry.Location.Latitude ?? 0.0;
            location.Y = response.Results?[0].Geometry.Location.Longitude ?? 0.0;

            return true;
        }

        static void Main (string[] args) {

            string[] addresses = new string[30] {
                "341 THYNNE ROAD, MORNINGSIDE, QLD, 4170, AUST",
                "LEVEL 6 229 ELIZABETH STREET, BRISBANE, QLD, 4000, AUST",
                "266 GEORGE STREET, BRISBANE, QLD, 4000, AUST",
                "BIRDWOOD TERRACE, TOOWONG, QLD, 4066, AUST",
                "58 GLEN ROSA ROAD, RED HILL, QLD, 4059, AUST",
                "Brisbane Airport (BNE), 11 The Circuit, Brisbane Airport QLD 4008, Australia",
                "2/34 Manton Street, Morningside QLD 4170, Australia",
                "341 THYNNE ROAD, MORNINGSIDE, QLD, 4170, AUST",
                "52 QANTAS DRIVE, EAGLE FARM, QLD, 4009, AUST",
                "23 Qantas Dr, Brisbane Airport QLD 4009, Australia",
                "AIRPORT DRIVE, BRISBANE AIRPORT, QLD, 4007, AUST",
                "1 - 5 THE CIRCUIT, BRISBANE AIRPORT, QLD, 4009, AUST",
                "1-5 THE CIRCUIT, BRISBANE AIRPORT, QLD, 4009, AUST",
                "11 THE CIRCUIT, BRISBANE AIRPORT, QLD, 4009, AUST",
                "131 QUEENS ROAD, NUDGEE, QLD, 4014, AUST",
                "58 YARRAMAN PLACE, VIRGINIA, QLD, 4014, AUST",
                "87 YARRAMAN PLACE, VIRGINIA, QLD, 4014, AUST",
                "210 ROBINSON ROAD, VIRGINIA, QLD, 4034, AUST",
                "304 ROGHAN ROAD, TAIGUM, QLD, 4018, AUST",
                "333 HANDFORD ROAD, TAIGUM, QLD, 4018, AUST",
                "396 Beams Rd, Zillmere QLD 4018, Australia",
                "42 RIDLEY ROAD, BRIDGEMAN DOWNS, QLD, 4035, AUST",
                "1A BYTH STREET, STAFFORD, QLD, 4053, AUST",
                "LLOYD STREET, ENOGGERA, QLD, 4051, AUST",
                "341 Thynne Road, Morningside QLD 4170, Australia",
                "JUNCTION ROAD, CANNON HILL, QLD, 4170, AUST",
                "OAKLANDS PARADE, EAST BRISBANE, QLD, 4169, AUST",
                "691 LOGAN ROAD, GREENSLOPES, QLD, 4120, AUST",
                "PEACH STREET & DENMAN STREET, GREENSLOPES, QLD, 4120, AUST",
                "19 DIVIDEND STREET, MANSFIELD, QLD, 4122, AUST"
            };

            // time window starts in seconds from midnight
            long[] startTimeWindow = new long[30] {
                21600, 21600, 21600, 21600, 21600, 21600, 21600, 21600, 21600, 21600, 21600,
                46800, 21600, 21600, 25200, 39600, 43200, 25200, 25200, 25200, 21600, 21600,
                21600, 21600, 21600, 21600, 25200, 25200, 21600, 25200
            };

            // time window end in seconds from midnight
            long[] endTimeWindow = new long[30] {
                61200, 61200, 61200, 57600, 61200, 61200, 57600, 25200, 46800, 61200, 61200,
                61200, 61200, 61200, 54000, 46800, 54000, 54000, 50400, 50400, 61200, 61200,
                61200, 61200, 57600, 28800, 43200, 39600, 28800, 39600
            };

            // capacity each job takes (demand)
            long[] capacity = new long[30] {
                15, 1, 3, 2, 11, 3, 26, 4, 6, 1, 1, 1, 1, 5, 6, 3, 1, 2, 2, 4, 5, 9, 11, 14, 21, 1, 1, 2, 8, 4
            };

            // iterate through addresses and generate a location
            List<Location> locations = new List<Location>();

            for (int index = 0; index < 10; index ++) {
                Location location = new Location();

                Console.Write("Geocoding address: " + addresses[index]);

                if (!GeocodeAddress(addresses[index], ref location)) {
                    Console.Write(" FAILED!\n");
                    continue;
                }
                Console.Write(string.Format(" - ({0}, {1})\n", location.X, location.Y));

                location.Demand = capacity[index];
                location.StartTime = startTimeWindow[index];
                location.EndTime = endTimeWindow[index];

                locations.Add(location);

            }

            // create vehicles
            Vehicle[] vehicles = new Vehicle[10];
            for (int index = 0; index < vehicles.Length; index ++) {
                vehicles[index] = new Vehicle();
            }

            Location[] locationsArray = locations.ToArray();

            // setup solver
            DistanceMatrix distanceMatrix = new MetricDistanceMatrix(locationsArray, Metrics.HaversineDistance);
            distanceMatrix.Populate();

            Solver solver = new Solver(locationsArray, vehicles);

            Console.Write("Solving - ");
            if (solver.Solve(distanceMatrix)) {
                Console.Write("SUCCESS!\n");
            } else {
                Console.Write("NO SOLUTIONS!\n");
            }
        }
    }
}
