using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class WorldToUnityService
    {
        private ReprojectionService reprojectionService;
        private ElevationQueryService elevationQueryService;
        private Vector2 originLocationSourceCRS;
        private Vector2 originLocationDestinationCRS;
        private float elevationOpenTopoData_ASTER;
        private float elevationOpenTopoData_EUDEM;

        public WorldToUnityService(
            ReprojectionService reprojectionService,
            ElevationQueryService elevationQueryService,
            Vector2 locationSourceLatLong,
            float elevationOpenTopoData_ASTER,
            float elevationOpenTopoData_EUDEM)
        {
            this.reprojectionService = reprojectionService;
            this.elevationQueryService = elevationQueryService;
            originLocationSourceCRS = locationSourceLatLong;
            originLocationDestinationCRS = reprojectionService.ReprojectPoint(originLocationSourceCRS.x, originLocationSourceCRS.y);
            this.elevationOpenTopoData_ASTER = elevationOpenTopoData_ASTER;
            this.elevationOpenTopoData_EUDEM = elevationOpenTopoData_EUDEM;
        }

        public async Task<Vector3> GetUnityPositionFromCoordinates(Vector2 xyCoords, ElevationAPI elevationAPI)
        {
            if (elevationAPI == ElevationAPI.OPEN_ELEVATION)
                throw new NotSupportedException("OPEN_ELEVATION no longer supported");
            if (elevationAPI == ElevationAPI.NO_ELEVATION)
                return ConvertWorldToUnityNoElevation(xyCoords);
            else
                return await ConvertWorldToUnityWithElevation(xyCoords, elevationAPI);
        }

        public async Task<List<Vector3>> GetUnityPositionsFromCoordinates(List<Vector2> xyCoords, ElevationAPI elevationAPI)
        {
            if (elevationAPI == ElevationAPI.OPEN_ELEVATION)
                throw new NotSupportedException("OPEN_ELEVATION no longer supported");
            if(elevationAPI == ElevationAPI.NO_ELEVATION)
                return ConvertWorldToUnityNoElevation(xyCoords);
            else
                return await ConvertWorldToUnityWithElevation(xyCoords, elevationAPI);
        }

        private Vector3 ConvertWorldToUnityNoElevation(Vector2 xyCoords)
        {
            Vector2 pointDestinationCRS = reprojectionService.ReprojectPoint(xyCoords.y, xyCoords.x);
            return new Vector3(pointDestinationCRS.x - originLocationDestinationCRS.x, 0f, pointDestinationCRS.y - originLocationDestinationCRS.y);
        }

        private List<Vector3> ConvertWorldToUnityNoElevation(List<Vector2> xyCoords)
        {
            double[] lats = new double[xyCoords.Count];
            double[] longs = new double[xyCoords.Count];
            for (int i = 0; i < xyCoords.Count; i++)
            {
                lats[i] = xyCoords[i].y;
                longs[i] = xyCoords[i].x;
            }
            Vector2[] pointsDestinationCRS = reprojectionService.ReprojectPoints(lats, longs);
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < pointsDestinationCRS.Length; i++)
            {
                Vector2 p = pointsDestinationCRS[i];
                points.Add(new Vector3(p.x - originLocationDestinationCRS.x, 0f, p.y - originLocationDestinationCRS.y));
            }

            return points;
        }

        private async Task<Vector3> ConvertWorldToUnityWithElevation(Vector2 xyCoords, ElevationAPI elevationAPI)
        {
            if (elevationAPI == ElevationAPI.OPEN_ELEVATION)
                throw new NotSupportedException("OPEN_ELEVATION no longer supported");
            if (elevationAPI == ElevationAPI.NO_ELEVATION)
                throw new InvalidOperationException("For no elevation use method ConvertWorldToUnityNoElevation instead");

            List<Vector2> locations = new List<Vector2> { xyCoords };
            OpenTopoDataResponse elevationResponse = await elevationQueryService.MakeOpenTopoDataQuery(locations, elevationAPI);
            float elevation = elevationResponse.results[0].elevation;
            float unityY = elevationAPI == ElevationAPI.OPEN_TOPO_DATA_EUDEM ? elevation - elevationOpenTopoData_EUDEM : elevation - elevationOpenTopoData_ASTER;

            Vector3 point = ConvertWorldToUnityNoElevation(xyCoords);
            point.y = unityY;

            return point;
        }

        private async Task<List<Vector3>> ConvertWorldToUnityWithElevation(List<Vector2> xyCoords, ElevationAPI elevationAPI)
        {
            if (elevationAPI == ElevationAPI.OPEN_ELEVATION)
                throw new NotSupportedException("OPEN_ELEVATION no longer supported");
            if (elevationAPI == ElevationAPI.NO_ELEVATION)
                throw new InvalidOperationException("For no elevation use method ConvertWorldToUnityNoElevation instead");

            OpenTopoDataResponse elevationsResponse = await elevationQueryService.MakeOpenTopoDataQuery(xyCoords, elevationAPI);

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < elevationsResponse.results.Count; i++)
            {
                OpenTopoDataResult coord = elevationsResponse.results[i];
                Vector2 pointInCartesianSpace = reprojectionService.ReprojectPoint(coord.location.lat, coord.location.lng);

                //float elevation = coord.elevation;
                float unityY = elevationAPI == ElevationAPI.OPEN_TOPO_DATA_EUDEM ? coord.elevation - elevationOpenTopoData_EUDEM : coord.elevation - elevationOpenTopoData_ASTER;
                Vector3 point = new Vector3(pointInCartesianSpace.x - originLocationDestinationCRS.x, unityY, pointInCartesianSpace.y - originLocationDestinationCRS.y);
                points.Add(point);
            }

            return points;
        }
    }
}