using Assets.Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class ElevationQueryService
    {
        private const string OPEN_ELEVATION_API_BASE_GET = "https://api.open-elevation.com/api/v1/lookup?";
        private const string OPENTOPODATA_API_BASE_GET = "https://api.opentopodata.org/v1/";
        private const string OPENTOPODATA_DATASET_ASTER = "aster30m?";
        private const string OPENTOPODATA_DATASET_EUDEM = "eudem25m?";

        private float lastOpenTopoDataCall = -1f;

        public async Task<OpenElevationResponse> MakeOpenElevationQuery(List<Vector2> locations)
        {
            string locationsString = ConstructLocationsListStringFromVector2Array(locations);
            string query = $"{OPEN_ELEVATION_API_BASE_GET}locations={locationsString}";
            var baseAddress = new Uri(query);
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Unity 2021.3.9f1 AR-Navigation test app");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json; charset=utf-8");

                using (var response = await httpClient.GetAsync(baseAddress))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        OpenElevationResponse elevationResults = JsonConvert.DeserializeObject<OpenElevationResponse>(responseData);
                        return elevationResults;
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString() + "\n" + response.ToString());
                    }
                }
            }
        }

        public async Task<OpenTopoDataResponse> MakeOpenTopoDataQuery(List<Vector2> locations, RouteVisualizationType routeVisualizationType)
        {
            if (routeVisualizationType != RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM && routeVisualizationType != RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER)
                throw new Exception($"Wrong Dataset requested: Expecting OpenTopoData, got {routeVisualizationType}");

            string locationsString = ConstructLocationsListStringFromVector2Array(locations);

            string dataset = routeVisualizationType == RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM ? OPENTOPODATA_DATASET_EUDEM : OPENTOPODATA_DATASET_ASTER;
            string query = $"{OPENTOPODATA_API_BASE_GET}{dataset}locations={locationsString}";

            var baseAddress = new Uri(query);
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Unity 2021.3.9f1 AR-Navigation test app");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json; charset=utf-8");

                while (Time.realtimeSinceStartup - lastOpenTopoDataCall < 1.5f)
                {
                    await Task.Delay(100);
                }

                using (var response = await httpClient.GetAsync(baseAddress))
                {
                    lastOpenTopoDataCall = Time.realtimeSinceStartup;
                    string responseData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        OpenTopoDataResponse elevationResults = JsonConvert.DeserializeObject<OpenTopoDataResponse>(responseData);
                        return elevationResults;
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString() + "\n" + response.ToString());
                    }
                }
            }
        }

        private string ConstructLocationsListStringFromVector2Array(List<Vector2> locations)
        {
            if (locations.Count == 1)
                return $"{locations[0].y.ToString("F7")},{locations[0].x.ToString("F7")}";

            string locationsString = "";
            for (int i = 0; i < locations.Count; i++)
            {
                Vector2 location = locations[i];
                locationsString += $"{location.y.ToString("F7")},{location.x.ToString("F7")}";
                if (i != locations.Count - 1)
                    locationsString += "|";
            }

            return locationsString;
        }
    }
}
