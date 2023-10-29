using Assets.Scripts.Models.Nominatim;
using Assets.Scripts.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeocoderSelectedResultPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Text coordinatesText;
        public Feature selectedFeature { get; private set; }

        private SceneControllerBase sceneController;
        private RoutingService routingService;

        private void Start()
        {
            sceneController = FindObjectOfType<SceneControllerBase>();

            routingService = sceneController.RoutingService;
        }

        public void SetSelectedFeature(Feature feature)
        {
            selectedFeature = feature;
            label.text = selectedFeature.properties.display_name;
            SetCoordinatesLabel(selectedFeature.geometry.coordinates);
        }

        private void SetCoordinatesLabel(List<double> coordsXY)
        {
            string text = $"coordinates: {coordsXY[1]}, {coordsXY[0]}";
            coordinatesText.text = text;
        }

        public void CalculateRoute()
        {
            Vector2 targetLocation = new Vector2((float)selectedFeature.geometry.coordinates[1], (float)selectedFeature.geometry.coordinates[0]);
            routingService.SetTargetLocation(targetLocation);
            routingService.StartQueryWithCurrentParameters();
        }
    }
}