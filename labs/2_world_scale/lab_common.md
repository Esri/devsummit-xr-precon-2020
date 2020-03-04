## World-scale lab

> **Summary**: In this lab, you will build an app to show historical flight data for Palm Springs in world-scale AR.

**Contents**

1. [Background](#background)
2. [Create the app](#create-the-app)
3. [Display the scene](#display-the-scene)
4. [Configure the view](#configure-the-view)
5. [Add heading calibration](#add-heading-calibration)

**Requirements**

To be successful with this lab, you'll need:

* A recent iOS device or a recent Android device on [Google's list of supported devices](https://developers.google.com/ar/discover/supported-devices).
* A development environment that you're comfortable working in.

## Background

There are three common patterns for AR:

* **Flyover** - the camera feed isn't shown; this is an immersive experience similar to traditional virtual reality (VR).
* **Tabletop** - scene content is pinned to a real-world surface. This simulates placing a 3D printed physical model of the scene on a surface.
* **World-scale** - scene content is rendered and displayed on top of the camera feed. The virtual camera's position is at the same position as the physical camera, so scene content appears as it would if it were physically visible.

In this tutorial, you will build an app that let's you see data for nearby flights in AR. To ensure a consistent experience, this lab will use pre-cooked historical flight data, but it could be easily adapted to show real-time flights.

## Create the app

> **Note**: You can skip this step if using one of the starter projects. This section goes through all the steps needed to set up a working AR app from scratch, like requesting camera & location permissions. This section is the same as *Create the app* from the first lab.

1. Create a new app in Xcode/Android Studio/Visual Studio
2. Install the toolkit
3. Add an AR scene view to the layout
4. Set up lifecycle methods
5. Configure privacy & manifest declarations

## Configure AR tracking for world-scale AR

In world-scale AR, the positions of the in-scene camera and the device's camera should match. There are two strategies for updating the position of the origin camera, each with pros and cons:

* **ARKit/ARCore tracking only**
   * Pro: smooth experience in small (~30ft) areas
   * Pro: easier to maintain calibration, especially on devices with noisy/inconsistent GPS
   * Pro: good when very precise positioning is critical (e.g. viewing hidden infrastructure)
   * Con: position error increases as you move from the origin
* **GPS+ARKit/ARCore tracking**
   * Pro: Works over large areas (e.g. when navigating through a campus)
   * Con: Position can jump suddenly as new positions arrive from the location data source every few seconds

> ℹ️ **NOTE**: you can combine these two strategies as needed in your app. Be creative in designing a location tracking and calibration strategy that works for your app's users.

For this lab, the initial position will be taken from the system location, and all subsequent updates will come from ARKit/ARCore.

To enable initial tracking mode, assign a location data source, then update the call to `StartTrackingAsync` to specify `Initial`:

```cs
// code here
```

## Configure the scene

With most 3D scenes, there's a globe, with a basemap, terrain, and atmosphere, set in a starry sky. In world-scale AR, the camera feed provides imagery of the natural Earth, atmosphere, and sky.

### 1 - Disable space & atmosphere effects

The first step to configuring the scene for world-scale AR is to disable the space effect. Runtime supports two space effects:

* Stars
* None/Transparent

```
// code here
```

When you run the app, you should see a flat basemap rendered over the camera feed.

### 2 - Configure the base surface

The next step is to add an elevation source to the scene. This will ensure content is placed accurately relative to the ground.

```
// code here
```

> ℹ️ This lab uses the ArcGIS world elevation service, but you can use any elevation source, including premium sources your organization may have.

By default, Runtime limits the scene camera to be above the scene surface at all times. This conflicts with AR camera positioning when a user goes underground or is very near the ground. For a smooth experience, disable the navigation constraint.

```
// code here
```

### 3 - Set basemap opacity

Because the camera feed will show surroundings, including the ground, there is no need to show the scene's base surface. Set the scene's surface opacity to `0` to hide it completely.

```
```

> ℹ️ **Why have a basemap at all?** The basemap can be shown at partial opacity later, which can be useful for calibration.

Now, when you run the app, you'll see an empty scene.

## Add flight data

1. Create graphics overlay
2. Load and create model symbol
3. Set graphics overlay renderer
4. Read the JSON and create graphics. Add graphics to overlay

## Enable calibration

1. Add calibration view to the app
2. Toggle basemap opacity when hiding/showing calibration UI
3. Confirm that the 