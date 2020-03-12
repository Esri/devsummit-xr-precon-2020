## World-scale lab

> **Summary**: In this lab, you will build an app to perform data collection in AR.

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


## Create the UI

Add the following to the AR page xaml:

```xml
<Grid>
   <Grid.RowDefinitions>
      <RowDefinition Height="auto" />
      <RowDefinition Height="*" />
      <RowDefinition Height="auto" />
      <RowDefinition Height="auto" />
   </Grid.RowDefinitions>
   <esriARToolkit:ARSceneView
      x:Name="MyARSceneView"
      Grid.Row="0"
      Grid.RowSpan="3"
      RenderPlanes="True" />
   <Label
      x:Name="HelpLabel"
      Grid.Row="0"
      BackgroundColor="#AA000000"
      HorizontalOptions="FillAndExpand"
      Text="Calibrate your device before starting"
      TextColor="White" />
   <Grid
      x:Name="CalibrationGrid"
      Grid.Row="2"
      BackgroundColor="Black"
      IsVisible="False">
      <Grid Margin="10">
            <Grid.RowDefinitions>
               <RowDefinition Height="auto" />
               <RowDefinition Height="auto" />
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
               <ColumnDefinition Width="auto" />
               <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>
            <Label
               Grid.Row="0"
               Grid.Column="0"
               Text="Elevation:"
               TextColor="White"
               VerticalTextAlignment="Center" />
            <resources:JoystickSlider
               x:Name="ElevationSlider"
               Grid.Row="0"
               Grid.Column="1"
               Margin="5"
               DeltaProgressChanged="AltitudeSlider_DeltaProgressChanged"
               Maximum="10"
               Minimum="-10" />
            <Label
               Grid.Row="1"
               Grid.Column="0"
               Text="Heading:"
               TextColor="White"
               VerticalTextAlignment="Center" />
            <resources:JoystickSlider
               x:Name="HeadingSlider"
               Grid.Row="1"
               Grid.Column="1"
               Margin="5"
               DeltaProgressChanged="HeadingSlider_DeltaProgressChanged"
               Maximum="10"
               Minimum="-10" />
      </Grid>
   </Grid>
   <Grid Grid.Row="3">
      <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
      </Grid.ColumnDefinitions>
      <Button
            x:Name="CalibrateButton"
            Grid.Column="0"
            Clicked="CalibrateButtonPressed"
            Text="Calibrate"
            TextColor="Red" />
      <Button
            x:Name="RoamingButton"
            Grid.Column="1"
            Clicked="RealScaleValueChanged"
            IsEnabled="False"
            Text="GPS" />
      <Button
            x:Name="LocalButton"
            Grid.Column="2"
            Clicked="RealScaleValueChanged"
            Text="Local" />
      <Button
            x:Name="AddButton"
            Grid.Column="3"
            BackgroundColor="Green"
            Clicked="AddButtonPressed"
            Text="+" />
   </Grid>
</Grid>
```

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

There are some concerns with location tracking in AR that are addressed by a custom `LocationDataSource`. Copy the following classes into your app:

Android:

