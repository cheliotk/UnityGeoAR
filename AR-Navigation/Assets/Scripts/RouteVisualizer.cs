using Assets.Scripts.Auxiliary;
using Assets.Scripts.Auxiliary.OSR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class RouteVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject waypointPrefab;
        private RoutingHandler routingHandler;

        private void Start()
        {
            routingHandler = RoutingHandler.Instance;
            if(routingHandler != null)
                routingHandler.onRouteReceived += RoutingHandler_onRouteReceived;
        }

        private void OnDestroy()
        {
            if (routingHandler != null)
                routingHandler.onRouteReceived -= RoutingHandler_onRouteReceived;
        }

        private void RoutingHandler_onRouteReceived(object sender, OpenRouteServiceResponse response)
        {
            if(response?.features != null)
            {
                foreach (Feature feature in response.features)
                {
                    List<List<double>> coords = feature.geometry.coordinates;
                    foreach (List<double> coord in coords)
                    {
                        Vector3 point = new Vector3((float)coord[0], 0f, (float)coord[1]);
                        Instantiate(waypointPrefab, point*1000f, Quaternion.identity);
                    }
                }
            }
        }
    }
}