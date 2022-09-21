using DotSpatial.Projections;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Auxiliary
{
    public class ProjectionUtilities
    {
        public static Vector2 ReprojectFrom_WGS84_To_GGRS87(float lat, float lng)
        {
            double[] latLongs = new double[] { lng, lat };
            double[] heights = new double[] { 0 };
            ProjectionInfo source = ProjectionInfo.FromEpsgCode(4326);
            ProjectionInfo destination = ProjectionInfo.FromEpsgCode(2100);
            Reproject.ReprojectPoints(latLongs, heights, source, destination, 0, heights.Length);

            Debug.Log($"{latLongs[0]} | {latLongs[1]}");
            return new Vector2((float)latLongs[0], (float)latLongs[1]);
        }
    }
}