
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marta.Common
{
    public static class StaticDataLoader
    {
        public static Route GetRouteById(int id)
        {
            Route route = null;

            _routes.Value.TryGetValue(id, out route);

            return route;
        }

        public static IEnumerable<Route> Routes
        {
            get { return _routes.Value.Values; }
        }

        private static Lazy<Dictionary<int, Route>> _routes = new Lazy<Dictionary<int, Route>>(() =>
        {
            return GetStaticData<Route>(StaticDataType.Routes, line =>
            {
                var elements = line.Split(',');

                return new Route
                {
                    Id = int.Parse(elements[0]),
                    Name = elements[2]
                };
            })
            .ToDictionary(r => r.Id);
        });

        public static Trip GetTripById(int id)
        {
            Trip trip = null;

            _trips.Value.TryGetValue(id, out trip);

            return trip;
        }

        public static IEnumerable<Trip> Trips
        {
            get { return _trips.Value.Values; }
        }

        private static Lazy<Dictionary<int, Trip>> _trips = new Lazy<Dictionary<int, Trip>>(() =>
        {
            return GetStaticData<Trip>(StaticDataType.Trips, line =>
            {
                var elements = line.Split(',');

                return new Trip
                {
                    RouteId = int.Parse(elements[0]),
                    Id = int.Parse(elements[2]),
                    ShapeId = int.Parse(elements[6]),
                    Headsign = elements[3]
                };
            })
            .ToDictionary(t => t.Id);
        });

        public static Stop GetStopById(int id)
        {
            Stop stop = null;

            _stops.Value.TryGetValue(id, out stop);

            return stop;
        }

        public static IEnumerable<Stop> Stops
        {
            get { return _stops.Value.Values; }
        }

        private static Lazy<Dictionary<int, Stop>> _stops = new Lazy<Dictionary<int, Stop>>(() =>
        {
            return GetStaticData<Stop>(StaticDataType.Stops, line =>
            {
                var elements = line.Split(',');

                return new Stop
                {
                    Id = int.Parse(elements[0]),
                    Latitude = double.Parse(elements[3]),
                    Longitude = double.Parse(elements[4]),
                    Name = elements[2]
                };
            })
            .ToDictionary(s => s.Id);
        });

        public static IEnumerable<StopTime> GetStopTimesByStopId(int stopId)
        {
            StopTime[] times = null;

            _stoptimes.Value.Item1.TryGetValue(stopId, out times);

            return times ?? Enumerable.Empty<StopTime>();
        }

        public static IEnumerable<StopTime> GetStopTimesByTripId(int tripId)
        {
            StopTime[] times = null;

            _stoptimes.Value.Item2.TryGetValue(tripId, out times);

            return times ?? Enumerable.Empty<StopTime>();
        }

        private static Lazy<Tuple<Dictionary<int, StopTime[]>, Dictionary<int, StopTime[]>>> _stoptimes =
            new Lazy<Tuple<Dictionary<int, StopTime[]>, Dictionary<int, StopTime[]>>>(() =>
        {
            var stoptimes = GetStaticData<StopTime>(StaticDataType.StopTimes, line =>
            {
                var elements = line.Split(',');

                return new StopTime
                {
                    TripId = int.Parse(elements[0]),
                    Arrival = new TimeSpan(int.Parse(elements[1].Split(':')[0]), int.Parse(elements[1].Split(':')[1]), int.Parse(elements[1].Split(':')[2])),
                    Departure = new TimeSpan(int.Parse(elements[2].Split(':')[0]), int.Parse(elements[2].Split(':')[1]), int.Parse(elements[2].Split(':')[2])),
                    StopId = int.Parse(elements[3]),
                    Sequence = int.Parse(elements[4])
                };
            })
            .ToArray();

            return Tuple.Create(
                stoptimes.GroupBy(st => st.StopId).ToDictionary(g => g.Key, g => g.ToArray()),
                stoptimes.GroupBy(st => st.TripId).ToDictionary(g => g.Key, g => g.ToArray()));
        });

        public static Shape GetShapeById(int id)
        {
            Shape shape = null;

            _shapes.Value.TryGetValue(id, out shape);

            return shape;
        }

        public static IEnumerable<Shape> Shapes
        {
            get { return _shapes.Value.Values; }
        }

        private static Lazy<Dictionary<int, Shape>> _shapes = new Lazy<Dictionary<int, Shape>>(() =>
        {
            return GetStaticData<Shape>(StaticDataType.Shapes, line =>
            {
                var elements = line.Split(',');

                return new Shape
                {
                    Id = int.Parse(elements[0]),
                    Latitude = double.Parse(elements[1]),
                    Longitude = double.Parse(elements[2]),
                    Sequence = int.Parse(elements[3])
                };
            })
            .ToDictionary(s => s.Id);
        });

        private static IEnumerable<T> GetStaticData<T>(StaticDataType type, Func<string, T> factory)
        {
            using (var strm = LoadCsvStream(type))
            using (var reader = new StreamReader(strm))
            {
                reader.ReadLine();  // skip header

                var line = reader.ReadLine();
                
                while(! string.IsNullOrWhiteSpace(line))
                {
                    yield return factory(line);

                    line = reader.ReadLine();
                }
            }
        }

        private static Stream LoadCsvStream(StaticDataType type)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            var resourceName = string.Format("Marta.Common.static_data.{0}.txt", type.ToString().ToLower());

            return assembly.GetManifestResourceStream(resourceName);
        }
    }

    internal enum StaticDataType
    {
        Trips,
        Routes,
        Stops,
        StopTimes,
        Shapes
    }
}
