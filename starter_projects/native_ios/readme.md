# iOS Starter projects

 You will need to install the ArcGIS Runtime iOS SDK and the ArcGIS Runtime toolkit for iOS to use these projects.

 Information about installing the iOS SDK can be found [here](https://developers.arcgis.com/ios/latest/swift/guide/install.htm) You can install the SDK manually or with CocoaPods. We recommend using CocoaPods now.

 Information about installing the toolkit for iOS can be found [here](https://github.com/Esri/arcgis-runtime-toolkit-ios). You can install the toolkit manually, with Carthage, or with CocoaPods. Again we recommend using CocoaPods again. Manually installing the toolkit does give you the freedom to modify the toolkit code if you would like to.

## If you want to download projects containing the toolkit and SDK
There are projects available here that already have the toolkit and SDK installed using CocoaPods [here](https://esriis-my.sharepoint.com/:f:/g/personal/matt9678_esri_com/EjhK7bruIGNKtkElO4KkwpAB9yYHdF0sLJ2agsi652ubrg?e=Nm09eE). The projects contain framework files for the SDK and toolkit which are bigger than 100 MB so I couldn't put them on github. Make sure you open the .xcworkspace with XCode


## If you want to use the projects without the Pods installed
A quick check list for installing the toolkit and SDK using CocoaPods with the projects here on github

1. Make sure you have [CocoaPods](https://cocoapods.org/) installed.
  > sudo gem install cocoapods

  > pod --version

2. Use the terminal to navigate where your project is and run:
  > pod install

3. Use XCode to open the .xcworkspace project.

## If you want to create a project from scratch
You will need to install the [SDK](https://developers.arcgis.com/ios/latest/swift/guide/install.htm) and [toolkit](https://github.com/Esri/arcgis-runtime-toolkit-ios).

#### Here is a quick check list if you use Cocoa pods

    1. $ sudo gem install cocoapods \\to install CocoaPods
    2. $ pod --version \\to confirm you installed CocoaPods
    3. Use XCode to create an empty project
    4. Navigate to your project in the terminal
    5. $ pod init \\this creates a default Podfile
    6. Add the following 2 lines to the top of your Podfile
      pod 'ArcGIS-Runtime-SDK-iOS', '100.7' 
      pod 'ArcGIS-Runtime-Toolkit-iOS'
    7. $ pod install \\to install the pods you just added to your Podfile
    8. Close your current XCode project and reopen the .xcworkspace file

## Once you have the toolkit and SDK installed follow these steps to setup a basic AR app
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
    11. Run your app. If you did everything correctly you will see a globe floating in front of you!


# Helpful links

[iOS toolkit](https://github.com/Esri/arcgis-runtime-toolkit-ios)

[iOS guide doc for AR](https://developers.arcgis.com/ios/latest/swift/guide/display-scenes-in-augmented-reality.htm)

[iOS AR samples](https://developers.arcgis.com/ios/latest/swift/sample-code/collect-data-in-ar/)

[iOS API ref](https://developers.arcgis.com/ios/latest/api-reference/)
