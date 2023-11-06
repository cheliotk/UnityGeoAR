using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface ISceneController
    {
        ElevationAPI routeVisualizationType { get; set; }
        Vector2 GetLocationSourceCRSAtSceneLoad();
        Vector2 GetLocationDestinationCRSAtSceneLoad();
        float GetElevationAtSceneLoad();
        float GetCompassHeadingAtSceneLoad();
    }
}