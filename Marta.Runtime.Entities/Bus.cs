
using Marta.Runtime.Interfaces;
using Marta.Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure;
using Orleans;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Marta.Runtime.Entities
{
    public class Bus : Grain, IBus
    {
        private readonly List<BusSnapshotInfo> _snapshots = new List<BusSnapshotInfo>();

        private IStop _lastStop = null;
        private IStop _nextStop = null;
        private IHubProxy _hub = null;

        public Task<BusInfo> GetInfo()
        {
            return Task.FromResult((BusInfo)_snapshots.LastOrDefault());
        }

        public async Task UpdateStatus(BusSnapshotInfo snapshot)
        {
            _snapshots.Add(snapshot);

            await UpdateMap(snapshot);
            await UpdateStops(snapshot);
        }

        private async Task UpdateStops(BusSnapshotInfo status)
        {
            var trip = GrainFactory.GetGrain<ITrip>(status.TripId);

            var stoptimes = await trip.GetStopTimes(status);

            IStop last = stoptimes.Item1 == null ? null : GrainFactory.GetGrain<IStop>(stoptimes.Item1.StopId);

            if (_lastStop != null)
            {
                if (last == null || stoptimes.Item1.StopId != (int) _lastStop.GetPrimaryKeyLong())
                {
                    await _lastStop.NoLongerDeparted(this);
                }
            }

            _lastStop = last;

            if (_lastStop != null)
            {
                await _lastStop.HasDeparted(this, status.AdjustedTimestamp.Subtract(stoptimes.Item1.Departure));
            }

            IStop next = stoptimes.Item2 == null ? null : GrainFactory.GetGrain<IStop>(stoptimes.Item2.StopId);

            if (_nextStop != null)
            {
                if (next == null || stoptimes.Item2.StopId != (int) _nextStop.GetPrimaryKeyLong())
                {
                    await _nextStop.IsNoLongerApproaching(this);
                }
            }

            _nextStop = next;

            if (_nextStop != null)
            {
                await _nextStop.IsApproaching(this, stoptimes.Item2.Arrival.Subtract(status.AdjustedTimestamp));
            }
        }

        private async Task UpdateMap(BusSnapshotInfo status)
        {
            if (_hub == null)
            {
                var signalrUri = CloudConfigurationManager.GetSetting("signalrUri");

                var conn = new HubConnection(signalrUri);

                _hub = conn.CreateHubProxy("MapHub");

                await conn.Start();
            }

            await _hub.Invoke("UpdateBus", status);
        }
    }
}
