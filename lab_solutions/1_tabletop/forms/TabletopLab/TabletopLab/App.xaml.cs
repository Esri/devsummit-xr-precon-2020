using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TabletopLab
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Initialize the ArcGIS Runtime before any components are created.
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.Initialize();

            // The root page of your application
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
