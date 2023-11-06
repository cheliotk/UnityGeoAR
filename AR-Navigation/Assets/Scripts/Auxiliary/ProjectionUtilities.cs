using DotSpatial.Projections;
using UnityEngine;

namespace Assets.Scripts.Auxiliary
{
    public class ProjectionUtilities
    {
        public static Vector2 ReprojectFromToCoordinateSystem(double lat, double lng, int sourceCRSCode, int destinationCRSCode)
        {
            ProjectionInfo source = ProjectionInfo.FromEpsgCode(sourceCRSCode);
            ProjectionInfo destination = ProjectionInfo.FromEpsgCode(destinationCRSCode);

            return ReprojectFromToCoordinateSystem(lat, lng, source, destination);
        }

        public static Vector2 ReprojectFromToCoordinateSystem(double lat, double lng, ProjectionInfo source, ProjectionInfo destination)
        {
            double[] latLongs = new double[] { lng, lat };
            double[] heights = new double[] { 0 };
            Reproject.ReprojectPoints(latLongs, heights, source, destination, 0, heights.Length);

            return new Vector2((float)latLongs[0], (float)latLongs[1]);
        }

        public static Vector2[] ReprojectFromToCoordinateSystem(double[] lats, double[] lngs, int sourceCRSCode, int destinationCRSCode)
        {
            ProjectionInfo source = ProjectionInfo.FromEpsgCode(sourceCRSCode);
            ProjectionInfo destination = ProjectionInfo.FromEpsgCode(destinationCRSCode);
            return ReprojectFromToCoordinateSystem(lats, lngs, source, destination);
        }

        public static Vector2[] ReprojectFromToCoordinateSystem(double[] lats, double[] lngs, ProjectionInfo source, ProjectionInfo destination)
        {
            double[] lngLats = new double[lats.Length * 2];
            double[] heights = new double[lats.Length];
            for (int i = 0; i < lats.Length; i++)
            {
                lngLats[i * 2] = lngs[i];
                lngLats[i * 2 + 1] = lats[i];
                heights[i] = 0;
            }
            Reproject.ReprojectPoints(lngLats, heights, source, destination, 0, heights.Length);

            Vector2[] results = new Vector2[lats.Length];
            for (int i = 0; i < lats.Length; i++)
            {
                results[i] = new Vector2((float)lngLats[i * 2], (float)lngLats[i * 2 + 1]);
            }
            return results;
        }
    }

    public enum CommonCRS
    {
        GGRS87 = 2100,
        WGS84 = 4326
    }
}