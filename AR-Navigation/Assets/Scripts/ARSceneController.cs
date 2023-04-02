using Assets.Scripts.Auxiliary;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Assets.Scripts
{
    public class ARSceneController : SceneControllerBase
    {
        [SerializeField] GameObject AR_origin;
        [SerializeField] TMP_Dropdown dropdown;

        private void Start()
        {
            SetStartValues();
            SetupDropdown();
        }

        private void SetupDropdown()
        {
            if (dropdown == null)
                return;

            dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var elevationType in Enum.GetValues(typeof(RouteVisualizationType)))
            {
                options.Add(new TMP_Dropdown.OptionData(elevationType.ToString()));
            }

            dropdown.AddOptions(options);
            dropdown.SetValueWithoutNotify((int)routeVisualizationType);

            dropdown.onValueChanged.AddListener(DropdownOnValueChanged);
        }

        private void OnDestroy()
        {
            if(dropdown != null)
                dropdown.onValueChanged.RemoveListener(DropdownOnValueChanged);
        }

        private void DropdownOnValueChanged(int selectedOption)
        {
            SetRouteVisualizationType((RouteVisualizationType)selectedOption);
        }

        private async void SetStartValues()
        {
            LocationCompassData lastLocationData = LocationUpdater.Instance.lastLocationCompassData;
            compassHeadingAtSceneLoad = lastLocationData.compass.trueHeading;
            locationAtSceneLoad = ConvertLocationDataToVector2(lastLocationData.location);
            locationGGRS87AtSceneLoad = ProjectionUtilities.ReprojectFrom_WGS84_To_GGRS87(locationAtSceneLoad.x, locationAtSceneLoad.y);

            Vector2 tempLoc = new Vector2(locationAtSceneLoad.y, locationAtSceneLoad.x);
            List<Vector2> locations = new List<Vector2>() { tempLoc };

            var resASTER = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_ASTER);
            elevationOpenTopoData_ASTER = resASTER.results[0].elevation+1;

            var resEUDEM = await ElevationQueryHandler.Instance.MakeOpenTopoDataQuery(locations, RouteVisualizationType.ELEVATION_OPEN_TOPO_DATA_EUDEM);
            elevationOpenTopoData_EUDEM = resEUDEM.results[0].elevation+1;

            //var res = await ElevationQueryHandler.Instance.MakeOpenElevationQuery(locations);
            //elevationOpenElevation = res.results[0].elevation + 1;

            SetCameraRotation();
        }

        private void SetCameraRotation()
        {
            Vector3 rot = AR_origin.transform.rotation.eulerAngles;
            rot.y = compassHeadingAtSceneLoad;
            AR_origin.transform.rotation = Quaternion.Euler(rot);

        }

        public override Vector2 GetCurrentLocation()
        {
            return ConvertLocationDataToVector2(LocationUpdater.Instance.lastLocationCompassData.location);
        }

        private Vector2 ConvertLocationDataToVector2(LocationData locationData)
        {
            return new Vector2(locationData.latitude, locationData.longitude);
        }
    }
}