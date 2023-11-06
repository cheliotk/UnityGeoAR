using Assets.Scripts.Auxiliary;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class SceneControllerBase : MonoBehaviour, ISceneController
    {
        [SerializeField] protected APIKeyContainer apiKeyContainer;
        [SerializeField] protected ElevationAPI _routeVisualizationType;
        [SerializeField] protected Vector2 locationSourceCRSAtSceneLoad;
        [SerializeField] protected CommonCRS sourceCRS = CommonCRS.WGS84;
        [SerializeField] protected CommonCRS destinationCRS = CommonCRS.GGRS87;
        protected Vector2 locationNow;
        protected Vector2 locationDestinationCRSAtSceneLoad;
        protected float elevationOpenElevation;
        protected float elevationOpenTopoData_EUDEM;
        protected float elevationOpenTopoData_ASTER;
        protected float compassHeadingAtSceneLoad;

        protected ReprojectionService _reprojectionService;
        public ReprojectionService ReprojectionService
        {
            get
            {
                if(_reprojectionService == null)
                {
                    _reprojectionService = new ReprojectionService((int)sourceCRS, (int)destinationCRS);
                }

                return _reprojectionService;
            }
        }

        private RoutingService _routingService;
        public RoutingService RoutingService
        {
            get
            {
                if(_routingService == null)
                {
                    _routingService = new RoutingService(apiKeyContainer.OpenRouteServiceApiKey);
                }

                return _routingService;
            }
        }

        private ElevationQueryService _elevationQueryService;
        public ElevationQueryService ElevationQueryService
        {
            get
            {
                if(_elevationQueryService == null)
                {
                    _elevationQueryService = new ElevationQueryService();
                }

                return _elevationQueryService;
            }
        }


        public ElevationAPI routeVisualizationType
        {
            get
            {
                return _routeVisualizationType;
            }
            set { }
        }

        public float GetElevationAtSceneLoad()
        {
            switch (_routeVisualizationType)
            {
                case ElevationAPI.NO_ELEVATION:
                    return 1f;
                case ElevationAPI.OPEN_ELEVATION:
                    return elevationOpenElevation;
                case ElevationAPI.OPEN_TOPO_DATA_EUDEM:
                    return elevationOpenTopoData_EUDEM;
                case ElevationAPI.OPEN_TOPO_DATA_ASTER:
                    return elevationOpenTopoData_ASTER;
                default:
                    return 1f;
            }
        }

        public Vector2 GetLocationSourceCRSAtSceneLoad() => locationSourceCRSAtSceneLoad;

        public Vector2 GetLocationDestinationCRSAtSceneLoad() => locationDestinationCRSAtSceneLoad;

        public float GetCompassHeadingAtSceneLoad() => compassHeadingAtSceneLoad;

        public abstract Vector2 GetCurrentLocation();
        public void SetRouteVisualizationType(ElevationAPI routeVisualizationType)
        {
            _routeVisualizationType = routeVisualizationType;
        }
    }
}