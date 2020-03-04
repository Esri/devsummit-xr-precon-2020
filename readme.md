# Developer Summit 2020 - XR Pre-Summit Hands-On Training Resources

This repo contains:

* [Starter projects, preconfigured with toolkit](starter_projects/)
* Labs
* Lab exercises, before & after
* Demos
* Links to useful resources

## Instructions

If you'd like to suggest changes to this repo, create a Fork and open a PR. Otherwise, you can clone the repo directly.

## FAQ

> 😤 **Stuck? Start here**

<details><summary>Visual Studio & Xamarin</summary><p>


* ❓ My solution won't build
    * 💡 Ensure that you've restored Nuget packages. Right click on your solution and select **Restore Nuget Packages**.
* ❓ I'm getting Android resource errors with the starter solutions.
    * 💡 Try deploying the project to device. This will trigger a rebuild of the Android Resources.
* ❓ I'm getting `File not found` or `maxpath` errors when trying to build.
    * 💡 Windows limits the lengths of paths, and this can cause problems with Xamarin projects. Either move the solution closer to the root of your drive, or create a _directory junction_ and build from there.
      > ```sh
      > mklink /J "C:\dev" "C:\some\long\path\starter_projects\..."
      > ```
* ❓ I can't restore the Nuget packages or I'm seeing hundreds of build errors that I can't resolve.
    * 💡 In the Visual Studio toolbar, navigate to **Tools->Nuget Package Manager->Package Manager Settings**, then select **Package Sources**. Verify that **nuget.org** is enabled and its source is set to `https://api.nuget.org/v3/index.json`. Disable all other sources; a single failing source will prevent any packages from restoring.
* ❓ I'm having trouble deploying to my iOS device through my mac.
    * 💡 This one can be tricky. Don't spend too much time on this before asking a session leader for help.

</p></details>

## Requirements

> TODO

* 

## Resources

### Local data for use with AR

Finding good data is one of the hardest parts of building a world-scale AR app. We've collected some data that you may find useful while working with AR at DevSummit:

* [AR DevSummit group on ArcGIS Online]()
    * [PSCC MMPK with locator]() - this locator can be used for indoor routing at the convention center
    * [PSCC feature layer]() - layer can be used for visualizing conference center features

### Platform-specific resources

<details><summary>iOS (swift)</summary><p>

* Samples
* Doc
* Articles

</p></details>

<details><summary>Android (Kotlin)</summary><p>

* Samples
* Doc

</p></details>

<details><summary>.NET/Xamarin (C#)</summary><p>

* Samples
* Doc

</p></details>

<details><summary>Qt (C++)</summary><p>

* Samples
* Doc

</p></details>

## Issues

Find a bug or want to request a change? Please let us know by submitting an issue.

## Licensing

Copyright 2020 Esri

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

A copy of the license is available in the repository's [license.txt](license.txt) file.
