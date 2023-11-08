using Assets.Scripts.Auxiliary;
using Assets.Scripts.Models;
using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class RouteVisualizer : MonoBehaviour
    {
        public int OPENTOPODATA_QUEUE_SIZE = 5;

        [SerializeField] private ElevationAPI routeVisualizationType = ElevationAPI.NO_ELEVATION;

        [SerializeField] private Transform containerNoElevation;
        [SerializeField] private Transform containerOpenElevation;
        [SerializeField] private Transform containerOpenTopoData_ASTER;
        [SerializeField] private Transform containerOpenTopoData_EUDEM;
        [SerializeField] private GameObject waypointPrefab;
        [SerializeField] private GameObject linePrefab;
        
        private SceneControllerBase sceneController;
        private RoutingService routingService;
        //private ReprojectionService reprojectionService;
        //private ElevationQueryService elevationQueryService;

        private void Start()
        {
            sceneController = FindObjectOfType<SceneControllerBase>();

            //reprojectionService = sceneController.ReprojectionService;
            //elevationQueryService = sceneController.ElevationQueryService;
            routingService = sceneController.RoutingService;
            if (routingService != null)
                routingService.onRouteReceived += RoutingHandler_onRouteReceived;
        }

        private void OnDestroy()
        {
            if (routingService != null)
                routingService.onRouteReceived -= RoutingHandler_onRouteReceived;
        }

        public async Task HandlePresetRoute(List<Vector2> route, List<string> waypointNames)
        {
            try
            {
                if (routeVisualizationType != ElevationAPI.NO_ELEVATION)
                {
                    await PrepareWaypointsWithElevations(route);
                }
                else
                {
                    //List<Vector3> waypoints = new List<Vector3>();
                    List<Vector3> waypoints = await sceneController.WorldToUnityService.GetUnityPositionsFromCoordinates(route, ElevationAPI.NO_ELEVATION);

                    //foreach (Vector2 coord in route)
                    //{
                    //    Vector2 startLocation = sceneController.GetLocationDestinationCRSAtSceneLoad();
                    //    Vector2 point_GGRS87 = reprojectionService.ReprojectPoint(coord.y, coord.x);
                    //    Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, 0f, point_GGRS87.y - startLocation.y);
                    //    waypoints.Add(point);
                    //}
                    VisualizeRoute(waypoints, waypointNames);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async void RoutingHandler_onRouteReceived(object sender, OpenRouteServiceResponse response)
        {
            try
            {
                routeVisualizationType = sceneController.routeVisualizationType;
                if (response?.features != null)
                {
                    Vector2 startLocation = sceneController.GetLocationDestinationCRSAtSceneLoad();
                    ClearCurrentWaypoints(routeVisualizationType);
                
                    List<Vector2> locationsList = new List<Vector2>();
                    //List<Vector3> waypointPositions = new List<Vector3>();
                    List<string> waypointNames = new List<string>();

                    foreach (Feature feature in response.features)
                    {
                        List<List<double>> coords = feature.geometry.coordinates;
                        foreach (List<double> coord in coords)
                        {
                            Vector2 location = new Vector2((float)coord[0], (float)coord[1]);
                            locationsList.Add(location);
                            waypointNames.Add($"{coord[1]},{coord[0]}");

                            //if (routeVisualizationType != ElevationAPI.NO_ELEVATION)
                            //{
                            //    Vector2 location = new Vector2((float)coord[0], (float)coord[1]);
                            //    locationsList.Add(location);
                            //}
                            //else
                            //{
                            //    Vector2 point_GGRS87 = reprojectionService.ReprojectPoint(coord[1], coord[0]);
                            //    Vector3 point = new Vector3(point_GGRS87.x - startLocation.x, 0f, point_GGRS87.y - startLocation.y);

                            //    waypointPositions.Add(point);
                            //    waypointNames.Add($"{coord[1]},{coord[0]}");
                            //}
                        }
                    }
                    if (routeVisualizationType != ElevationAPI.NO_ELEVATION)
                    {
                        await PrepareWaypointsWithElevations(locationsList);
                    }
                    else
                    {

                        List<Vector3> waypointPositions = await sceneController.WorldToUnityService.GetUnityPositionsFromCoordinates(locationsList, routeVisualizationType);
                        VisualizeRoute(waypointPositions, waypointNames);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task PrepareWaypointsWithElevations(List<Vector2> locationsList)
        {
            routeVisualizationType = sceneController.routeVisualizationType;
            ClearCurrentWaypoints(routeVisualizationType);
            Vector2 startLocation = sceneController.GetLocationDestinationCRSAtSceneLoad();

            List<Vector2> tempLocationsList = new List<Vector2>();

            bool clearStaleWaypoints = true;
            while(locationsList.Count > OPENTOPODATA_QUEUE_SIZE)
            {
                tempLocationsList.AddRange(locationsList.GetRange(0, OPENTOPODATA_QUEUE_SIZE));
                locationsList.RemoveRange(0, OPENTOPODATA_QUEUE_SIZE - 1);
                await PrepareWaypointBatchWithElevations(startLocation, tempLocationsList, clearStaleWaypoints);
                clearStaleWaypoints = false;
                tempLocationsList.Clear();
            }

            if(locationsList.Count > 0)
            {
                tempLocationsList.AddRange(locationsList);
                await PrepareWaypointBatchWithElevations(startLocation, tempLocationsList, clearStaleWaypoints);
            }
        }

        private async Task PrepareWaypointBatchWithElevations(Vector2 startPointInCartesianSpace, List<Vector2> tempLocationsList, bool clearStaleWaypoints)
        {
            //List<Vector3> currentWaypointPositions = new List<Vector3>();
            List<string> waypointNames = new List<string>();
            foreach (var waypoint in tempLocationsList)
            {
                waypointNames.Add($"{waypoint.y},{waypoint.x}");
            }

            if (routeVisualizationType == ElevationAPI.OPEN_ELEVATION)
            {
                // OPEN_ELEVATION no longer supported
                throw new NotSupportedException("OPEN_ELEVATION no longer supported");
                //OpenElevationResponse elevationsResponse = await elevationQueryService.MakeOpenElevationQuery(tempLocationsList);
                //foreach (var coord in elevationsResponse.results)
                //{
                //    Vector2 point_GGRS87 = reprojectionService.ReprojectPoint(coord.latitude, coord.longitude);

                //    float y = (float)(coord.elevation - sceneController.GetElevationAtSceneLoad() - 1f);
                //    Vector3 point = new Vector3(point_GGRS87.x - startPointInCartesianSpace.x, y, point_GGRS87.y - startPointInCartesianSpace.y);

                //    currentWaypointPositions.Add(point);
                //    waypointNames.Add($"{coord.latitude},{coord.longitude}");
                //}
            }
            //else
            //{
                //OpenTopoDataResponse elevationsResponse = await elevationQueryService.MakeOpenTopoDataQuery(tempLocationsList, routeVisualizationType);

                //foreach (OpenTopoDataResult coord in elevationsResponse.results)
                //{
                //    Vector2 pointInCartesianSpace = reprojectionService.ReprojectPoint(coord.location.lat, coord.location.lng);

                //    float y = coord.elevation - sceneController.GetElevationAtSceneLoad();
                //    Vector3 point = new Vector3(pointInCartesianSpace.x - startPointInCartesianSpace.x, y, pointInCartesianSpace.y - startPointInCartesianSpace.y);

                //    currentWaypointPositions.Add(point);
                //    waypointNames.Add($"{coord.location.lat},{coord.location.lng}");
                //}
            //}
            List<Vector3> currentWaypointPositions = await sceneController.WorldToUnityService.GetUnityPositionsFromCoordinates(tempLocationsList, routeVisualizationType);
            VisualizeRoute(currentWaypointPositions, waypointNames, clearStaleWaypoints);
        }

        private void VisualizeRoute(List<Vector3> waypointPositions, List<string> waypointNames, bool clearStaleWaypoints = true)
        {
            routeVisualizationType = sceneController.routeVisualizationType;
            
            if(clearStaleWaypoints)
                ClearCurrentWaypoints(routeVisualizationType);

            Transform container;

            switch (routeVisualizationType)
            {
                case ElevationAPI.NO_ELEVATION:
                    container = containerNoElevation;
                    break;
                case ElevationAPI.OPEN_ELEVATION:
                    container = containerOpenElevation;
                    break;
                case ElevationAPI.OPEN_TOPO_DATA_EUDEM:
                    container = containerOpenTopoData_EUDEM;
                    break;
                case ElevationAPI.OPEN_TOPO_DATA_ASTER:
                    container = containerOpenTopoData_ASTER;
                    break;
                default:
                    container = containerNoElevation;
                    break;
            }

            Transform lineContainer = new GameObject("lineContainer").transform;
            lineContainer.SetParent(container);
            lineContainer.localPosition = Vector3.zero;

            GameObject line = Instantiate(linePrefab, waypointPositions[0], Quaternion.Euler(Vector3.zero));
            line.transform.SetParent(lineContainer.transform, false);
            LineRenderer lren = line.GetComponent<LineRenderer>();
            lren.positionCount = waypointPositions.Count;

            lren.SetPosition(0, Vector3.zero);

            for (int i = 1; i < waypointPositions.Count; i++)
            {
                Vector3 waypointPosition = waypointPositions[i];
                string waypointName = $"Waypoint:{waypointNames[i]}";
                VisualizeWaypoint(container, waypointPosition, waypointName);
                lren.SetPosition(i, waypointPosition - waypointPositions[0]);
            }
        }

        private void VisualizeWaypoint(Transform container, Vector3 waypointPosition, string waypointName)
        {
            GameObject waypoint = Instantiate(waypointPrefab, waypointPosition, Quaternion.identity);
            waypoint.transform.SetParent(container, false);
            waypoint.name = waypointName;
        }

        private void ClearCurrentWaypoints(ElevationAPI routeVisualizationType)
        {
            foreach (Transform child in containerNoElevation.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in containerOpenElevation.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in containerOpenTopoData_EUDEM.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in containerOpenTopoData_ASTER.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public enum ElevationAPI
    {
        NO_ELEVATION = 0,
        OPEN_ELEVATION = 1,
        OPEN_TOPO_DATA_EUDEM = 2,
        OPEN_TOPO_DATA_ASTER = 3
    }
}