using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class WorldToUnityService
    {
        private ReprojectionService reprojectionService;
        private ElevationQueryService elevationQueryService;
        private Vector2 originLocationSourceCRS;
        private Vector2 originLocationDestinationCRS;

        public WorldToUnityService(ReprojectionService reprojectionService, ElevationQueryService elevationQueryService, Vector2 locationSourceLatLong)
        {
            this.reprojectionService = reprojectionService;
            this.elevationQueryService = elevationQueryService;
            originLocationSourceCRS = locationSourceLatLong;
            originLocationDestinationCRS = reprojectionService.ReprojectPoint(originLocationSourceCRS.x, originLocationSourceCRS.y);
        }
    }
}