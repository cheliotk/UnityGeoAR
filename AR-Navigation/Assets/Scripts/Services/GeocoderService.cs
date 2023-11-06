using Assets.Scripts.Models.Nominatim;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web;

namespace Assets.Scripts.Services
{
    public class GeocoderService
    {
        private const string NOMINATIM_GEOCODER_API_BASE = "https://nominatim.openstreetmap.org/search?";

        public event EventHandler<NominatimResponse> onGeocodeResultsReceived;
        private string searchTerm = "";
        private string geocoderApiBase;

        public GeocoderService(string geocoderApiBase = NOMINATIM_GEOCODER_API_BASE)
        {
            this.geocoderApiBase = geocoderApiBase;
        }

        public void MakeQuery(string searchTerm)
        {
            this.searchTerm = searchTerm;
            MakeQuery();
        }

        private async void MakeQuery()
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}