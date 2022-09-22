using Assets.Scripts.Auxiliary.Nominatim;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web;
using UnityEngine;

namespace Assets.Scripts
{
    public class Geocoder : MonoBehaviour
    {
        public event EventHandler<NominatimResponse> onGeocodeResultsReceived;
        public static Geocoder Instance { get; private set; }

        private string geocoderApiBase = "https://nominatim.openstreetmap.org/search?";
        private string searchTerm = "";

        private void Awake()
        {
            var instances = FindObjectsOfType<Geocoder>();
            if (instances.Length > 1)
                Destroy(this.gameObject);

            Instance = this;
        }

        [ContextMenu("Make Default Query")]
        public void MakeDefaultQuery()
        {
            searchTerm = "ανω πατησια";
            MakeQuery();
        }

        public void MakeQuery(string searchTerm)
        {
            this.searchTerm = searchTerm;
            MakeQuery();
        }

        private async void MakeQuery()
        {
            string urlSafeSearchTerm = HttpUtility.UrlEncode(searchTerm);
            string query = $"{geocoderApiBase}q={urlSafeSearchTerm}&format=geojson";
            var baseAddress = new Uri(query);
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Unity 2021.3.9f1 AR-Navigation test app");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");

                using (var response = await httpClient.GetAsync(baseAddress))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        NominatimResponse geocodedResults = JsonConvert.DeserializeObject<NominatimResponse>(responseData);
                        onGeocodeResultsReceived?.Invoke(this, geocodedResults);
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString() + "\n" + response.ToString());
                    }
                }
            }
        }
    }
}