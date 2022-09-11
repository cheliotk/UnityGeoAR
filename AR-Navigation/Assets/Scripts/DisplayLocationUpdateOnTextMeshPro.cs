using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class DisplayLocationUpdateOnTextMeshPro : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        private LocationUpdater locationUpdater;

        private void Start()
        {
            locationUpdater = LocationUpdater.Instance;
            if(locationUpdater != null)
                locationUpdater.onLocationCompassDataUpdatedEvent += LocationUpdater_onLocationCompassDataUpdatedEvent;
        }

        private void OnDestroy()
        {
            if (locationUpdater != null)
                locationUpdater.onLocationCompassDataUpdatedEvent -= LocationUpdater_onLocationCompassDataUpdatedEvent;
        }

        private void LocationUpdater_onLocationCompassDataUpdatedEvent(object sender, LocationCompassData e)
        {
            string s = $"lat: {e.location.latitude}" +
                $"\n lon: {e.location.longitude}" +
                $"\n brn: {e.compass.trueHeading}";

            text.text = s;
        }
    }
}