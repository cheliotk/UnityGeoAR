using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface ISceneController
    {
        ElevationAPI routeVisualizationType { get; set; }
        Vector2 GetLocationAtSceneLoad();
        Vector2 GetPositionGGRS87AtSceneLoad();
        float GetElevationAtSceneLoad();
        float GetCompassHeadingAtSceneLoad();
    }
}