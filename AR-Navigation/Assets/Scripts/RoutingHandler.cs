﻿using Assets.Scripts.Auxiliary;
using Assets.Scripts.Auxiliary.OSR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoutingHandler : MonoBehaviour
    {
        public static RoutingHandler Instance { get; private set; }
        public event EventHandler<OpenRouteServiceResponse> onRouteReceived;

        [SerializeField] private Vector2 startLatLong = new Vector2(38.01408f, 23.74127f);
        [SerializeField] private Vector2 endLatLong = new Vector2(38.02369f, 23.73612f);

        [SerializeField] APIKeyContainer apiKeyContainer;

        private string directionsApiBase = "https://api.openrouteservice.org/v2/directions";
        private string directionsProfile = "foot-walking";
        private SceneControllerBase sceneController;

        private void Awake()
        {
            var instances = FindObjectsOfType<RoutingHandler>();
            if(instances.Length > 1)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            sceneController = FindObjectOfType<SceneControllerBase>();
            startLatLong = sceneController.GetLocationAtSceneLoad();
        }

        public void SetTargetLocation(Vector2 targetLatLong) => endLatLong = targetLatLong;
        public void StartQueryWithCurrentParameters() => RouteDirections_GET();

        [ContextMenu("RunQuery")]
        public void StartDefaultQuery()
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
                    if (response.IsSuccessStatusCode)
                    {
                        OpenRouteServiceResponse route = JsonConvert.DeserializeObject<OpenRouteServiceResponse>(responseData);
                        onRouteReceived?.Invoke(this, route);
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }
                }
            }
        }
    }
}
