
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
    public class RouteCatalog : Grain, IRouteCatalog
    {
        public Task<IEnumerable<IRoute>> GetRoutes()
        {
            return Task.FromResult(StaticData.Routes.Select(ri => GrainFactory.GetGrain<IRoute>(ri.Id)).ToArray().AsEnumerable());
        }
    }
}
