
using Marta.Runtime.Interfaces;
using Marta.Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure;
using Orleans;
using System.Threading.Tasks;

namespace Marta.Runtime.Entities
{
    public class Bus : Grain, IBus
    {
        private int _tripId;
        private double _latitude;
        private double _longitude;
        private IStop _lastStop = null;
        private IStop _nextStop = null;
        private IHubProxy _hub = null;

        public async Task UpdateStatus(BusStatus status)
        {
            _tripId = status.TripId;
            _latitude = status.Latitude;
            _longitude = status.Longitude;

            await UpdateMap(status);
            await UpdateStops(status);
        }

        private async Task UpdateStops(BusStatus status)
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

        private async Task UpdateMap(BusStatus status)
        {
            if (_hub == null)
            {
                var signalrUri = CloudConfigurationManager.GetSetting("signalrUri");

                var conn = new HubConnection(signalrUri);

                _hub = conn.CreateHubProxy("MapHub");

                await conn.Start();
            }

            await _hub.Invoke("UpdateBusStatus", status);
        }
    }
}
