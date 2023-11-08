using Assets.Scripts.Auxiliary;
using Assets.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DesktopTestSceneController : SceneControllerBase
    {
        [Header("Sample route start-end")]
        [SerializeField] private Vector2 startLatLong = new Vector2(38.01408f, 23.74127f);
        [SerializeField] private Vector2 endLatLong = new Vector2(38.02369f, 23.73612f);

        private void Start()
        {
            SetDefaultValues();
        }

        private async void SetDefaultValues()
        {
            if(Mathf.Approximately((Vector2.zero - locationSourceCRSAtSceneLoad).magnitude, 0f))
            {
                locationSourceCRSAtSceneLoad = new Vector2(38.01408f, 23.74127f);
            }

            locationDestinationCRSAtSceneLoad = ReprojectionService.ReprojectPoint(locationSourceCRSAtSceneLoad.x, locationSourceCRSAtSceneLoad.y);

            Vector2 tempLoc = new Vector2(locationSourceCRSAtSceneLoad.y, locationSourceCRSAtSceneLoad.x);
            List<Vector2> locations = new List<Vector2>() { tempLoc };

            var resASTER = await ElevationQueryService.MakeOpenTopoDataQuery(locations, ElevationAPI.OPEN_TOPO_DATA_ASTER);
            elevationOpenTopoData_ASTER = resASTER.results[0].elevation;

            var resEUDEM = await ElevationQueryService.MakeOpenTopoDataQuery(locations, ElevationAPI.OPEN_TOPO_DATA_EUDEM);
            elevationOpenTopoData_EUDEM = resEUDEM.results[0].elevation;
        }

        [ContextMenu("Make Default Query")]
        public void MakeRoutingQuery()
        {
            RoutingService.StartQueryWithParameters(startLatLong, endLatLong);
        }

        public override Vector2 GetCurrentLocation()
        {
            return GetLocationSourceCRSAtSceneLoad();
        }
    }
}