using Assets.Scripts.Auxiliary.Nominatim;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeocoderSelectedResultPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        public Feature selectedFeature { get; private set; }

        public void SetSelectedFeature(Feature feature)
        {
            selectedFeature = feature;
            label.text = selectedFeature.properties.display_name;
        }

        public void CalculateRoute()
        {
            Vector2 targetLocation = new Vector2((float)selectedFeature.geometry.coordinates[1], (float)selectedFeature.geometry.coordinates[0]);
            RoutingHandler.Instance.SetTargetLocation(targetLocation);
            RoutingHandler.Instance.StartQueryWithCurrentParameters();
        }
    }
}