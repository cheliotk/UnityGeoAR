using Assets.Scripts.Auxiliary;
using Assets.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DesktopTestSceneController : SceneControllerBase
    {
        [SerializeField] private Vector2 startLatLong = new Vector2(38.01408f, 23.74127f);
        [SerializeField] private Vector2 endLatLong = new Vector2(38.02369f, 23.73612f);

        private ReprojectionService reprojectionService;

        private void Start()
        {
            reprojectionService = new ReprojectionService((int)CommonCRS.WGS84, (int)CommonCRS.GGRS87);
            SetDefaultValues();
        }

        private async void SetDefaultValues()
        {
            if(Mathf.Approximately((Vector2.zero - locationAtSceneLoad).magnitude, 0f))
            {
                locationAtSceneLoad = new Vector2(37.975321f, 23.780022f);
            }

            locationGGRS87AtSceneLoad = reprojectionService.ReprojectPoint(locationAtSceneLoad.x, locationAtSceneLoad.y);

            Vector2 tempLoc = new Vector2(locationAtSceneLoad.y, locationAtSceneLoad.x);
            List<Vector2> locations = new List<Vector2>() { tempLoc };

            var resASTER = await ElevationQueryService.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER);
            elevationOpenTopoData_ASTER = resASTER.results[0].elevation;

            var resEUDEM = await ElevationQueryService.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM);
            elevationOpenTopoData_EUDEM = resEUDEM.results[0].elevation;
        }

        [ContextMenu("Make Default Query")]
        public void MakeRoutingQuery()
        {
            RoutingService.StartQuery(startLatLong, endLatLong);
        }

        public override Vector2 GetCurrentLocation()
        {
            return GetLocationAtSceneLoad();
        }
    }
}