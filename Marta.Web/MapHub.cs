
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Marta.Common;
using Orleans;
using Marta.Runtime.Interfaces;

namespace Marta.Web
{
    public class MapHub : Hub
    {
        private static object _lock = new object();
        private static bool _orleansInitialized = false;

        private static void Init()
        {
            lock(_lock)
            {
                if(!_orleansInitialized)
                {
                    GrainClient.Initialize(HttpContext.Current.Server.MapPath("~/ClientConfiguration.xml"));
                    _orleansInitialized = true;
                }
            }
        }

        public Task RegisterMapView()
        {
            return Groups.Add(Context.ConnectionId, "mapViews");
        }
        
        public async Task<IEnumerable<RouteInfo>> GetRoutes()
        {
            Init();

            var catalog = GrainFactory.GetGrain<IRouteCatalog>(1);

            var infos = new List<RouteInfo>();

            foreach(var route in await catalog.GetRoutes())
            {
                infos.Add(await route.GetInfo());
            }

            return infos;
        }

        public async Task<IEnumerable<StopInfo>> GetStopsForRoute(int routeId)
        {
            Init();

            var route = GrainFactory.GetGrain<IRoute>(routeId);

            var infos = new List<StopInfo>();

            foreach(var stop in await route.GetStops())
            {
                infos.Add(await stop.GetInfo());
            }

            return infos;
        }

        public Task BusApproachingStop(StopInfo stop, int vehicleId, TimeSpan delta)
        {
            return Task.FromResult(0);
        }

        public Task BusNoLongerApproachingStop(StopInfo stop, int vehicleId)
        {
            return Task.FromResult(0);
        }

        public Task BusHasDepartedStop(StopInfo stop, int vehicleId, TimeSpan delta)
        {
            return Task.FromResult(0);
        }

        public Task BusNoLongerDepartedStop(StopInfo stop, int vehicleId)
        {
            return Task.FromResult(0);
        }

        public async Task UpdateBus(BusSnapshotInfo snapshot)
        {
            Init();

            var trip = GrainFactory.GetGrain<ITrip>(snapshot.TripId);

            var tripInfo = await trip.GetInfo();

            var headsign = tripInfo == null ? string.Format("[{0}]", snapshot.TripId) : tripInfo.Headsign;

            await Clients.Group("mapViews").updateBus(snapshot, headsign);
        }
    }
}
