using UnityEngine;

namespace Assets.Scripts.Models
{
    public class LocationCompassData
    {
        public double timestamp;
        public LocationData location;
        public CompassData compass;
        public bool isFirstUpdate;

        public LocationCompassData()
        {
            timestamp = 0f;
            location.latitude = 0f;
            location.longitude = 0f;
            location.altitude = 0f;

            compass.magneticHeading = 72f;
            compass.trueHeading = 72f;
            compass.rawVector = Vector3.one;

            isFirstUpdate = false;
        }
    }

    public struct LocationData
    {
        public float latitude;
        public float longitude;
        public float altitude;
    }

    public struct CompassData
    {
        public float magneticHeading;
        public float trueHeading;
        public Vector3 rawVector;
    }
}
