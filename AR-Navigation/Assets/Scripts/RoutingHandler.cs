using Assets.Scripts.Auxiliary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoutingHandler : MonoBehaviour
    {
        private string directionsApiBase = "https://api.openrouteservice.org/v2/directions";
        private string directionsProfile = "foot-walking";
        [SerializeField] private Vector2 startLatLong;
        [SerializeField] private Vector2 endLatLong;

        [SerializeField] APIKeyContainer apiKeyContainer;

        [ContextMenu("RunQuery")]
        public void StartQuery()
        {
            RouteDirections_GET();
        }

        public async void RouteDirections_GET()
        {
            if (apiKeyContainer == null)
                return;

            var baseAddress = new Uri($"{directionsApiBase}/{directionsProfile}?api_key={apiKeyContainer.OpenRouteServiceApiKey}&start={startLatLong.y},{startLatLong.x}&end={endLatLong.y},{endLatLong.x}");
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");

                using (var response = await httpClient.GetAsync(baseAddress))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject(responseData);

                    OpenRouteServiceResponse data2 = JsonConvert.DeserializeObject<OpenRouteServiceResponse>(responseData);
                    var bob = data2?.type;
                }
            }
        }
    }
}

