﻿using Assets.Scripts.Interfaces;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class SceneControllerBase : MonoBehaviour, ISceneController
    {
        [SerializeField] protected APIKeyContainer apiKeyContainer;
        [SerializeField] protected RouteVisualizationType _routeVisualizationType;
        [SerializeField] protected Vector2 locationAtSceneLoad;
        protected Vector2 locationNow;
        protected Vector2 locationGGRS87AtSceneLoad;
        protected float elevationOpenElevation;
        protected float elevationOpenTopoData_EUDEM;
        protected float elevationOpenTopoData_ASTER;
        protected float compassHeadingAtSceneLoad;
        protected RoutingService routingService;

        public RouteVisualizationType routeVisualizationType
        {
            get
            {
                return _routeVisualizationType;
            }
            set { }
        }

        public RoutingService GetRoutingService()
        {
            if(routingService == null)
            {
                routingService = new RoutingService(apiKeyContainer.OpenRouteServiceApiKey,
                    RoutingService.OpenRouteServiceDirectionsApiBase,
                    RoutingService.OpenRouteServiceDirectionProfileWalking);
            }

            return routingService;
        }

        public float GetElevationAtSceneLoad()
        {
            switch (_routeVisualizationType)
            {
                case RouteVisualizationType.NO_ELEVATION:
                    return 1f;
                case RouteVisualizationType.ELEVATION_OPEN_ELEVATION:
                    return elevationOpenElevation;
                case RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM:
                    return elevationOpenTopoData_EUDEM;
                case RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER:
                    return elevationOpenTopoData_ASTER;
                default:
                    return 1f;
            }
        }

        public Vector2 GetLocationAtSceneLoad() => locationAtSceneLoad;

        public Vector2 GetPositionGGRS87AtSceneLoad() => locationGGRS87AtSceneLoad;

        public float GetCompassHeadingAtSceneLoad() => compassHeadingAtSceneLoad;

        public abstract Vector2 GetCurrentLocation();
        public void SetRouteVisualizationType(RouteVisualizationType routeVisualizationType)
        {
            _routeVisualizationType = routeVisualizationType;
        }
    }
}