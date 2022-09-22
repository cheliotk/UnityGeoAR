using Assets.Scripts.Auxiliary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Assets.Scripts
{
    public class ARSceneController : SceneControllerBase
    {
        [SerializeField] GameObject AR_origin;

        private void Start()
        {
            SetStartValues();
        }

        private async void SetStartValues()
        {
            LocationCompassData lastLocationData = LocationUpdater.Instance.lastLocationCompassData;
            compassHeadingAtSceneLoad = lastLocationData.compass.trueHeading;
            locationAtSceneLoad = new Vector2(lastLocationData.location.latitude, lastLocationData.location.longitude);
            locationGGRS87AtSceneLoad = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87(locationAtSceneLoad.x, locationAtSceneLoad.y);

            Vector2 tempLoc = new Vector2(locationAtSceneLoad.y, locationAtSceneLoad.x);
            List<Vector2> locations = new List<Vector2>() { tempLoc };

            var resASTER = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER);
            elevationOpenTopoData_ASTER = resASTER.results[0].elevation;

            var resEUDEM = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM);
            elevationOpenTopoData_EUDEM = resEUDEM.results[0].elevation;

            var res = await ElevationQueryHandler.Instance.MakeOpenElevationQuery(locations);
            elevationOpenElevation = res.results[0].elevation;

            SetCameraRotation();
        }

        private void SetCameraRotation()
        {
            Vector3 rot = AR_origin.transform.rotation.eulerAngles;
            rot.y = compassHeadingAtSceneLoad;
            AR_origin.transform.rotation = Quaternion.Euler(rot);

        }
    }
}