using Assets.Scripts.Auxiliary;
using Assets.Scripts.Auxiliary.Nominatim;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using UnityEditor.Search;
using UnityEngine;

namespace Assets.Scripts
{
    public class ElevationQueryHandler : MonoBehaviour
    {
        public static ElevationQueryHandler Instance { get; private set; }

        private string openElevationApiBase_GET = "https://api.open-elevation.com/api/v1/lookup?";
        private string temp = "https://api.opentopodata.org/v1/aster30m?locations=39.747114,-104.996334";
        private string openTopoDataApiBase_GET = "https://api.opentopodata.org/v1/";
        private string openTopoDataDataset_ASTER = "aster30m?";
        private string openTopoDataDataset_EUDEM = "eudem25m?";

        private void Awake()
        {
            var instances = FindObjectsOfType<ElevationQueryHandler>();
            if (instances.Length > 1)
                Destroy(this.gameObject);

            Instance = this;
        }

        public async Task<OpenElevationResponse> MakeOpenElevationQuery(List<Vector2> locations)
        {
            string locationsString = ConstructLocationsListStringFromVector2Array(locations);
            string query = $"{openElevationApiBase_GET}locations={locationsString}";
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

            string dataset = routeVisualizationType == RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM ? openTopoDataDataset_EUDEM : openTopoDataDataset_ASTER;
            string query = $"{openTopoDataApiBase_GET}{dataset}locations={locationsString}";
            
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