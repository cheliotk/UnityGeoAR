using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public interface ISceneController
    {
        RouteVisualizationType routeVisualizationType { get; set; }
        Vector2 GetLocationAtSceneLoad();
        Vector2 GetPositionGGRS87AtSceneLoad();
        float GetElevationAtSceneLoad();
        float GetCompassHeadingAtSceneLoad();
    }
}