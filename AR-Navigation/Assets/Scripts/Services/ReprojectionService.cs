using Assets.Scripts.Auxiliary;
using DotSpatial.Projections;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class ReprojectionService
    {
        public ProjectionInfo SourceProjection { get; protected set; }
        public ProjectionInfo DestinationProjection { get; protected set; }

        public ReprojectionService(int sourceProjectionEpsgCode, int destinationProjectionEpsgCode)
        {
            SourceProjection = ProjectionInfo.FromEpsgCode(sourceProjectionEpsgCode);
            DestinationProjection = ProjectionInfo.FromEpsgCode(destinationProjectionEpsgCode);
        }

        public Vector2 ReprojectPoint(double lat, double lng)
        {
            return ProjectionUtilities.ReprojectFromToCoordinateSystem(lat, lng, SourceProjection, DestinationProjection);
        }

        public Vector2 ReprojectPointReverse(double lat, double lng)
        {
            return ProjectionUtilities.ReprojectFromToCoordinateSystem(lat, lng, DestinationProjection, SourceProjection);
        }

        public Vector2[] ReprojectPoints(double[] lats, double[] lngs)
        {
            return ProjectionUtilities.ReprojectFromToCoordinateSystem(lats, lngs, SourceProjection, DestinationProjection);
        }

        public Vector2[] ReprojectPointsReverse(double[] lats, double[] lngs)
        {
            return ProjectionUtilities.ReprojectFromToCoordinateSystem(lats, lngs, DestinationProjection, SourceProjection);
        }
    }
}