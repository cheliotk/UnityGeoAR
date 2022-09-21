using Assets.Scripts.Auxiliary;
using Assets.Scripts.Auxiliary.OSR;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Assets.Scripts
{
    public class RouteVisualizer : MonoBehaviour
    {
        //[SerializeField] private bool useElevations = false;
        //[SerializeField] private bool useOpenElevation = true;
        [SerializeField] private float scaleModifier = 100000f;

        [SerializeField] private RouteVisualizationType routeVisualizationType = RouteVisualizationType.NO_ELEVATION;

        [SerializeField] private Transform containerNoElevation;
        [SerializeField] private Transform containerOpenElevation;
        [SerializeField] private Transform containerOpenTopoData_ASTER;
        [SerializeField] private Transform containerOpenTopoData_EUDEM;
        [SerializeField] private GameObject waypointPrefab;
        private RoutingHandler routingHandler;

        private SceneControllerBase sceneController;

        private void Start()
        {
            sceneController = FindObjectOfType<SceneControllerBase>();
            routingHandler = RoutingHandler.Instance;
            if (routingHandler != null)
                routingHandler.onRouteReceived += RoutingHandler_onRouteReceived;
        }

        private void OnDestroy()
        {
            if (routingHandler != null)
                routingHandler.onRouteReceived -= RoutingHandler_onRouteReceived;
        }

        private void RoutingHandler_onRouteReceived(object sender, OpenRouteServiceResponse response)
        {
            routeVisualizationType = sceneController.routeVisualizationType;
            if (response?.features != null)
            {
                Vector2 startLocation = sceneController.GetPositionGGRS87AtSceneLoad();
                ClearCurrentWaypoints(routeVisualizationType);
                List<Vector2> locationsList = new List<Vector2>();
                foreach (Feature feature in response.features)
                {
                    List<List<double>> coords = feature.geometry.coordinates;
                    foreach (List<double> coord in coords)
                    {
                        if (routeVisualizationType != RouteVisualizationType.NO_ELEVATION)
                        {
                            Vector2 location = new Vector2((float)coord[0], (float)coord[1]);
                            locationsList.Add(location);
                        }
                        else
                        {
                            Vector2 point_GGRS87 = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87((float)coord[1], (float)coord[0]);
                            Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, 0f, point_GGRS87.y - startLocation.y);
                            GameObject waypoint = Instantiate(waypointPrefab, point, Quaternion.identity, containerNoElevation);
                            waypoint.name = $"Waypoint:{coord[0]},{coord[1]}";
                        }
                    }
                }

                if (routeVisualizationType != RouteVisualizationType.NO_ELEVATION)
                {
                    VisualizeRouteWithElevations(locationsList);
                }
            }
        }

        private async void VisualizeRouteWithElevations(List<Vector2> locationsList)
        {
            routeVisualizationType = sceneController.routeVisualizationType;
            ClearCurrentWaypoints(routeVisualizationType);
            Vector2 startLocation = sceneController.GetPositionGGRS87AtSceneLoad();

            if (routeVisualizationType == RouteVisualizationType.ELEVATION_OPEN_ELEVATION)
            {
                OpenElevationResponse elevationsResponse = await ElevationQueryHandler.Instance.MakeOpenElevationQuery(locationsList);
                foreach (var coord in elevationsResponse.results)
                {
                    Vector2 point_GGRS87 = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87((float)coord.latitude, (float)coord.longitude);

                    float y = (float)(coord.elevation - sceneController.GetElevationAtSceneLoad());
                    //float x = (float)(coord.longitude - sceneController.GetLocationAtSceneLoad().y) * scaleModifier;
                    //float z = (float)(coord.latitude - sceneController.GetLocationAtSceneLoad().x) * scaleModifier;
                    //Vector3 point = new Vector3(x, y, z);
                    Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, y, point_GGRS87.y - startLocation.y);
                    GameObject waypoint = Instantiate(waypointPrefab, point, Quaternion.identity, containerOpenElevation);
                    waypoint.name = $"Waypoint:{coord.latitude},{coord.longitude}";
                }
            }
            else
            {
                OpenTopoDataResponse elevationsResponse = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locationsList, routeVisualizationType);

                foreach (var coord in elevationsResponse.results)
                {
                    Vector2 point_GGRS87 = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87((float)coord.location.lat, (float)coord.location.lng);

                    float y = (float)(coord.elevation - sceneController.GetElevationAtSceneLoad());
                    //float x = (float)(coord.location.lng - sceneController.GetLocationAtSceneLoad().y) * scaleModifier;
                    //float z = (float)(coord.location.lat - sceneController.GetLocationAtSceneLoad().x) * scaleModifier;
                    //Vector3 point = new Vector3(x, y, z);
                    Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, y, point_GGRS87.y - startLocation.y);

                    Transform container = routeVisualizationType == RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM ? containerOpenTopoData_EUDEM : containerOpenTopoData_ASTER;
                    GameObject waypoint = Instantiate(waypointPrefab, point, Quaternion.identity, container);
                    waypoint.name = $"Waypoint:{coord.location.lat},{coord.location.lng}";
                }
            }
        }

        private void ClearCurrentWaypoints(RouteVisualizationType routeVisualizationType)
        {
            Transform container;
            switch (routeVisualizationType)
            {
                case RouteVisualizationType.NO_ELEVATION:
                    container = containerNoElevation;
                    break;
                case RouteVisualizationType.ELEVATION_OPEN_ELEVATION:
                    container = containerOpenElevation;
                    break;
                case RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM:
                    container = containerOpenTopoData_EUDEM;
                    break;
                case RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER:
                    container = containerOpenTopoData_ASTER;
                    break;
                default:
                    container = containerNoElevation;
                    break;
            }

            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public enum RouteVisualizationType
    {
        NO_ELEVATION = 0,
        ELEVATION_OPEN_ELEVATION = 1,
        ELEVATION_OPEN_TOPO_DATA_EUDEM = 2,
        ELEVATION_OPEN_TOPO_DATA_ASTER = 3
    }
}