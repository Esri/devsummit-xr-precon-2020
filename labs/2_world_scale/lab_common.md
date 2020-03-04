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

Have you ever looked up at the sky and wondered where that airplane is going, or where it came from? Curious about how firefighting aircraft respond to wildfires?

Starting in 2020, most aircraft are required to transmit their status for tracking by air traffic control and other aircraft. Services like [Flight Radar 24](flightradar24.com/), [FlightAware](https://flightaware.com/), and [OpenSky Network](https://opensky-network.org/) use crowdsourcing to record these transmissions and show flights on a map.

This lab will show historical flight data collected by the OpenSky Network, using a 3D model to represent each airplane.

> ℹ️ **NOTE**: once you've completed the lab, you can ask the presenters for information on how to sign up for an account and show real-time data.

### 1 - Load and create a model symbol

The first step is to load the model symbol. We've provided a model depicting a Boeing jet which you can use in your scene.

First, download the model from ArcGIS Online.

```cs
// code here
```

Next, create the 3D model symbol:

```cs
// code here
```

### 2 - Show graphics overlay with renderer

You can use a `GraphicsOverlay` to show temporary graphics in the scene. With graphics overlays, you can either define a symbol for each graphic individually, or provide a renderer that draws each graphic based on its attributes. 

For this lab, a renderer that shows the airplane model, with heading set programmatically, is appropriate.

First, create the renderer.

```cs
// code here
```

Next, create the graphics overlay and configure it with the renderer.

```
// code here
```

Finally, show it in the scene.


### 3 - Read data and add graphics

A snapshot of aircraft positions has been provided as a JSON string which you can include in your app. [QuickType.io]() can be used to generate code for reading that JSON into usable objects. You can use that service now, or copy-paste the following:

<details><summary>Code from QuickType</summary><p>

```cs
// code here
```

</p></details>

After reading the JSON, create a graphic for each aircraft, being sure to set the `HEADING` attribute.

```cs
// code here
```

Now, when you run the app, you'll see aircraft in the sky, rendered over the camera feed.

## Enable calibration

Because this lab is showing a snapshot of historical data, location and heading errors are hard to see. If you were showing real-time data, you might notice that the heading is off, causing aircraft to appear in the wrong place. 

Position & heading accuracy requirements will differ for each application. A key part of developing your AR app will be identifying calibration needs and designing an approach that works well for your users.

Because aircraft are typically quite far away, small errors in (x,y) location don't significantly affect visualization, but heading (angle) errors will.

To correct for heading errors indoors, you will build a calibration workflow that involves aligning a virtual in-scene 'north star' with a known reference point in the room.

### 1 - Disable scene view interaction

The first step to creating the calibration workflow is to disable scene view interaction. Touch interactions in the scene view can be problematic because they can conflict with the calibration experience.

```cs
// code here
```

### 2 - Add a slider to the view

When the user is calibrating, show a slider to allow them to adjust their heading.

First, add the slider to the view.

```cs
// code here
```

Next, add a button to hide and show the calibration slider as needed.

```cs
// code here
```

### 3 - Add a North Star

The North star will be used to help align the scene against the real world.

First, create a graphic with a diamond scene symbol.

```cs
// code here - should determine a good north star programmatically
```

Next, add the graphic to a new graphics overlay, and add that to the scene.

```cs
// code here
```

### 4 - Rotate the camera when the user slides

There are a variety of slider behaviors to choose from, but to keep this lab simple, the slider will control the absolute heading of the scene's origin camera.

First, listen for slider value changes.

```cs
// code here
```

When the slider value changes, rotate the scene view's origin camera.


```cs
// code here
```

## Result

When you run the app, you should see something like the following:

![image of completed lab]()