
using Marta.Common;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface ITrip : IGrainWithIntegerKey
    {
        Task<TripInfo> GetInfo();

        Task<Tuple<StopTimeInfo, StopTimeInfo>> GetStopTimes(BusSnapshotInfo status);
    }
}
