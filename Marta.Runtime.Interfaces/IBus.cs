
using Marta.Common;
using Orleans;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IBus : IGrainWithIntegerKey
    {
        Task UpdateStatus(BusStatus snapshot);
    }
}
