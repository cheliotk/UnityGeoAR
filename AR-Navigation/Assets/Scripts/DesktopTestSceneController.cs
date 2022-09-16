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
            locationAtSceneLoad = new Vector2(38.01408f, 23.74127f);
            Vector2 tempLoc = new Vector2(locationAtSceneLoad.y, locationAtSceneLoad.x);
            List<Vector2> locations = new List<Vector2>() { tempLoc };

            var resASTER = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER);
            elevationOpenTopoData_ASTER = resASTER.results[0].elevation;

            var resEUDEM = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM);
            elevationOpenTopoData_EUDEM = resEUDEM.results[0].elevation;
            
            var res = await ElevationQueryHandler.Instance.MakeOpenElevationQuery(locations);
            elevationOpenElevation = res.results[0].elevation;
        }

        [ContextMenu("Make Default Query")]
        public void MakeRoutingQuery()
        {
            RoutingHandler.Instance.StartDefaultQuery();
        }
    }
}