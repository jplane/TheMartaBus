
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Marta.Common;

namespace Marta.Web
{
    public class MapHub : Hub
    {
        public Task RegisterMapView()
        {
            return Groups.Add(Context.ConnectionId, "mapViews");
        }

        public Task UpdateStopStatus(int stopId, Tuple<int, TimeSpan>[] arriving, Tuple<int, TimeSpan>[] departed)
        {
            var stop = StaticDataLoader.GetStopById(stopId);

            var name = stop == null ? stopId.ToString() : stop.Name;

            return Clients.Group("mapViews").updateStopMarker(stopId, name, stop.Latitude, stop.Longitude, arriving.Length, departed.Length);
        }

        public Task UpdateBusStatus(BusStatus status)
        {
            var trip = StaticDataLoader.GetTripById(status.TripId);

            var name = "Bus " + status.VehicleId + " - [" + (trip == null ? "trip " + status.TripId.ToString() : trip.Headsign) + "]";

            return Clients.Group("mapViews").updateBusMarker(status.VehicleId, name, status.Latitude, status.Longitude);
        }
    }
}
