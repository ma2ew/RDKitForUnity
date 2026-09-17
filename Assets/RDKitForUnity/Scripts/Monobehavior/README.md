# Monobehavior Folder

This folder, though misleadingly named, does contain strictly Monobehavior scripts. There are two scripts of interest.

1) **Camera Movement**: This script can be attached to a camera and allows it to be controlled using desktop controls (mouse dragging and keyboard input) Some of the controls are as follows:
    * WASD keys - Forwards, left, backwards, and right controls respectively.
    * Left click + Mouse drag - Rotate around a fixed point located at the center of the scene.
    * Right click + Mouse drag - Pans the camera.
    * 'N' key - Activates 'No-Clip' allowing the camera to freely fly around.
    * 'O' key - Reorients the camera back to the original rotation and position. Useful if the user is lost.

2) **Testrun**: This script allows the user to test out the molecule draw function, depending on what url you insert into the 'string' test variable. the 'P' key allows the user to create a desktop molecule, and the 'V' key lets the user create a VR/GPU-instanced molecule.

NOTE: The VR/GPU-instanced object is more simple to work with that the Desktop/ECS version, but can suffer from performance issues due to the instancing being handed via the main thread of the application