```cs
#if __ANDROID__
using System;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Android.Locations;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Location = Esri.ArcGISRuntime.Location.Location;

namespace ArcGISRuntimeXamarin.Converters
{
    /// <summary>
    /// Custom location data source that allows you to apply an altitude offset in addition to
    /// returning altitude values relative to mean sea level, rather than the WGS84 ellipsoid.
    /// </summary>
    public class ARLocationDataSource : LocationDataSource
    {
        public enum AltitudeAdjustmentMode
        {
            GpsRawEllipsoid,
            NmeaParsedMsl
        }
        private AltitudeAdjustmentMode _currentMode = AltitudeAdjustmentMode.GpsRawEllipsoid;

        // Enable configuration of the altitude mode, adding or removing NMEA listener as needed.
        public AltitudeAdjustmentMode AltitudeMode
        {
            get => _currentMode;
            set
            {
                _currentMode = value;

                if (_currentMode == AltitudeAdjustmentMode.NmeaParsedMsl)
                {
                    GetLocationManager().AddNmeaListener(_listener);
                }
                else
                {
                    GetLocationManager().RemoveNmeaListener(_listener);
                }
            }
        }

        // Object to handle NMEA messages from the onboard GNSS device.
        private readonly NmeaListener _listener = new NmeaListener();

        // Allow setting an altitude offset.
        private double _altitudeOffset;
        public double AltitudeOffset
        {
            get => _altitudeOffset;
            set
            {
                _altitudeOffset = value;

                // Raise a location changed event if possible.
                if (_lastLocation != null)
                {
                    BaseSource_LocationChanged(_baseSource, _lastLocation);
                }
            }
        }

        // Track the last location so that a location changed
        // event can be raised when the altitude offset is changed.
        private Location _lastLocation;

        public IntPtr Handle => throw new NotImplementedException();

        // Track the last elevation received from the GNSS.
        private double _lastNmeaElevation;

        // Use the underlying system location data source.
        private readonly SystemLocationDataSource _baseSource;

        private readonly Context _context;

        public ARLocationDataSource(Context context)
        {
            _context = context;

            // Create and listen for updates from a new system location data source.
            _baseSource = new SystemLocationDataSource();
            _baseSource.HeadingChanged += BaseSource_HeadingChanged;
            _baseSource.LocationChanged += BaseSource_LocationChanged;

            // Listen for altitude change events from the onboard GNSS.
            _listener.NmeaAltitudeChanged += (o, e) =>
            {
                _lastNmeaElevation = e.Altitude;
            };
        }

        private void BaseSource_LocationChanged(object sender, Location e)
        {
            // Store the last location to enable raising change events.
            _lastLocation = e;

            // Intercept location change events from the base source and either
            // apply an altitude offset, or return the offset altitude from the latest NMEA message.
            MapPoint newPosition = null;
            switch (AltitudeMode)
            {
                case AltitudeAdjustmentMode.GpsRawEllipsoid:
                    newPosition = new MapPoint(e.Position.X, e.Position.Y, e.Position.Z + AltitudeOffset, e.Position.SpatialReference);
                    break;
                case AltitudeAdjustmentMode.NmeaParsedMsl:
                    newPosition = new MapPoint(e.Position.X, e.Position.Y, _lastNmeaElevation + AltitudeOffset, e.Position.SpatialReference);
                    break;
            }

            Location newLocation = new Location(newPosition, e.HorizontalAccuracy, e.Velocity, e.Course, e.IsLastKnown);

            UpdateLocation(newLocation);
        }

        private void BaseSource_HeadingChanged(object sender, double e)
        {
            UpdateHeading(e);
        }

        protected override Task OnStartAsync() => _baseSource.StartAsync();

        protected override Task OnStopAsync() => _baseSource.StopAsync();

        private LocationManager _locationManager;

        private LocationManager GetLocationManager()
        {
            if (_locationManager == null)
            {
                _locationManager = (LocationManager)_context.GetSystemService("location");
            }
            return _locationManager;
        }

        private class NmeaListener : Java.Lang.Object, IOnNmeaMessageListener
        {
            private long _lastTimestamp;
            private double _lastElevation;

            public event EventHandler<AltitudeEventArgs> NmeaAltitudeChanged;

            public void OnNmeaMessage(string message, long timestamp)
            {
                if (message.StartsWith("$GPGGA") || message.StartsWith("$GNGNS") || message.StartsWith("$GNGGA"))
                {
                    var parts = message.Split(',');

                    if (parts.Length < 10)
                    {
                        return; // not enough
                    }

                    string mslAltitude = parts[9];

                    if (string.IsNullOrEmpty(mslAltitude)) { return; }


                    if (double.TryParse(mslAltitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double altitudeParsed))
                    {
                        if (timestamp > _lastTimestamp)
                        {
                            _lastElevation = altitudeParsed;
                            _lastTimestamp = timestamp;
                            NmeaAltitudeChanged?.Invoke(this, new AltitudeEventArgs { Altitude = _lastElevation });
                        }
                    }
                }
            }

            public class AltitudeEventArgs
            {
                public double Altitude { get; set; }
            }
        }
    }
}
#endif
```

iOS:

