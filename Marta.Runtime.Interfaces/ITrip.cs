
using Marta.Common;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface ITrip : IGrainWithIntegerKey
    {
        Task<Tuple<StopTime, StopTime>> GetStopTimes(BusStatus status);
    }
}
