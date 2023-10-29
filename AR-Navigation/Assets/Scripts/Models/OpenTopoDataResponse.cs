using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class OpenTopoDataResult
    {
        public string dataset { get; set; }
        public float elevation { get; set; }
        public Location location { get; set; }
    }

    public class OpenTopoDataResponse
    {
        public List<OpenTopoDataResult> results { get; set; }
        public string status { get; set; }
    }


}