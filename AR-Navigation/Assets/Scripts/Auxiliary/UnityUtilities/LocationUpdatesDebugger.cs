using Assets.Scripts;
using Assets.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationUpdatesDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] Image compassImage;

    private LocationUpdater locationUpdater;

    private void Start()
    {
        locationUpdater = LocationUpdater.Instance;
        locationUpdater.onLocationCompassDataUpdatedEvent += LocationUpdater_onLocationCompassDataUpdatedEvent;
    }

    private void OnDestroy()
    {
        locationUpdater.onLocationCompassDataUpdatedEvent -= LocationUpdater_onLocationCompassDataUpdatedEvent;
    }

    private void LocationUpdater_onLocationCompassDataUpdatedEvent(object sender, LocationCompassData e)
    {
        text.text = $"{e.location.latitude.ToString("F5")}, {e.location.longitude.ToString("F5")}";
        var rot = compassImage.transform.rotation.eulerAngles;
        rot.z = e.compass.magneticHeading;
        compassImage.transform.rotation = Quaternion.Euler(rot);
    }
}
