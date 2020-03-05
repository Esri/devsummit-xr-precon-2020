## Tabletop lab

> **Summary**: In this lab, you will build an app to show a 3D city on a table. Using your phone, you'll be able to move around to see the scene from different angles.

**Contents**

1. [Background](#background)
2. [Create the app](#create-the-app)
3. [Display the scene](#display-the-scene)
4. [Set clipping distance](#set-clipping-distance)
5. [Set translation factor](#set-translation-factor)
6. [Set origin camera](#set-origin-camera)

**Requirements**

To be successful with this lab, you'll need:

* A recent iOS device or a recent Android device on [Google's list of supported devices](https://developers.google.com/ar/discover/supported-devices).
    * If deploying to iOS, a Mac set up for Xamarin development
* Visual Studio 2019, for Mac or Windows

## Background

There are three common patterns for AR:

* **Flyover** - the camera feed isn't shown; this is an immersive experience similar to traditional virtual reality (VR).
* **Tabletop** - scene content is pinned to a real-world surface. This simulates placing a 3D printed physical model of the scene on a surface.
* **World-scale** - scene content is rendered and displayed on top of the camera feed. The virtual camera's position is at the same position as the physical camera, so scene content appears as it would if it were physically visible.

In this tutorial, you will build an app that let's you tap a real-world surface (such as a desk or table) to virtually place a scene on that surface. This is an example of the tabletop AR pattern.

## Create the app

> **Note**: You can skip this step if using one of the starter projects. This section goes through all the steps needed to set up a working AR app from scratch, like requesting camera & location permissions.

### 1 - Create a new project

First, create a new Xamarin.Forms project with Visual Studio. On VS for Mac, choose the 'Blank Forms App' template.

When you're done, you should see something like the following:

![](./images/xamarin_base_project.png)

### 2 - Install the toolkit

### 3 - Add the AR view to the layout

### 4 - Set up lifecycle methods

### 5 - Configure privacy and manifest declarations

## Display the scene

While any scene can be used in tabletop AR, this lab will use the [San Diego Convention Center web scene](https://www.arcgis.com/home/item.html?id=6bf6d9f17bdd4d33837e25e1cae4e9c9).

## Set origin camera

In tabletop AR, the origin camera is the point, in scene coordinates, where the scene is anchored to the physical surface. In most cases, a good origin camera is at the x,y position of the focal point for the scene. To ensure all content is visible and the scene appears to be on top of the table, the vertical position of the origin camera should be at the lowest point in the scene.

For the [San Diego Convention Center scene](https://www.arcgis.com/home/item.html?id=6bf6d9f17bdd4d33837e25e1cae4e9c9), a good origin camera centers on the Manchester Grand Hyatt in San Diego.

> lat: 32.71012, lng: -117.168654, alt: 0

```cs
// code here
```

## Set translation factor

The translation factor determines how far the virtual scene camera moves when the physical camera moves. This value should be set such you can move the phone completely around the scene.

A good formula for finding an appropriate translation factor is: **(virtual content width)/(desired real-world physical width of the scene)**. For example, if you want to show a 1000 meter wide scene on a 1 foot (0.3 meter) table, you would use **1000 / 0.3**, or 3333.33.

For this scene, a translation factor of 700 is appropriate.

```cs
// code here
```

## Set clipping distance

To ensure that the scene looks good on a table, it needs to be limited in extent. Setting a clipping distance will limit what is rendered to a radius around the origin camera.

> **NOTE**: While clipping distance is a great option for limiting the extent of a tabletop scene, you can also specially author data for more precise control. For example, you can choose a rectangular section of a scene and package that with your app.

A clipping distance of 180 will show a circle with a radius of 180, which is appropriate for the translation factor set above.

```cs
// code here
```

## Result

When you run the app, it should look like the following:

![image of app]()