# Android Starter projects

> ℹ **NOTE**: The starter projects are configured with a min SDK version of 29. You can lower this down to 23 depending on what device you are on

## Intalling the SDK and toolkit
The projects here already contain gradle project dependencies to include the SDK and toolkit. You can go here to learn more about installing the [SDK](https://developers.arcgis.com/android/latest/guide/install-and-set-up.htm) and [toolkit](https://github.com/Esri/arcgis-runtime-toolkit-android). Using gradle to install the toolkit is much easier but manually installing it allows you to modify the toolkit code if you chose to.

#### Here is a quick check list if you want to create a project from scratch

    1. Create a new project using Android Studio with a min SDK version of 23 (or whatever your device is using)
    2. In build.gradle : Project - add these lines
        maven {
            url 'https://esri.bintray.com/arcgis'
        }
    3. In build.gradle : Module - add these lines under dependencies
        implementation 'com.esri.arcgisruntime:arcgis-android:100.7.0'
        implementation com.esri.arcgisruntime:arcgis-android-toolkit:100.7.0'
    3. In build.gradle : Module - add these lines in android
        //java version
        compileOptions {
            sourceCompatibility = '1.8'
            targetCompatibility = '1.8'
        }
    4. In Android.manifest add these lines. Note you will need to add more permissions later to access GPS position
        <uses-permission android:name="android.permission.INTERNET" />
        <uses-permission android:name="android.permission.CAMERA" />
    5. Go to activity_main.xml and add the ArcGISArView
        <com.esri.arcgisruntime.toolkit.ar.ArcGISArView
          android:id="@+id/arView"
          android:layout_width="match_parent"
          android:layout_height="match_parent" >
        </com.esri.arcgisruntime.toolkit.ar.ArcGISArView>
    6. Add - private ArcGISArView mArView;
    7. Go to your MainActivity.kt or .java and Override the onResume and onPause

    @Override
    public synchronized void onResume() {
        super.onResume();

        //start tracking when app is being used
        if (mArView != null) {
            mArView.startTracking(ArcGISArView.ARLocationTrackingMode.INITIAL);
        }
    }

    @Override
    public synchronized void onPause() {
        //stop tracking when app is not being used
        if (mArView != null) {
            mArView.stopTracking();
        }
        super.onPause();
    }

    8. Change the onCreate function to look like this:

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        //get the AR view from the activity
        mArView = findViewById(R.id.arView);
        mArView.registerLifecycle(getLifecycle());

        ArcGISScene scene = new ArcGISScene(Basemap.createImagery());
        mArView.getSceneView().setScene(scene);
    }

    9. Build the app, you should see a globe floating in front of you!

#### Once you have the toolkit and SDK installed follow these steps to setup a basic AR app
    1. In XCode go to your Main.storyboard.
    2. Click View->Show Library
    3. Add a new view to your view controller
    4. Change the view to use class ArcGISARView and module ArcGISToolkit
    5. Click Editor->Assistant
    6. Right click the view you added and drag it into ViewController.swift file
    7. Go to Info.plist and add these 2 settings
      Privacy - Location When in Use Usage
      Private - Camera Usage Description
    8. Go to ViewController.swift and add these 2 imports
      import ArcGIS
      import ArcGISToolkit
    9. Add these to functions inside the ViewController Class to start ARKit when the view opens and stop it when it closes
        
        override func viewDidAppear(_ animated: Bool) {
            super.viewDidAppear(animated)
            arView.startTracking(.continuous, completion: nil)
        }

        override func viewDidDisappear(_ animated: Bool) {
            super.viewDidDisappear(animated)
            arView.stopTracking()
        }
    10. Change the viewDidLoad function to look like this
        override func viewDidLoad() {
            super.viewDidLoad()
            arView.sceneView.scene = AGSScene(basemap: AGSBasemap.streets())
        }
    11. Run your app. If you did everything correctly you will see a globe floating in front of you.


# Helpful links

[Android toolkit](https://github.com/Esri/arcgis-runtime-toolkit-android)

[Android guide doc for AR](https://developers.arcgis.com/android/latest/guide/display-scenes-in-augmented-reality.htm)

[Android AR samples](https://developers.arcgis.com/android/latest/java/sample-code/display-scene-in-tabletop-ar/)

[Android API ref](https://developers.arcgis.com/android/latest/api-reference/reference/)