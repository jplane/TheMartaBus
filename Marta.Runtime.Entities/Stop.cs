
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

        public Task<StopInfo> GetInfo()
        {
            return Task.FromResult(StaticData.GetStopById((int)this.GetPrimaryKeyLong()));
        }

        private async Task Init()
        {
            if(_approaching == null)
            {
                _approaching = new Dictionary<int, TimeSpan>();
            }

            if (_departed == null)
            {
                _departed = new Dictionary<int, TimeSpan>();
            }

            if (_hub == null)
            {
                var signalrUri = CloudConfigurationManager.GetSetting("signalrUri");
            
                var conn = new HubConnection(signalrUri);

                _hub = conn.CreateHubProxy("MapHub");

                await conn.Start();
            }
        }

        public async Task IsApproaching(IBus bus, TimeSpan delta)
        {
            await Init();

            var busId = (int)bus.GetPrimaryKeyLong();

            _approaching[busId] = delta;

            await _hub.Invoke("BusApproachingStop", await this.GetInfo(), busId, delta);
        }

        public async Task IsNoLongerApproaching(IBus bus)
        {
            await Init();

            var busId = (int)bus.GetPrimaryKeyLong();

            _approaching.Remove(busId);

            await _hub.Invoke("BusNoLongerApproachingStop", await this.GetInfo(), busId);
        }

        public async Task HasDeparted(IBus bus, TimeSpan delta)
        {
            await Init();

            var busId = (int)bus.GetPrimaryKeyLong();

            _departed[busId] = delta;

            await _hub.Invoke("BusHasDepartedStop", await this.GetInfo(), busId, delta);
        }

        public async Task NoLongerDeparted(IBus bus)
        {
            await Init();

            var busId = (int)bus.GetPrimaryKeyLong();

            _departed.Remove(busId);

            await _hub.Invoke("BusNoLongerDepartedStop", await this.GetInfo(), busId);
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
