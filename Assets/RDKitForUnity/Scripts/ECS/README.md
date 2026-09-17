# ECS/Core Functionality Folder

This folder houses the functionality central to RDKitForUnity.
<br><br>

# Contents

* [Authoring](#authoring-folder)
* [AtomRenderBaker](#atomrenderbaker)
* [ECSJobs](#ecsjobs)
* [RDKitForUnity](#rdkitforunity)
* [RDKFUHelperFuncs](#rdkfuhelperfuncs)
<br><br>

# Folders and Files
<br>

## Authoring Folder

This folder contains "authoring" files that allow us to identify a prefab that corresponds to an atom (AtomRenderAuthoring.cs) or a bond (BondRenderAuthoring.cs). These will be passed to the baker in order to be converted to ECS entities.

<br><br>

## AtomRenderBaker

This file converts the prefabs previously defined in the "Authoring" section into ECS-compatible entities. The baking of atom and bond prefabs are handled within this single file.
<br><br>

## ECSJobs

This file contains ECS 'Jobs', which are highly performant functions handled within Unity's runtime. The two functions here are labelled as "AtomPlotterJob" and "BondPlotterJob". both of these jobs are parallelized for higher performance, and are called by the RDKitForUnity script when dealing with Desktop molecules.
* **AtomPlotterJob**: This job is used to instantiate atoms for a given molecular structure. It accepts:
  * An **Entity** as a template atom
  * A **Parallel Writer Entity-Command-Buffer (ECB)** to handle parallel writing of entities
  * A **NativeArray of float3 objects** corresponding to the xyz locations of all atoms
  * A **NativeArray of Element enums** used to retrieve the corresponding colors and radii of the atoms
  * It also possesses a built-in function to convert gamma color values to linear values for to increase the color quality, along with maintaining the color consistency between VR and Desktop molecules. This is also a full function provided in RDKFUHelperFuncs, so this can be used outside of jobs as well if necessary.
* **BondPlotterJob**: This job is used to instantiate bond cylinders for a given molecular structure. It accepts:
  * An **Entity** as a template bond cylinder
  * A **Parallel Writer Entity-Command-Buffer (ECB)** to handle parallel writing of entities
  * A **NativeArray of float4x4 objects** corresponding to the matrix values of all bonds
  * A **NativeArray of float4 objects** used to retrieve the corresponding colors of the bonds
  * The bonds have their BondPrototypeTag component removed, preventing them from being flagged as uninitialized templates while retaining full ECS entity functionality for rendering.<br>

NOTE: Changes will be made very shortly to BondPlotterJob, moving gamma to linear modifications from the main thread to the bond job itself.
  <br><br>

## RDKitForUnity
<br><br>

## RDKFUHelperFuncs
<br><br>
