
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
        private LinkedList<StopTime> _stoptimes = null;

        public Task<Tuple<StopTime, StopTime>> GetStopTimes(BusStatus status)
        {
            if (_stoptimes == null)
            {
                var trip = StaticDataLoader.GetTripById((int)this.GetPrimaryKeyLong());

                if (trip != null)
                {
                    _stoptimes = new LinkedList<StopTime>(trip.StopTimes.OrderBy(st => st.Sequence));
                }
            }

            StopTime last = null;
            StopTime next = null;

            GetStopTimes(status, out last, out next);

            return Task.FromResult(Tuple.Create(last, next));
        }

        private void GetStopTimes(BusStatus status, out StopTime last, out StopTime next)
        {
            last = next = null;

            if (_stoptimes == null)
            {
                return;
            }

            var busTime = status.AdjustedTimestamp;

            LinkedListNode<StopTime> node = _stoptimes.First;

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
