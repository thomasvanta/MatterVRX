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

## Configuration File
Located at ECS MatterVRX/Assets/Configutation/BaseConfig.xml, this file contains editable options for loading and navigating :
- **zoom factor** : the speed at which the voxel cluster is zoomed in or out
- **zoom center offset** : the distance between the user head and the center point of the zoom 
- **render distance** : the distance, expressed in *voxels*, beyond which voxels do not appear
- **selection distance** : the distance, expressed in *voxels*, beyond which voxels cannot be pointed nor selected
- **vertical speed** : the speed at which voxels move vertically
- **horizontal speed** : the speed at which voxels move horizontally
- **file name** : the name of the file to be loaded. It must be a *.nii* or a *.nii.gz*, and must be located in ECS MatterVRX/Assets/Resources/
- **load whole** : whether the file should be fully loaded into the software or not
- **load region**: if the file is not fully loaded, the loaded cuboid within the file


## git clone
when cloning the project you may encounter the following problem
- **Loading blocked on SteamVR_Settings.asset** : there is 2 known methods:
  - *method 1* : wait a bit, then interrupt the loading (alt f4 or task manager). Then restart Unity
  - *method 2* : interrupt the loading, delete the 3 folder Assets/SteamVR*, and start the unity project again by ignoring the warning. When in the project, use the package manager to remove the SteamVr SDk, then reinstall it. If the loading block again on the SteamVR_Settings.asset, force restart Unity
- **missing mesh** :
  - *default mesh for the minimap* : the .obj is to big to be pushed on git, you need to ask the relevant people to give it to you. Import the .obj in the Assets/Obj folder. Once the .obj obtained, put the mesh on the MiniMap gameobject (the mesh is called "default")
  - *various mesh for the voxels* : the meshes for the voxels are in Assets/Obj. You need to give them to the Voxels ECS Spawner -> Ecs Spawner (Script) -> Meshes (Array). Currently, the distribution of shapes is random, with a density defined by the proportions in the array (the distribution used is [tetrahedron, diamond, cube, cube], in this order) 