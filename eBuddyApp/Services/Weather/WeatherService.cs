using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherNet.Model;

namespace eBuddyApp.Services.Weather
{
    class WeatherService
    {
        private const string _OpenWeatherMapKey = "1d28437aa3cba120d3b7174f474f7434";

        private static WeatherService _instance;
        public static WeatherService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeatherService();
                }

                return _instance;
            }
        }

        private WeatherService()
        {
            WeatherNet.ClientSettings.SetApiKey(_OpenWeatherMapKey);
        }

        public async Task<CurrentWeatherResult> GetWeatherForLocation(double lon, double lat)
        {
            return (await WeatherNet.Clients.CurrentWeather.GetByCoordinatesAsync(lat, lon, "eng", "metric")).Item;
        }
    }
}
