# MatterVRX
"Matter VR Explorer" is a VR prototype to display a voxel model to visualize matter data.

## Controls
This is the features that are currently implemented, along with their controls :  
- **Move voxels horizontally** : touching the left track pad in the wanted direction
- **Move voxels vertically** : touching the right track pad in the wanted direction
- **Zoom** : hold both triggers and change distance between the controllers to zoom in or out
- **Select a voxel** : point a voxel with one of the controller and grab the grip of said controller

## Control Panel
The control panel is an interactive user interface : one can press buttons by pointing it with the **right** controller and pressing the **trigger**.  
These are the currently implemented buttons on this panel :  
- **Reset** : resets the voxels positions and scales
- **Filters** : a dropdown menu which allows one to choose a filter in order to disable certain voxels according to some criteria :
  - *No Filter* : no filter, all voxels are enabled
  - *Selected* : all unselected voxels are disabled
  - *Unselected* : all selected voxels are disabled
  - *On Value* : this filter uses the slider beneath the button : all voxels which have a value less than the slider's are disabled
- **Colormaps** : a dropdown menu which allows one to change the colormap used for coloring voxels. There currently are 4 colormaps : *Grey, Hot, Cool, Jet*.
- **Record annotation** : starts a voice recognition engine which converts voice to text and saves the annotation to all selected voxels
