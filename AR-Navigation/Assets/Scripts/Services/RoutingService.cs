using Assets.Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class RoutingService
    {
        public static string OpenRouteServiceDirectionsApiBase = "https://api.openrouteservice.org/v2/directions";
        public static string OpenRouteServiceDirectionProfileWalking = "foot-walking";
        public event EventHandler<OpenRouteServiceResponse> onRouteReceived;

        private Vector2 originLatLong = default;
        private Vector2 destinationLatLong = default;

        private string openRouteServiceApiKey;

        private string directionsApiBase;
        private string directionsProfile;

        public RoutingService(string openRouteServiceApiKey, string directionsApiBase, string directionsProfile)
        {
            this.openRouteServiceApiKey = openRouteServiceApiKey;
            this.directionsApiBase = directionsApiBase;
            this.directionsProfile = directionsProfile;
        }

        public void SetTargetLocation(Vector2 targetLatLong) => destinationLatLong = targetLatLong;
        public void StartQueryWithCurrentParameters() => StartQuery(originLatLong, destinationLatLong);

        public async void StartQuery(Vector2 originLatLong, Vector2 destinationLatLong)
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
                //Debug.Log($"route request: {baseAddress}");
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");

                    using (var response = await httpClient.GetAsync(baseAddress))
                    {
                        //Debug.Log($"1: {response}");
                        string responseData = await response.Content.ReadAsStringAsync();
                        //Debug.Log($"2: {responseData}");

                        if (response.IsSuccessStatusCode)
                        {
                            OpenRouteServiceResponse route = JsonConvert.DeserializeObject<OpenRouteServiceResponse>(responseData);
                            //Debug.Log($"3: route with {route.features.Count} features");
                            onRouteReceived?.Invoke(this, route);
                        }
                        else
                        {
                            string exceptionMessage = $"routing request το {directionsApiBase} failed with response {response}";
                            Debug.LogWarning(exceptionMessage);
                            throw new Exception(exceptionMessage);
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
