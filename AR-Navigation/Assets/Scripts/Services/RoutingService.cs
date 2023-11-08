using Assets.Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class RoutingService
    {
        private const string DIRECTIONS_API_BASE = "https://api.openrouteservice.org/v2/directions";
        private const string PROFILE_WALKING = "foot-walking";
        public event EventHandler<OpenRouteServiceResponse> onRouteReceived;

        private Vector2 originLatLong = default;
        private Vector2 destinationLatLong = default;

        private string openRouteServiceApiKey;

        private string directionsApiBase;
        private string directionsProfile;

        public RoutingService(string openRouteServiceApiKey,
            string directionsApiBase = DIRECTIONS_API_BASE,
            string directionsProfile = PROFILE_WALKING)
        {
            this.openRouteServiceApiKey = openRouteServiceApiKey;
            this.directionsApiBase = directionsApiBase;
            this.directionsProfile = directionsProfile;
        }

        public void SetTargetLocation(Vector2 targetLatLong) => destinationLatLong = targetLatLong;
        public void StartQueryWithParameters(Vector2 startLatLong,  Vector2 endLatLong)
        {
            StartQuery(originLatLong, destinationLatLong);
        }

        private async void StartQuery(Vector2 originLatLong, Vector2 destinationLatLong)
        {
            this.originLatLong = originLatLong;
            this.destinationLatLong = destinationLatLong;
            await RouteDirections_GET();
        }

        public async Task RouteDirections_GET()
        {
            if (string.IsNullOrWhiteSpace(openRouteServiceApiKey))
            {
                throw new OperationCanceledException("No OpenRouteService API key set");
            }

            try
            {
                var baseAddress = new Uri($"{directionsApiBase}/{directionsProfile}?api_key={openRouteServiceApiKey}&start={originLatLong.y},{originLatLong.x}&end={destinationLatLong.y},{destinationLatLong.x}");
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");

                    using (var response = await httpClient.GetAsync(baseAddress))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            OpenRouteServiceResponse route = JsonConvert.DeserializeObject<OpenRouteServiceResponse>(responseData);
                            onRouteReceived?.Invoke(this, route);
                        }
                        else
                        {
                            string exceptionMessage = $"routing request το {directionsApiBase} failed with response {response}";
                            throw new ExternalException(exceptionMessage);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
