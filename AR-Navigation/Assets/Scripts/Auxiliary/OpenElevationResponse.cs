using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Auxiliary
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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