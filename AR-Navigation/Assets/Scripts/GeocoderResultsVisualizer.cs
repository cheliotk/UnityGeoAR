using Assets.Scripts.Auxiliary.Nominatim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeocoderResultsVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject resultPrefab;
        [SerializeField] private Transform container;

        private Geocoder geocoder;

        private void Start()
        {
            geocoder = Geocoder.Instance;
            if(geocoder != null)
                geocoder.onGeocodeResultsReceived += Geocoder_onGeocodeResultsReceived;
        }

        private void OnDestroy()
        {
            if(geocoder != null)
                geocoder.onGeocodeResultsReceived -= Geocoder_onGeocodeResultsReceived;
        }

        private void Geocoder_onGeocodeResultsReceived(object sender, NominatimResponse e)
        {
            ClearChildren();
            foreach (Feature feature in e.features)
            {
                GameObject result = Instantiate(resultPrefab, container);
                GeocoderResultEntry entry = result.GetComponent<GeocoderResultEntry>();
                entry.SetLabel(feature.properties.display_name);
            }
        }

        private void ClearChildren()
        {
            for (int i = 0; i < container.childCount; i++)
            {
                Transform child = container.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }
}