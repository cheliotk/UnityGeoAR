using Assets.Scripts.Models;
using Assets.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class LocationUpdater : MonoBehaviour
    {
        private const int MAX_COMPASS_RECORDS = 10;
        public static LocationUpdater Instance { get; private set; }
        public event EventHandler<LocationCompassData> onLocationCompassDataUpdatedEvent;
        public LocationCompassData lastLocationCompassData { get; private set; } = new LocationCompassData();

        [SerializeField] private float updateInterval = 1f;

        private LocationUpdatesService locationUpdatesService;
        private bool isUpdating = true;
        private readonly List<CompassData> latestCompassHeadings = new List<CompassData>();

        private void Awake()
        {
            LocationUpdater[] instances = FindObjectsOfType<LocationUpdater>();
            if (instances.Length > 1)
            {
                Destroy(this.gameObject);
                return;
            }

            Application.logMessageReceived += Application_logMessageReceived;

            Instance = this;
            locationUpdatesService = new LocationUpdatesService(1f, 1f);

            DontDestroyOnLoad(this.gameObject);
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            Debug.Log($"{type} : {stackTrace}");
        }

        IEnumerator LocationUpdateCoroutine()
        {
            Debug.Log("Starting Location Updates");

            // delay initialization for 1s to wait for Unity/android systems to initialize
            float i = 1f;
            while (i > 0f)
            {
                i -= Time.deltaTime;
                yield return null;
            }

            // First location update, for listeners to initialize
            lastLocationCompassData.isFirstUpdate = true;
            onLocationCompassDataUpdatedEvent?.Invoke(this, lastLocationCompassData);

            // Attempt to initialize the LocationUpdatesService
            LocationServiceInitResult initializationResult = LocationServiceInitResult.NOT_ENABLED_BY_USER;
            bool enabledByUser = locationUpdatesService.InitializeService();
            if (!enabledByUser)
            {
                Debug.Log("LocationUpdates disabled by user");
                yield break;
            }

            initializationResult = LocationServiceInitResult.INITIALIZING;
            int waitForInitializationSecs = 20;
            while (!locationUpdatesService.IsInitialized() && waitForInitializationSecs > 0)
            {
                yield return new WaitForSeconds(1);
                waitForInitializationSecs -= 1;
            }

            initializationResult = waitForInitializationSecs <= 0 ? LocationServiceInitResult.INITIALIZING : LocationServiceInitResult.SUCCESS;

            if(initializationResult == LocationServiceInitResult.INITIALIZING)
            {
                Debug.Log("LocationUpdates timed out");
                yield break;
            }

            float timeForNextUpdate = Time.time + updateInterval;

            while (isUpdating)
            {
                while (Time.time < timeForNextUpdate && isUpdating)
                {
                    lastLocationCompassData.compass = locationUpdatesService.GetLatestCompassData();
                    latestCompassHeadings.Add(lastLocationCompassData.compass);
                    if(latestCompassHeadings.Count > MAX_COMPASS_RECORDS)
                    {
                        latestCompassHeadings.RemoveAt(0);
                    }

                    yield return null;
                }

                // Connection has failed
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    Debug.Log("Unable to determine device location");
                    yield break;
                }
                else
                {
                    // Access granted and location value could be retrieved
                    lastLocationCompassData.location = locationUpdatesService.GetLatestLocationData();

                    lastLocationCompassData.isFirstUpdate = false;
                    onLocationCompassDataUpdatedEvent?.Invoke(this, lastLocationCompassData);
                }
                timeForNextUpdate = Time.time + updateInterval;

            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }

        public void StopLocationUpdates() => isUpdating = false;
        public void StartLocationUpdates()
        {
            isUpdating = true;
            StartCoroutine(LocationUpdateCoroutine());
        }

        public float GetAverageMagneticHeading()
        {
            float[] angles = new float[latestCompassHeadings.Count];
            for (int i = 0; i < latestCompassHeadings.Count; i++)
            {
                CompassData item = latestCompassHeadings[i];
                angles[i] = item.magneticHeading;
            }

            var x = angles.Sum(a => Math.Cos(a * Math.PI / 180)) / angles.Length;
            var y = angles.Sum(a => Math.Sin(a * Math.PI / 180)) / angles.Length;
            return (float)(Math.Atan2(y, x) * 180 / Math.PI);
        }

        public float GetAverageTrueHeading()
        {
            float[] angles = new float[latestCompassHeadings.Count];
            for (int i = 0; i < latestCompassHeadings.Count; i++)
            {
                CompassData item = latestCompassHeadings[i];
                angles[i] = item.trueHeading;
            }

            var x = angles.Sum(a => Math.Cos(a * Math.PI / 180)) / angles.Length;
            var y = angles.Sum(a => Math.Sin(a * Math.PI / 180)) / angles.Length;
            return (float)(Math.Atan2(y, x) * 180 / Math.PI);
        }
    }
}