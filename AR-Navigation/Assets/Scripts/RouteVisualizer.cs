using Assets.Scripts.Auxiliary;
using Assets.Scripts.Auxiliary.OSR;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Assets.Scripts
{
    public class RouteVisualizer : MonoBehaviour
    {
        [SerializeField] private float scaleModifier = 100000f;

        [SerializeField] private RouteVisualizationType routeVisualizationType = RouteVisualizationType.NO_ELEVATION;

        [SerializeField] private Transform containerNoElevation;
        [SerializeField] private Transform containerOpenElevation;
        [SerializeField] private Transform containerOpenTopoData_ASTER;
        [SerializeField] private Transform containerOpenTopoData_EUDEM;
        [SerializeField] private GameObject waypointPrefab;
        [SerializeField] private GameObject linePrefab;
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
                List<Vector3> waypointPositions = new List<Vector3>();
                List<string> waypointNames = new List<string>();

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
                            
                            waypointPositions.Add(point);
                            waypointNames.Add($"{coord[1]},{coord[0]}");
                        }
                    }
                }

                if (routeVisualizationType != RouteVisualizationType.NO_ELEVATION)
                {
                    PrepareWaypointsWithElevations(locationsList);
                }
                else
                {
                    VisualizeRoute(waypointPositions, waypointNames);
                }
            }
        }

        private async void PrepareWaypointsWithElevations(List<Vector2> locationsList)
        {
            routeVisualizationType = sceneController.routeVisualizationType;
            ClearCurrentWaypoints(routeVisualizationType);
            Vector2 startLocation = sceneController.GetPositionGGRS87AtSceneLoad();
            
            List<Vector3> waypointPositions = new List<Vector3>();
            List<string> waypointNames = new List<string>();
            
            if (routeVisualizationType == RouteVisualizationType.ELEVATION_OPEN_ELEVATION)
            {
                OpenElevationResponse elevationsResponse = await ElevationQueryHandler.Instance.MakeOpenElevationQuery(locationsList);
                foreach (var coord in elevationsResponse.results)
                {
                    Vector2 point_GGRS87 = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87((float)coord.latitude, (float)coord.longitude);

                    float y = (float)(coord.elevation - sceneController.GetElevationAtSceneLoad() - 1f);
                    Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, y, point_GGRS87.y - startLocation.y);
                    
                    waypointPositions.Add(point);
                    waypointNames.Add($"{coord.latitude},{coord.longitude}");
                }
            }
            else
            {
                OpenTopoDataResponse elevationsResponse = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locationsList, routeVisualizationType);

                foreach (var coord in elevationsResponse.results)
                {
                    Vector2 point_GGRS87 = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87((float)coord.location.lat, (float)coord.location.lng);

                    float y = (float)(coord.elevation - sceneController.GetElevationAtSceneLoad());
                    Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, y, point_GGRS87.y - startLocation.y);

                    waypointPositions.Add(point);
                    waypointNames.Add($"{coord.location.lat},{coord.location.lng}");
                }
            }

            VisualizeRoute(waypointPositions, waypointNames);
        }

        private void VisualizeRoute(List<Vector3> waypointPositions, List<string> waypointNames)
        {
            routeVisualizationType = sceneController.routeVisualizationType;
            ClearCurrentWaypoints(routeVisualizationType);

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

            Transform lineContainer = new GameObject("lineContainer").transform;
            lineContainer.SetParent(container);
            lineContainer.localPosition = Vector3.zero;

            for (int i = 0; i < waypointPositions.Count; i++)
            {
                Vector3 waypointPosition = waypointPositions[i];
                GameObject waypoint = Instantiate(waypointPrefab, waypointPosition, Quaternion.identity, container);
                waypoint.name = $"Waypoint:{waypointNames[i]}";

                if (i == 0)
                    continue;

                GameObject line = Instantiate(linePrefab, waypointPosition, Quaternion.Euler(Vector3.right*90f), lineContainer);
                LineRenderer lren = line.GetComponent<LineRenderer>();
                lren.positionCount = 2;
                lren.SetPositions(new Vector3[] { waypointPosition, waypointPositions[i - 1] });
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