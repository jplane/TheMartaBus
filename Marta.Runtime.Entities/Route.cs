
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orleans;
using Marta.Runtime.Interfaces;
using System.Threading.Tasks;
using Marta.Common;

namespace Marta.Runtime.Entities
{
    public class Route : Grain, IRoute
    {
        public Task<RouteInfo> GetInfo()
        {
            return Task.FromResult(StaticData.GetRouteById((int) this.GetPrimaryKeyLong()));
        }

        public Task<IEnumerable<IStop>> GetStops()
        {
            var stops = StaticData.GetStopsByRouteId((int)this.GetPrimaryKeyLong())
                                  .Select(si => GrainFactory.GetGrain<IStop>(si.Id))
                                  .ToArray()
                                  .AsEnumerable();

            return Task.FromResult(stops);
        }
    }
}
