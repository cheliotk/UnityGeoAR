using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LandingPageController : MonoBehaviour
    {
        [SerializeField] Button startAppButton;
        [SerializeField] GameObject locationPermissionDeniedMessage;

        private void Start()
        {
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Permission already granted");
                SetupMenuWithPermissionsEnabled();
            }
            else
            {
                Debug.Log("Requesting permission");
                var fineLocationCallbacks = new PermissionCallbacks();
                fineLocationCallbacks.PermissionGranted += FineLocationCallbacks_PermissionGranted;
                fineLocationCallbacks.PermissionDenied += FineLocationCallbacks_PermissionDenied;
                fineLocationCallbacks.PermissionDeniedAndDontAskAgain += FineLocationCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.FineLocation, fineLocationCallbacks);
            }
        }

        private void FineLocationCallbacks_PermissionDeniedAndDontAskAgain(string obj)
        {
            Debug.Log("Permission denied");
            startAppButton.interactable = false;
            locationPermissionDeniedMessage?.SetActive(true);
        }

        private void FineLocationCallbacks_PermissionDenied(string obj)
        {
            Debug.Log("Permission denied");
            startAppButton.interactable = false;
            locationPermissionDeniedMessage?.SetActive(true);
        }

        private void FineLocationCallbacks_PermissionGranted(string obj) => SetupMenuWithPermissionsEnabled();

        private void SetupMenuWithPermissionsEnabled()
        {
            Debug.Log("Permission granted");
            startAppButton.GetComponentInChildren<Text>().text = "Initializing...";
            startAppButton.interactable = false;
            locationPermissionDeniedMessage?.SetActive(false);

            LocationUpdater.Instance.StartLocationUpdates();

            LocationUpdater.Instance.onLocationCompassDataUpdatedEvent += Instance_onLocationCompassDataUpdatedEvent;
        }

        private void Instance_onLocationCompassDataUpdatedEvent(object sender, LocationCompassData e)
        {
            startAppButton.GetComponentInChildren<Text>().text = "Start application";
            startAppButton.interactable = true;
        }

        public void LoadScene(int sceneIndex = 1)
        {
            LocationUpdater.Instance.onLocationCompassDataUpdatedEvent -= Instance_onLocationCompassDataUpdatedEvent;
            SceneManager.LoadScene(sceneIndex);
        }
    }
}