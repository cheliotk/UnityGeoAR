using Assets.Scripts.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public enum LocationServiceInitResult
    {
        NOT_ENABLED_BY_USER = 0,
        INITIALIZING = 1,
        SUCCESS = 2
    }
    public class LocationUpdatesService
    {
        private Compass compass;
        private LocationService locationService;

        private float desiredAccuracyInMeters;
        private float updateDistanceInMeters;

        private LocationServiceInitResult initializationResult;

        public LocationUpdatesService(float desiredAccuracyInMeters, float updateDistanceInMeters)
        {
            compass = Input.compass;
            locationService = Input.location;
            this.desiredAccuracyInMeters = desiredAccuracyInMeters;
            this.updateDistanceInMeters = updateDistanceInMeters;
        }

        public bool InitializeServiceIfEnabledByUser()
        {
            // First, check if user has location service enabled
            if (!locationService.isEnabledByUser)
            {
                initializationResult = LocationServiceInitResult.NOT_ENABLED_BY_USER;
                return false;
            }

            // Start service before querying location
            Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
            Input.compass.enabled = true;

            initializationResult = LocationServiceInitResult.INITIALIZING;
            return true;
        }

        public bool IsInitialized()
        {
            if (Input.location.status == LocationServiceStatus.Running)
                initializationResult = LocationServiceInitResult.SUCCESS;

            return initializationResult == LocationServiceInitResult.SUCCESS;
        }

        public bool TryGetLatestCompassData(ref CompassData outCompassData)
        {
            if (!compass.enabled)
            {
                return false;
            }
            else
            {
                outCompassData = GetLatestCompassData();
                return true;
            }
        }

        public CompassData GetLatestCompassData()
        {
            if(initializationResult != LocationServiceInitResult.SUCCESS)
                throw new InvalidOperationException("LocationUpdates has not initialized.");
            
            CompassData outCompassData = new CompassData();
            outCompassData.magneticHeading = compass.magneticHeading;
            outCompassData.trueHeading = compass.trueHeading;
            outCompassData.rawVector = compass.rawVector;
            outCompassData.timestamp = compass.timestamp;
            return outCompassData;
        }

        public bool TryGetLatestLocationData(ref LocationData outLocationData)
        {
            if(locationService.status != LocationServiceStatus.Running)
            {
                return false;
            }
            else
            {
                outLocationData = GetLatestLocationData();
                return true;
            }
        }

        public LocationData GetLatestLocationData()
        {
            if (initializationResult != LocationServiceInitResult.SUCCESS)
                throw new InvalidOperationException("LocationUpdates has not initialized.");
            
            LocationData locationData = new LocationData();
            locationData.latitude = locationService.lastData.latitude;
            locationData.longitude = locationService.lastData.longitude;
            locationData.altitude = locationService.lastData.altitude;
            locationData.timestamp = locationService.lastData.timestamp;
            return locationData;
        }

        public bool TryGetLatestLocationCompassData(ref LocationCompassData outLocationCompassData)
        {
            if(locationService.status != LocationServiceStatus.Running
                || initializationResult != LocationServiceInitResult.SUCCESS)
            {
                return false;
            }

            outLocationCompassData = GetLatestLocationCompassData();
            return true;
        }

        public LocationCompassData GetLatestLocationCompassData()
        {
            if (initializationResult != LocationServiceInitResult.SUCCESS)
                throw new InvalidOperationException("LocationUpdates has not initialized.");

            LocationCompassData result = new LocationCompassData();
            result.location = GetLatestLocationData();
            result.compass = GetLatestCompassData();

            return result;
        }
    }
}