```cs
#if __IOS__
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;


namespace ArcGISRuntimeXamarin.Converters
{
    /// <summary>
    /// Wraps the built-in location data source to enable altitude adjustment.
    /// </summary>
    public class ARLocationDataSource : LocationDataSource
    {
        // Track the altitude offset and raise location changed event when it is updated.
        private double _altitudeOffset = 0;
        public double AltitudeOffset
        {
            get => _altitudeOffset;
            set
            {
                _altitudeOffset = value;

                if (_lastLocation != null)
                {
                    _baseSource_LocationChanged(_baseSource, _lastLocation);
                }
            }
        }

        // Track the last location provided by the system.
        private Location _lastLocation;

        // The system's location data source.
        private SystemLocationDataSource _baseSource;

        public ARLocationDataSource()
        {
            _baseSource = new SystemLocationDataSource();
            _baseSource.HeadingChanged += _baseSource_HeadingChanged;
            _baseSource.LocationChanged += _baseSource_LocationChanged;
        }

        private void _baseSource_LocationChanged(object sender, Location e)
        {
            // Store the last location; used to raise location changed event when only the offset is changed.
            _lastLocation = e;

            // Create the offset map point.
            MapPoint newPosition = new MapPoint(e.Position.X, e.Position.Y, e.Position.Z + AltitudeOffset, e.Position.SpatialReference);

            // Create a new location from the map point.
            Location newLocation = new Location(newPosition, e.HorizontalAccuracy, e.Velocity, e.Course, e.IsLastKnown);

            // Call the base UpdateLocation implementation.
            UpdateLocation(newLocation);
        }

        private void _baseSource_HeadingChanged(object sender, double e)
        {
            UpdateHeading(e);
        }

        protected override Task OnStartAsync() => _baseSource.StartAsync();

        protected override Task OnStopAsync() => _baseSource.StopAsync();
    }
}
#endif
```

To enable continuous tracking mode, assign a location data source, then update the call to `StartTrackingAsync` to specify `Continuous`:

```cs

public CollectDataAR()
{
   InitializeComponent();
   Initialize();
}

// Create this method
private void Initialize()
{
   // Create the custom location data source and configure the AR scene view to use it.
#if XAMARIN_ANDROID
   _locationDataSource = new ARLocationDataSource(Android.App.Application.Context);
   _locationDataSource.AltitudeMode = ARLocationDataSource.AltitudeAdjustmentMode.NmeaParsedMsl;
   MainActivity.Instance.AskForLocationPermission(_locationDataSource);
#elif __IOS__
   _locationDataSource = new ARLocationDataSource();
#endif
   MyARSceneView.LocationDataSource = _locationDataSource;
}

override OnAppearing(){
   base.OnAppearing();
   // Start device tracking.
   try
   {
         await MyARSceneView.StartTrackingAsync(ARLocationTrackingMode.Continuous);
   }
   catch (Exception ex)
   {
         System.Diagnostics.Debug.WriteLine(ex.Message);
   }
}
```

The above code needs some additional support to work on Android. In the Android platform project, update **MainActivity.cs** to something like the following:

```cs
internal static MainActivity Instance { get; private set; }

protected override void OnCreate(Bundle bundle)
{
   base.OnCreate(bundle);

   Instance = this;

   Xamarin.Forms.Forms.Init(this, bundle);
   LoadApplication(new App());
}

#region LocationDisplay

private const int LocationPermissionRequestCode = 99;
private LocationDataSource _lastlocationSource;

public async void AskForLocationPermission(LocationDataSource source)
{
   // Save the mapview for later.
   _lastlocationSource = source;

   // Only check if permission hasn't been granted yet.
   if (ContextCompat.CheckSelfPermission(this, LocationService) != Permission.Granted)
   {
         // Show the standard permission dialog.
         // Once the user has accepted or denied, OnRequestPermissionsResult is called with the result.
         RequestPermissions(new[] {Manifest.Permission.AccessFineLocation}, LocationPermissionRequestCode);
   }
   else
   {
         try
         {
            // Explicit DataSource.LoadAsync call is used to surface any errors that may arise.
            await _lastlocationSource.StartAsync();
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine(ex);
            ShowMessage(ex.Message, "Failed to start location display.");
         }
   }
}

public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
{
   // Ignore other location requests.
   if (requestCode != LocationPermissionRequestCode)
   {
         return;
   }

   // If the permissions were granted, enable location.
   if (grantResults.Length == 1 && grantResults[0] == Permission.Granted && _lastUsedMapView != null)
   {
         System.Diagnostics.Debug.WriteLine("User affirmatively gave permission to use location. Enabling location.");
         try
         {
            // Explicit DataSource.LoadAsync call is used to surface any errors that may arise.
            await _lastlocationSource.StartAsync();
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine(ex);
            ShowMessage(ex.Message, "Failed to start location display.");
         }
   }
   else
   {
         ShowMessage("Location permissions not granted.", "Failed to start location display.");
   }

   // Reset the mapview.
   _lastlocationSource = null;
}

private void ShowMessage(string message, string title = "Error") => new AlertDialog.Builder(this).SetTitle(title).SetMessage(message).Show();
#endregion LocationDisplay
```

