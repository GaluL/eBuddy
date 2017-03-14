using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace eBuddyApp.Services.Location
{
    static class ExtentionMethods
    { 
        public static Geopoint ToGeoPoint(this Geoposition position)
        {
            return new Geopoint(new BasicGeoposition()
            {
                Altitude = position.Coordinate.Point.Position.Altitude,
                Longitude = position.Coordinate.Point.Position.Longitude,
                Latitude = position.Coordinate.Point.Position.Latitude,
            });
        }

        public static Geopoint GetDefaultPoint()
        {
            return new Geopoint(new BasicGeoposition()
            {
                Altitude = 0,
                Longitude = 34.7681563,
                Latitude = 32.0797956
            });
        }
    }
}
