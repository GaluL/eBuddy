using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace eBuddyApp.Services
{
    class MapServiceWrapper
    {
        private static MapServiceWrapper _Instance = null;
        internal static MapServiceWrapper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MapServiceWrapper();
                }

                return _Instance;
            }
        }

        private MapServiceWrapper()
        {
            Windows.Services.Maps.MapService.ServiceToken =
                "OARdWd6u76SCOJpF63br~nEfdGL_BYBbFn1jt0wom8Q~Aoyg4vAPbQczjy1VVSUyFfFcpz_G1Q9eqrBUO9FdMP1a725us7XvhB7zycvi-lbq";
        }

        internal async Task<MapRoute> GetRoute(IEnumerable<Geopoint> waypoints)
        {
            try
            {
                MapRouteFinderResult routeFind = null;

                if (waypoints.Count() > 1)
                {
                    routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(waypoints);
                }
                else if (waypoints.Count() == 1)
                {
                    routeFind = await MapRouteFinder.GetWalkingRouteAsync(waypoints.ElementAt(0), waypoints.ElementAt(0));
                }

                if (routeFind != null && routeFind.Status == MapRouteFinderStatus.Success)
                {
                    return routeFind.Route;
                }
                else
                {
                    
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
