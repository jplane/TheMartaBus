
using Marta.Common;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IRouteCatalog : IGrainWithIntegerKey
    {
        Task<IEnumerable<IRoute>> GetRoutes();
    }
}
