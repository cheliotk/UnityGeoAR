using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneControllerBase : MonoBehaviour, ISceneController
    {
        [SerializeField] protected RouteVisualizationType _routeVisualizationType;
        [SerializeField] protected Vector2 locationAtSceneLoad;
        protected Vector2 locationGGRS87AtSceneLoad;
        protected float elevationOpenElevation;
        protected float elevationOpenTopoData_EUDEM;
        protected float elevationOpenTopoData_ASTER;
        protected float compassHeadingAtSceneLoad;

        public RouteVisualizationType routeVisualizationType
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
                case RouteVisualizationType.NO_ELEVATION:
                    return 0f;
                case RouteVisualizationType.ELEVATION_OPEN_ELEVATION:
                    return elevationOpenElevation;
                case RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM:
                    return elevationOpenTopoData_EUDEM;
                case RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER:
                    return elevationOpenTopoData_ASTER;
                default:
                    return 0f;
            }
        }

        public Vector2 GetLocationAtSceneLoad() => locationAtSceneLoad;

        public Vector2 GetPositionGGRS87AtSceneLoad() => locationGGRS87AtSceneLoad;

        public float GetCompassHeadingAtSceneLoad() => compassHeadingAtSceneLoad;

    }
}