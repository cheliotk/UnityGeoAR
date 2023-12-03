# UnityGeoAR
UnityGeoAR is a tool for Unity3D that streamlines the development of geolocated Augmented Reality (AR) applications. It provides services for converting geo-coordinates and elevations to Unity Scene units and for aligning camera rotations between a virtual and real-world view.

# Installation

Fork/Copy/Download this repository and open in the Unity Editor.

# Usage

## Setting up location updates

1. Make sure your app has location services permissions accepted.

2. Have a Monobehaviour that initializes an instance of `LocationUpdatesService`, or alternatively use the provided `LocationUpdater`.

3. During runtime, ensure that location updates are running before opening an AR/geolocation enabled scene. We recommend keeping the location updates running in the background until the end of the app runtime.

## Setting up AR-enabled scenes

1. For AR-enabled scenes, first setup ARFoundation as per [its instructions](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/manual/index.html).

2. Make sure you have a behaviour in the scene that implements `SceneControllerBase`, or use the provided `ARSceneController`.

3. At minimum, set the `sourceCRS` and `destinationCRS` fields of `SceneControllerBase` using the EPSG code for the required CRS. [List of EPSG codes](https://epsg.io/).

4. When the geolocated AR scene is first loaded, make sure to set the `locationSourceCRSAtSceneLoad`, `locationDestinationCRSAtSceneLoad`, and `compassHeadingAtSceneLoad` fields of `SceneControllerBase`. Optionally, also set the elevation at start, if using elevations.

5. If using your own implementation of `SceneControllerBase`, also make sure to offset the Y rotation of the AR Session Origin GameObject (that is the parent of the ARCamera) by `compassHeadingAtSceneLoad` degrees.

6. Now you can use the methods provided in `SceneControllerBase.WorldToUnityService` to reproject objects geolocated near the user's location, and place them in the scene around the camera.

# Example

You can always have a look at the included `ARNavigation` examples (Found in `Assets > Scenes > AR-Navigation`) to see usages of the `UnityGeoAR` tools. `ARNavigation` draws a path that users can follow in AR to navigate between two points.
- `AR-Navigation_TestScene` implements a geocoder and routing service to set a destination (requires an API key from [OpenRouteService](https://openrouteservice.org/), you can store API keys in a `APIKeyContainer` scriptable object: `Create > ScriptableObjects > API Keys Container`)

- `AR-Navigation_PresetRoutes` reads from preset routes available during compile time. Preset routes can be provided as .txt files containing series of waypoints in (y,x) format. Have a look at `PresetRouteSelectorHandler` for a preset routes parser. Sample preset routes are stored in `Assets > Resources`

To test the example `ARNavigation` app, build the app using the following scenes included in the build settings, and run on a mobile device.

![Scene order](/gitFiles/Unity_UngRGQ9JF3.png)
