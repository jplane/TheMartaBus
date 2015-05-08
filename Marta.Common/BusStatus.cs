
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.Common
{
    public class BusStatus
    {
        public int TripId { get; set; }
        public int VehicleId { get; set; }
        public int RouteId { get; set; }
        public int NextStopId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Direction DirectionOfTravel { get; set; }
        public Timeliness Timeliness { get; set; }
        public int TimelinessOffset { get; set; }

        public TimeSpan AdjustedTimestamp
        {
            get { return this.Timestamp.ToLocalTime().TimeOfDay.Add(TimeSpan.FromMinutes(this.TimelinessOffset)); }
        }
    }

    public enum Timeliness
    {
        OnTime = 1,
        Early,
        Late
    }

    public enum Direction
    {
        North = 1,
        South,
        East,
        West
    }
}
