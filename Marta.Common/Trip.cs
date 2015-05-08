using System.Collections.Generic;

namespace Marta.Common
{
    public class Trip
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int ShapeId { get; set; }
        public string Headsign { get; set; }

        public Route Route
        {
            get { return StaticDataLoader.GetRouteById(this.RouteId); }
        }

        public Shape Shape
        {
            get { return StaticDataLoader.GetShapeById(this.ShapeId); }
        }

        public IEnumerable<StopTime> StopTimes
        {
            get { return StaticDataLoader.GetStopTimesByTripId(this.Id); }
        }
    }
}