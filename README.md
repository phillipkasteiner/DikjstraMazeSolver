# DikjstraMazeSolver
    This is a unity project that contains two folders. On is to be built on Desktop and the other can be built for the hololens.

# DikjstraMazeSolver

This is a unity project that contains two folders. On is to be built on Desktop and the other can be built for the hololens.
## Getting Started

Clone the repo to get both folders that contain the projects.

## Requirements
 Before being able to build and develope this project you will need the following:
* Unity 2017.3
* Visual Studio 2017

For Hololens Deployment Only:
* [Hololens Toolkit](https://github.com/Microsoft/MixedRealityToolkit-Unity) 
* Mixed Realities Portal

### Installing

Clone or Download the repo to a location on your drive.

Open a new, empty Unity project.

Place the Assest folder of either project into the source explore and save the project.

Hololens deployment specifics only:
* Insure that the HololensToolKit folders are ported into the Unity project.
* A new tab should appear on the Unity banner called 'Mixed Reality Tool'
* Click and select 'Format Project for Hololens'
* This should Build your project for Hololens devolopment

## Building Your Desktop App

To build on desktop you need to select File > Build Settings.
* From here set the build settings for a Windows Desktop app and sellect build and run.
* You should be presented with the Unity logo and the application.

## Building Your Hololens App
To build on the Hololens you need to select File > Build Settings.
* Select UWP in the Build settings and select to deploy to the Holoens, Lastest Version of the SDK and Visual Studio and Build for Local.

* Select the destination for the build folder.
* Build porject and open the visual studio project file in the folder.
* Run the project without debugging and if it is the first time you are building the project you will be prompted to pair your hololens.
* [Follow These Instruction To Pair The Hololens](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio)

## Built With

* [Unity](https://unity3d.com/) - Game Engine
* [Visual Studio](https://www.visualstudio.com/) - IDE
* [Hololens Tool Kit](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/GettingStarted.md) - Used for Hololens Development

## Authors

* Phillip Kasteiner
* David Escobedo
* Taeton Prettyman
