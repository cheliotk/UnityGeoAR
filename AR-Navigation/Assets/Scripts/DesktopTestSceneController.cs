using Assets.Scripts.Auxiliary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DesktopTestSceneController : SceneControllerBase
    {
        private void Start()
        {
            SetDefaultValues();
        }

        private async void SetDefaultValues()
        {
            if(Mathf.Approximately((Vector2.zero - locationAtSceneLoad).magnitude, 0f))
            {
                locationAtSceneLoad = new Vector2(37.975321f, 23.780022f);
            }

            locationGGRS87AtSceneLoad = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87(locationAtSceneLoad.x, locationAtSceneLoad.y);

            Vector2 tempLoc = new Vector2(locationAtSceneLoad.y, locationAtSceneLoad.x);
            List<Vector2> locations = new List<Vector2>() { tempLoc };

            var resASTER = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER);
            elevationOpenTopoData_ASTER = resASTER.results[0].elevation;

            var resEUDEM = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM);
            elevationOpenTopoData_EUDEM = resEUDEM.results[0].elevation;
            
            //var res = await ElevationQueryHandler.Instance.MakeOpenElevationQuery(locations);
            //elevationOpenElevation = res.results[0].elevation;
        }

        [ContextMenu("Make Default Query")]
        public void MakeRoutingQuery()
        {
            RoutingHandler.Instance.StartDefaultQuery();
        }

        public override Vector2 GetCurrentLocation()
        {
            return GetLocationAtSceneLoad();
        }
    }
}