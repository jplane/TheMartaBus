
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.Runtime.Entities
{
    public class Stop : Grain, IStop
    {
        private Dictionary<int, TimeSpan> _approaching = null;
        private Dictionary<int, TimeSpan> _departed = null;
        private IHubProxy _hub = null;

        private async Task UpdateMap()
        {
            if (_hub == null)
            {
                var signalrUri = CloudConfigurationManager.GetSetting("signalrUri");
            
                var conn = new HubConnection(signalrUri);

                _hub = conn.CreateHubProxy("MapHub");

                await conn.Start();
            }

            await _hub.Invoke("UpdateStopStatus",
                              (int) this.GetPrimaryKeyLong(),
                              _approaching.Select(pair => Tuple.Create(pair.Key, pair.Value)).ToArray(),
                              _departed.Select(pair => Tuple.Create(pair.Key, pair.Value)).ToArray());
        }

        public Task IsApproaching(IBus bus, TimeSpan delta)
        {
            if(_approaching == null)
            {
                _approaching = new Dictionary<int, TimeSpan>();
            }

            if (_departed == null)
            {
                _departed = new Dictionary<int, TimeSpan>();
            }

            _approaching[(int) bus.GetPrimaryKeyLong()] = delta;

            return UpdateMap();
        }

        public Task IsNoLongerApproaching(IBus bus)
        {
            if (_approaching != null)
            {
                _approaching.Remove((int)bus.GetPrimaryKeyLong());
                return UpdateMap();
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        public Task HasDeparted(IBus bus, TimeSpan delta)
        {
            if (_approaching == null)
            {
                _approaching = new Dictionary<int, TimeSpan>();
            }

            if (_departed == null)
            {
                _departed = new Dictionary<int, TimeSpan>();
            }

            _departed[(int) bus.GetPrimaryKeyLong()] = delta;

            return UpdateMap();
        }

        public Task NoLongerDeparted(IBus bus)
        {
            if (_departed != null)
            {
                _departed.Remove((int)bus.GetPrimaryKeyLong());
                return UpdateMap();
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        public Task<IEnumerable<IBus>> GetApproachingBuses()
        {
            if (_approaching != null)
            {
                return Task.FromResult((IEnumerable<IBus>)_approaching.Keys.Select(id => GrainFactory.GetGrain<IBus>(id)).ToArray());
            }
            else
            {
                return Task.FromResult(Enumerable.Empty<IBus>());
            }
        }

        public Task<IEnumerable<IBus>> GetDepartedBuses()
        {
            if(_departed != null)
            { 
            return Task.FromResult((IEnumerable<IBus>) _departed.Keys.Select(id => GrainFactory.GetGrain<IBus>(id)).ToArray());
            }
            else
            {
                return Task.FromResult(Enumerable.Empty<IBus>());
            }
        }
    }
}
