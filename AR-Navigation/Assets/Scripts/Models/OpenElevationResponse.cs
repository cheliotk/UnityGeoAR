using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class OpenElevationResult
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int elevation { get; set; }
    }

    public class OpenElevationResponse
    {
        public List<OpenElevationResult> results { get; set; }
    }
}