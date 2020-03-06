using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TabletopLab
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void GeoViewTapped(object sender, Esri.ArcGISRuntime.Xamarin.Forms.GeoViewInputEventArgs e)
        {
            if (MySceneView.SetInitialTransformation(e.Position))
            {
                InitializeSceneAsync();
            }
            else
            {
                DisplayAlert("No plane found", "Try moving the phone around until you see white dots", "Ok");
            }
        }

        public async Task InitializeSceneAsync()
        {
            try
            {
                // Create the scene
                Scene sanDiegoScene = new Scene(new Uri("https://arcgisruntime.maps.arcgis.com/home/item.html?id=90a10b734aef4f30b3014fb515764296"));

                // Explicitly load the scene (in case there are errors, this will throw an exception)
                await sanDiegoScene.LoadAsync();

                // Disable the navigation constraint
                sanDiegoScene.BaseSurface.NavigationConstraint = NavigationConstraint.None;

                // Show the scene in the view
                MySceneView.Scene = sanDiegoScene;

                MapPoint originPoint = new MapPoint(-117.168654, 32.71012, SpatialReferences.Wgs84);

                await sanDiegoScene.BaseSurface.LoadAsync();
                double elevation = await sanDiegoScene.BaseSurface.GetElevationAsync(originPoint);

                // Pin the scene to the table at 32.71012,-117.168654, 2 meters above sea level
                MySceneView.OriginCamera = new Camera(32.71012, -117.168654, elevation, 0, 90, 0);
            }
            catch (Exception ex)
            {
                _ = DisplayAlert("Failed to init scene", ex.Message, "Ok");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await MySceneView.StartTrackingAsync();
            }
            catch (System.Exception ex)
            {
                DisplayAlert("Error starting AR", ex.Message, "Ok");
            }
        }
    }
}
