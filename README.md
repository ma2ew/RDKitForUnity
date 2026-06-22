# RDKitForUnity

This project is a Software Development Kit (SDK) designed to integrate RDKit functionality into Unity, along with a molecular visualization library supporting both Desktop and VR viewing.

## Necessary Applications and Packages

* Unity: The software development engine that this SDK is built on. Unity can be downloaded from https://unity.com/
  * Unity Entities: This package is necessary to handle the Entity-Component-System (ECS) functionality of RDKitForUnity.
  * Unity Physics: This package works with Unity Entities to provide interactions with ECS objects. When installing the package, be sure to import the "Custom Physics Authoring" Sample found in the package description.
* NuGetForUnity: An open-source client that integrates the NuGet package manager into Unity. Source code and installation instructions can be found here: https://github.com/GlitchEnzo/NuGetForUnity
  * RDKit2DotNetStandard: Once NuGetForUnity is installed, open the package manager and search for "RDKit2DotNetStandard". This is the version of the RDKit C# wrapper that is used within the project.

## SDK Folders
* Assets: This houses the RDKitForUnity folder.
  * RDKitForUnity: The folder that contains all scripts related to RDKitForUnity functionality (Scripts folder) and files related to visualization tools (Visual Data folder).
