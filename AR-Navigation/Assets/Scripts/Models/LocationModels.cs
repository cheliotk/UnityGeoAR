using UnityEngine;

namespace Assets.Scripts.Models
{
    public class LocationCompassData
    {
        public LocationData location;
        public CompassData compass;
        public bool isFirstUpdate;

        public LocationCompassData()
        {
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
        public double timestamp;
    }

    public struct CompassData
    {
        public float magneticHeading;
        public float trueHeading;
        public Vector3 rawVector;
        public double timestamp;
    }
}
