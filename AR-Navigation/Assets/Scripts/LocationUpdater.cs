using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class LocationUpdater : MonoBehaviour
    {
        private const int MAX_COMPASS_RECORDS = 10;
        public static LocationUpdater Instance { get; private set; }
        private bool isUpdating = true;
        [SerializeField] private float updateInterval = 1f;

        public event EventHandler<LocationCompassData> onLocationCompassDataUpdatedEvent;
        public LocationCompassData lastLocationCompassData { get; private set; } = new LocationCompassData();
        
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
            DontDestroyOnLoad(this.gameObject);
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            Debug.Log($"{type} : {stackTrace}");
        }

        //private void Start()
        //{
        //    StartLocationUpdates();
        //}

        IEnumerator LocationUpdateCoroutine()
        {
            Debug.Log("Starting Location Updates");
            float i = 1f;
            while (i > 0f)
            {
                i -= Time.deltaTime;
                yield return null;
            }
            lastLocationCompassData.isFirstUpdate = true;
            onLocationCompassDataUpdatedEvent?.Invoke(this, lastLocationCompassData);

            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location disabled by user");
                yield break;
            }

            // Start service before querying location
            Input.location.Start(1f, 1f);
            Input.compass.enabled = true;
            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {

                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                Debug.Log("Timed out");
                yield break;
            }

            float timeForNextUpdate = Time.time + updateInterval;

            while (isUpdating)
            {
                while (Time.time < timeForNextUpdate && isUpdating)
                {
                    lastLocationCompassData.compass.magneticHeading = Input.compass.magneticHeading;
                    lastLocationCompassData.compass.trueHeading = Input.compass.trueHeading;
                    lastLocationCompassData.compass.rawVector = Input.compass.rawVector;

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
                    //Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

                    lastLocationCompassData.location.latitude = Input.location.lastData.latitude;
                    lastLocationCompassData.location.longitude = Input.location.lastData.longitude;
                    lastLocationCompassData.location.altitude = Input.location.lastData.altitude;

                    lastLocationCompassData.timestamp = Input.location.lastData.timestamp;

                    lastLocationCompassData.isFirstUpdate = false;

                    onLocationCompassDataUpdatedEvent?.Invoke(this, lastLocationCompassData);
                }
                timeForNextUpdate = Time.time + updateInterval;

            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();

            //SetUIText("FINISHING COROUTINE");
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

    public class LocationCompassData
    {
        public double timestamp;
        public LocationData location;
        public CompassData compass;
        public bool isFirstUpdate;

        public LocationCompassData()
        {
            timestamp = 0f;
            location.latitude = 0f;
            location.longitude = 0f;
            location.altitude = 0f;

            compass.magneticHeading = 72f;
            compass.trueHeading = 72f;
            compass.rawVector = Vector3.one;

            isFirstUpdate = false;
        }
    }

    public struct LocationData
    {
        public float latitude;
        public float longitude;
        public float altitude;
    }

    public struct CompassData
    {
        public float magneticHeading;
        public float trueHeading;
        public Vector3 rawVector;
    }

}