
using Marta.Common;
using Marta.Runtime.Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.Runtime.Entities
{
    public class Trip : Grain, ITrip
    {
        private LinkedList<StopTimeInfo> _stoptimes = null;

        public Task<TripInfo> GetInfo()
        {
            return Task.FromResult(StaticData.GetTripById((int)this.GetPrimaryKeyLong()));
        }

        public Task<Tuple<StopTimeInfo, StopTimeInfo>> GetStopTimes(BusSnapshotInfo status)
        {
            if (_stoptimes == null)
            {
                var times = StaticData.GetStopTimesByTripId((int)this.GetPrimaryKeyLong());
                _stoptimes = new LinkedList<StopTimeInfo>(times.OrderBy(st => st.Sequence));
            }

            StopTimeInfo last = null;
            StopTimeInfo next = null;

            GetStopTimes(status, out last, out next);

            return Task.FromResult(Tuple.Create(last, next));
        }

        private void GetStopTimes(BusSnapshotInfo status, out StopTimeInfo last, out StopTimeInfo next)
        {
            last = next = null;

            if (_stoptimes == null)
            {
                return;
            }

            var busTime = status.AdjustedTimestamp;

            LinkedListNode<StopTimeInfo> node = _stoptimes.First;

            while(node != null)
            {
                if(busTime < node.Value.Arrival)
                {
                    next = node.Value;
                    break;
                }

                last = node.Value;
                node = node.Next;
            }
        }
    }
}
