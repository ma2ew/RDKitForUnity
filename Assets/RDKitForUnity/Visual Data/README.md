# Visual Data

This folder contains data corresponding to molecular objects (e.g. Bond Variant.prefab), and a section that defines both the color of atoms and their respective bonds as well as defining atomic radii
<br><br><br>

## Contents
* [Prefab Explanations](#bond-variant-and-ummappointrenderer-variant-prefabs)
* [CPKColorsAndRadii](#cpkcolorsandradii)
* [RenderingData](#renderingdata)
* [Configurable Rendering Options](#android-preset-testmaterial-and-performance-urp-config)
<br><br><br>

## "Bond Variant" and "UMAPPointRenderer Variant" Prefabs

The name "UMAPPointRenderer" is an artifact from a closely related, but not tightly coupled, project under the name "PubChemVIZ". This prefab is actually the prefab used to render atoms. Both of these are used in Desktop and VR modes, but in different ways. While the VR mode simply grabs the mesh data, The Desktop mode will grab all object information in a "baked" format (See the README in the 'Scripts' folder) that can be used for Entity-Component-System (ECS) functionalities.
<br><br>

## CPKColorsAndRadii

This file defines an enum called **Element**, which is used to send RDKit and RDKitForUnity information on a given atom. It possesses a static class named "AtomicRadiiAndColors" which contains functions that return 3 of the following items:

1) **Radius(Element element)**: Uses a switch statement which returns a **float** that corresponds to a given atom's atomic radius.
2) **ElementColor(Element element)**: Uses a switch statement which returns a **float4** (a list object containing four float values) that correspond to a given atom's CPK colors
3) **Parse(string symbol)**: If necessary, this will parse a string symbol into a **Element** enum. RDKit can identify an atom by atomic number (which should directly be transferable to a Element enum value), so this should shouldn't be necessary for 99% of use cases.
<br><br>

## RenderingData

This file creates a "ScriptableObject" that allows the user to change between various models of molecular visualization. These include:
* Ball+Stick Models: This is the conventional atom+bond depiction
* Bondline Models: This strictly depicts the bonds themselves and excludes the atoms from the display
* Simple Ball+Stick: This is equivalent to Ball+Stick, but excludes bond orders
* Simple Bondline: This is equivalent to Bondline, but excludes bond orders
<br><br>

## Android Preset, TestMaterial and Performance URP Config

Android Preset: This is the default preset used in the render pipeline to render the project. Can be changed by the developer if so desired. The preset chosen provides a decent match of rendering quality and performance.

Performance URP Config: This is also a performance-centered rendering configuration file. This contains multiple options for visual fidelity and noise reduction options that can be manually modified by the developer.

TestMaterial.mat: This is the material placed onto atoms and bond prefabs. It is designed to be a performant material which does not significantly impact framerates, but can be swapped out or modified if necessary