## Configure the scene

The following code in `Initialize` will configure the scene:

```cs
// Create the scene and show it.
_scene = new Scene(Basemap.CreateImagery());
MyARSceneView.Scene = _scene;

// Create and add the elevation surface.
_elevationSource = new ArcGISTiledElevationSource(new Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer"));
_elevationSurface = new Surface();
_elevationSurface.ElevationSources.Add(_elevationSource);
MyARSceneView.Scene.BaseSurface = _elevationSurface;

// Hide the surface in AR.
_elevationSurface.NavigationConstraint = NavigationConstraint.None;
_elevationSurface.Opacity = 0;

// Configure the space and atmosphere effects for AR.
MyARSceneView.SpaceEffect = SpaceEffect.None;
MyARSceneView.AtmosphereEffect = AtmosphereEffect.None;

// Add a graphics overlay for displaying points in AR.
_graphicsOverlay = new GraphicsOverlay();
_graphicsOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Absolute;
_graphicsOverlay.Renderer = new SimpleRenderer(_tappedPointSymbol);
MyARSceneView.GraphicsOverlays.Add(_graphicsOverlay);

// Add the existing features to the scene.
FeatureLayer treeLayer = new FeatureLayer(_featureTable);
treeLayer.SceneProperties.SurfacePlacement = SurfacePlacement.Absolute;
MyARSceneView.Scene.OperationalLayers.Add(treeLayer);

// Add the event for the user tapping the screen.
MyARSceneView.GeoViewTapped += ARViewTapped;

// Disable scene interaction.
MyARSceneView.InteractionOptions = new SceneViewInteractionOptions() { IsEnabled = false };
```

Add the following supporting fields:

```cs
// Scene content.
private ArcGISTiledElevationSource _elevationSource;
private Surface _elevationSurface;
private Scene _scene;

// Track when user is changing between AR and GPS localization.
private bool _changingScale;

// Feature table for collected data about trees.
private ServiceFeatureTable _featureTable = new ServiceFeatureTable(new Uri("https://services2.arcgis.com/ZQgQTuoyBrtmoGdP/arcgis/rest/services/AR_Tree_Survey/FeatureServer/0"));

// Graphics for tapped points in the scene.
private GraphicsOverlay _graphicsOverlay;
private SimpleMarkerSceneSymbol _tappedPointSymbol = new SimpleMarkerSceneSymbol(SimpleMarkerSceneSymbolStyle.Diamond, System.Drawing.Color.Orange, 0.5, 0.5, 0.5, SceneSymbolAnchorPosition.Center);

// Custom location data source that enables calibration and returns values relative to mean sea level rather than the WGS84 ellipsoid.
private ARLocationDataSource _locationDataSource;

// Calibration state fields.
private bool _isCalibrating;
private double _altitudeOffset;
```

## Enable calibration

Add the following property to support calibration:

```cs
private bool IsCalibrating
{
   get
   {
         return _isCalibrating;
   }
   set
   {
         _isCalibrating = value;
         if (_isCalibrating)
         {
            // Show the surface semitransparent for calibration.
            _scene.BaseSurface.Opacity = 0.5;

            // Enable scene interaction.
            MyARSceneView.InteractionOptions.IsEnabled = true;
            CalibrationGrid.IsVisible = true;
         }
         else
         {
            // Hide the scene when not calibrating.
            _scene.BaseSurface.Opacity = 0;

            // Disable scene interaction.
            MyARSceneView.InteractionOptions.IsEnabled = false;
            CalibrationGrid.IsVisible = false;
         }
   }
}
```

Add the following calibration support code:

