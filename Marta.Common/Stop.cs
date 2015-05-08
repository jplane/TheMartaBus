using System.Collections.Generic;

namespace Marta.Common
{
    public class Stop
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }

        public IEnumerable<StopTime> StopTimes
        {
            get { return StaticDataLoader.GetStopTimesByStopId(this.Id); }
        }
    }
}