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

> **Note**: You can skip this step if using one of the starter projects. This section goes through all the steps needed to set up a working AR app from scratch, like requesting camera & location permissions.

1. Create a new app in Xcode/Android Studio/Visual Studio
2. Install the toolkit
3. Add an AR scene view to the layout
4. Set up lifecycle methods
5. Configure privacy & manifest declarations

## Configure the scene

1. Do basic scene set up
2. Get the models
3. Set up a graphics overlay with renderer

## Add flight data

1. Either read JSON, or add from a feature layer