```cs
private void CalibrateButtonPressed(object sender, EventArgs e) { IsCalibrating = !IsCalibrating; }

private void AltitudeSlider_DeltaProgressChanged(object sender, DeltaChangedEventArgs e)
{
   // Add the new value to the existing altitude offset.
   _altitudeOffset += e.DeltaProgress;

   // Update the altitude offset on the custom location data source.
   _locationDataSource.AltitudeOffset = _altitudeOffset;
}

private void HeadingSlider_DeltaProgressChanged(object sender, DeltaChangedEventArgs e)
{
   // Get the old camera.
   Camera camera = MyARSceneView.OriginCamera;

   // Calculate the new heading by applying the offset to the old camera's heading.
   double heading = camera.Heading + e.DeltaProgress;

   // Create a new camera by rotating the old camera to the new heading.
   Camera newCamera = camera.RotateTo(heading, camera.Pitch, camera.Roll);

   // Use the new camera as the origin camera.
   MyARSceneView.OriginCamera = newCamera;
}

private async void RealScaleValueChanged(object sender, EventArgs e)
{
   // Prevent this from being called concurrently
   if (_changingScale)
   {
         return;
   }
   _changingScale = true;

   // Disable the associated UI controls while switching.
   RoamingButton.IsEnabled = false;
   LocalButton.IsEnabled = false;

   // Check if using roaming for AR location mode.
   if (((Button)sender).Text == "GPS")
   {
         await MyARSceneView.StopTrackingAsync();

         // Start AR tracking using a continuous GPS signal.
         await MyARSceneView.StartTrackingAsync(ARLocationTrackingMode.Continuous);
         ElevationSlider.IsEnabled = true;
         LocalButton.IsEnabled = true;
   }
   else
   {
         await MyARSceneView.StopTrackingAsync();

         // Start AR tracking without using a GPS signal.
         await MyARSceneView.StartTrackingAsync(ARLocationTrackingMode.Ignore);
         ElevationSlider.IsEnabled = false;
         RoamingButton.IsEnabled = true;
   }
   _changingScale = false;
}
```

## Enable data collection:

```cs
private async void AddButtonPressed(object sender, System.EventArgs e)
{
   // Check if the user has already tapped a point.
   if (!_graphicsOverlay.Graphics.Any())
   {
         await Application.Current.MainPage.DisplayAlert("Error", "Didn't find anything, try again.", "OK");
         return;
   }

   try
   {
         // Prevent the user from changing the tapped feature.
         MyARSceneView.GeoViewTapped -= ARViewTapped;

         // Prompt the user for the health value of the tree.
         int healthValue = await GetTreeHealthValue();

         // Create a new ArcGIS feature and add it to the feature service.
         await CreateFeature(healthValue);
   }
   // This exception is thrown when the user cancels out of the prompt.
   catch (TaskCanceledException)
   {
         return;
   }
   finally
   {
         // Restore the event listener for adding new features.
         MyARSceneView.GeoViewTapped += ARViewTapped;
   }
}

private async Task<int> GetTreeHealthValue()
{
   string health = await DisplayActionSheet("Tree health?", "Cancel", null, "Dead", "Distressed", "Healthy");

   // Return a tree health value based on the users selection.
   switch (health)
   {
         case "Dead": // Dead tree.
            return 0;

         case "Distressed": // Distressed tree.
            return 5;

         case "Healthy": // Healthy tree.
            return 10;

         default:
            return 0;
   }
}

private async Task CreateFeature(int healthValue)
{
   HelpLabel.Text = "Adding feature...";

   try
   {
         // Get the geometry of the feature.
         MapPoint featurePoint = _graphicsOverlay.Graphics.First().Geometry as MapPoint;

         // Create attributes for the feature using the user selected health value.
         IEnumerable<KeyValuePair<string, object>> featureAttributes = new Dictionary<string, object>() { { "Health", (short)healthValue }, { "Height", 3.2 }, { "Diameter", 1.2 } };

         // Ensure that the feature table is loaded.
         if (_featureTable.LoadStatus != Esri.ArcGISRuntime.LoadStatus.Loaded)
         {
            await _featureTable.LoadAsync();
         }

         // Create the new feature
         ArcGISFeature newFeature = _featureTable.CreateFeature(featureAttributes, featurePoint) as ArcGISFeature;

         // Add the newly created feature to the feature table.
         await _featureTable.AddFeatureAsync(newFeature);

         // Apply the edits to the service feature table.
         await _featureTable.ApplyEditsAsync();

         // Reset the user interface.
         HelpLabel.Text = "Tap to create a feature";
         _graphicsOverlay.Graphics.Clear();
         AddButton.IsEnabled = false;
   }
   catch (Exception ex)
   {
         Console.WriteLine(ex.Message);
         await Application.Current.MainPage.DisplayAlert("Error", "Could not create feature", "OK");
   }
}
```
