# Assets Folder

This folder contains essentially everything needed for RDKit4Unity modifications.

## RDKit4Unity-Specific Contents

* [NuGet](#nuget)
* [Packages](#packages)
* [RDKitForUnity](#rdkitforunity)
* [Samples](#samples)
* [Scenes](#scenes)
* [Settings](#settings)

### NuGet

This folder contains the NuGetForUnity functionality to access and automatically control the version of NuGet packages. This version of NuGet is for Windows computers, and so will only work on Windows natively, or Linux with Wine. MacOS users will need to reinstall NuGetForUnity.

While used to manage RDKit's installation, it is not necessariy RDKit-specific, and so can be used to download various other C# packages as well.




### Packages

This folder contains the packages installed with NuGetForUnity. It currently contains a Windows version of RDKit2DotNetStandard, but as mentioned before, this will need to be replaced if you do not have a Windows or Linux+Wine setup.




### RDKitForUnity

This folder contains all RDKitForUnity scripts. If you need to modify anything or search for functions, this would be where to look. See this folder for further explanations.




### Samples

This folder is where 'Unity Physics' custom authoring is located. This allows RDKitForUnity to be more easily modified and add functionality for things such as atom-level interactions, if so desired.





### Scenes

This folder contains the primary 'Scene' (the environment that is visually displayed within Unity) and 'Subscenes' (These are contained within the primary scene, and are used for ECS instantiation purposes). You should not need to manually modify this unless you wish to add other subscenes.




### Settings

These are different asset rendering profiles that can be selected for use within the project.
