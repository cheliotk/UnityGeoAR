using Assets.Scripts.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public enum LocationServiceInitializationResult
    {
        NOT_ENABLED_BY_USER = 0,
        FAILED_TO_INITIALIZE = 1,
        SUCCESS = 2
    }
    public class LocationUpdatesService
    {
        private Compass compass;
        private LocationService locationService;

        private float desiredAccuracyInMeters;
        private float updateDistanceInMeters;

        private LocationServiceInitializationResult initializationResult;

        public LocationUpdatesService(float desiredAccuracyInMeters, float updateDistanceInMeters)
        {
            compass = Input.compass;
            locationService = Input.location;
            this.desiredAccuracyInMeters = desiredAccuracyInMeters;
            this.updateDistanceInMeters = updateDistanceInMeters;
        }

        public async Task<LocationServiceInitializationResult> WaitForLocationServiceInitialization(int waitForInitializationMs = 20000)
        {
            // First, check if user has location service enabled
            if (!locationService.isEnabledByUser)
            {
                initializationResult = LocationServiceInitializationResult.NOT_ENABLED_BY_USER;
                return initializationResult;
            }

            // Start service before querying location
            Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
            Input.compass.enabled = true;

            // Wait until service initializes
            while (Input.location.status == LocationServiceStatus.Initializing && waitForInitializationMs > 0)
            {
                await Task.Delay(1000);
                waitForInitializationMs -= 1000;
            }

            // Service didn't initialize before timeout
            if (waitForInitializationMs < 1)
            {
                initializationResult = LocationServiceInitializationResult.FAILED_TO_INITIALIZE;
                return initializationResult;
            }

            initializationResult = LocationServiceInitializationResult.SUCCESS;
            return initializationResult;
        }

        public bool TryGetLatestCompassData(ref CompassData outCompassData)
        {
            if (!compass.enabled)
            {
                return false;
            }
            else
            {
                outCompassData.magneticHeading = compass.magneticHeading;
                outCompassData.trueHeading = compass.trueHeading;
                outCompassData.rawVector = compass.rawVector;
                outCompassData.timestamp = compass.timestamp;
                return true;
            }
        }

        public CompassData GetLatestCompassData()
        {
            if(initializationResult != LocationServiceInitializationResult.SUCCESS)
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
            if(locationService.status == LocationServiceStatus.Failed)
            {
                return false;
            }
            else
            {
                outLocationData.latitude = locationService.lastData.latitude;
                outLocationData.longitude = locationService.lastData.longitude;
                outLocationData.altitude = locationService.lastData.altitude;
                outLocationData.timestamp = locationService.lastData.timestamp;
                return true;
            }
        }

        public LocationData GetLatestLocationData()
        {
            if (initializationResult != LocationServiceInitializationResult.SUCCESS)
                throw new InvalidOperationException("LocationUpdates has not initialized.");
            
            LocationData locationData = new LocationData();
            locationData.latitude = locationService.lastData.latitude;
            locationData.longitude = locationService.lastData.longitude;
            locationData.altitude = locationService.lastData.altitude;
            locationData.timestamp = locationService.lastData.timestamp;
            return locationData;
        }
    }
}
