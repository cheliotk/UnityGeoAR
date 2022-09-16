using Assets.Scripts.Auxiliary.Nominatim;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeocoderResultsVisualizer : MonoBehaviour
    {
        public event EventHandler<Feature> onEntrySelected;
        [SerializeField] private GameObject resultPrefab;
        [SerializeField] private Transform container;

        private readonly List<GeocoderResultEntry> entries = new List<GeocoderResultEntry>();

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
                entries.Add(entry);
                entry.SetLabel(feature.properties.display_name);
                entry.SetValue(feature);
                entry.onEntryClicked += Entry_onEntrySelected;
            }
        }

        private void Entry_onEntrySelected(object sender, System.EventArgs e)
        {
            GeocoderResultEntry senderEntry = sender as GeocoderResultEntry;
            foreach (var entry in entries)
            {
                entry.SetEntryIsSelected(entry == senderEntry);
            }

            onEntrySelected?.Invoke(this, senderEntry.valueFeature);
        }

        private void RemoveChild(GeocoderResultEntry entry)
        {
            entry.onEntryClicked -= Entry_onEntrySelected;
            entries.Remove(entry);
            Destroy(entry.gameObject);
        }

        private void ClearChildren()
        {
            //foreach (GeocoderResultEntry entry in entries)
            //{
            //    Destroy(entry.gameObject);
            //}
            //entries.Clear();

            for (int i = 0; i < container.childCount; i++)
            {
                RemoveChild(container.GetChild(i).GetComponent<GeocoderResultEntry>());
                //Transform child = container.GetChild(i);
                //Destroy(child.gameObject);
            }
        }
    }
}