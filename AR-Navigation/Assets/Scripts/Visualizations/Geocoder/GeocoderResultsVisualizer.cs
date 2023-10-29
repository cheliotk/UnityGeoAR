using Assets.Scripts.Models.Nominatim;
using Assets.Scripts.Services;
using System;
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

        private GeocoderService geocoder;

        private void Start()
        {
            geocoder = new GeocoderService(GeocoderService.NominatimGeocoderApiBase);
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
            for (int i = 0; i < container.childCount; i++)
            {
                RemoveChild(container.GetChild(i).GetComponent<GeocoderResultEntry>());
            }
        }
    }
}