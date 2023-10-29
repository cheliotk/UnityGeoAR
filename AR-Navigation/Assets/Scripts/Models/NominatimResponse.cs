using System.Collections.Generic;

namespace Assets.Scripts.Models.Nominatim
{
    public class Feature
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<double> bbox { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Properties
    {
        public int place_id { get; set; }
        public string osm_type { get; set; }
        public long osm_id { get; set; }
        public string display_name { get; set; }
        public int place_rank { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public double importance { get; set; }
    }

    public class NominatimResponse
    {
        public string type { get; set; }
        public string licence { get; set; }
        public List<Feature> features { get; set; }
    }


}