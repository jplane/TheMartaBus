
using Marta.Common;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IStop : IGrainWithIntegerKey
    {
        Task<StopInfo> GetInfo();

        Task IsApproaching(IBus bus, TimeSpan delta);
        Task IsNoLongerApproaching(IBus bus);
        Task HasDeparted(IBus bus, TimeSpan delta);
        Task NoLongerDeparted(IBus bus);

        Task<IEnumerable<IBus>> GetApproachingBuses();
        Task<IEnumerable<IBus>> GetDepartedBuses();
    }
}
