using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public interface ISceneController
    {
        RouteVisualizationType routeVisualizationType { get; set; }
        Vector2 GetLocationAtSceneLoad();
        float GetElevationAtSceneLoad();
    }
}