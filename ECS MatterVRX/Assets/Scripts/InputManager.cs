using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public static class InputManager
{
    /* Singleton used to pass data to the systems
     * prameters fields are fixed when the file is loaded 
     * updates fields are updated each frame by monobihaviours scripts
     */


    //Movement
    //parameters
    public static float verticalSpeed = 1;                      // speed factor for vertical speed
    public static float horizontalSpeed = 1;                    // speed factor for horisontal speed
    //updates
    public static float3 direction = float3.zero;               // input direction given by the controlers
                                                                // already take into account movement speed
                                                                // updated in VerticalMovement.cs and HorizontalMovement.cs

    public static float3 globalPosition = float3.zero;          // position of the (0,0,0) of the voxel cluster
                                                                // relative to the world
                                                                // updated in VerticalMovement.cs and HorizontalMovement.cs


    //Zoom
    //parameters
    public static float zoomSpeed = 1;                          // speed factor for the zoom
    public static float zoomCenterOffset = 1.5f;                // distance of the zoom center (zoomPivot)
                                                                // relative to the user head 

    public static float renderDist = 50;                        // voxel render distance (in voxel) 
    //update
    public static float zoomFactor = 1;                         // zoom difference between frame
                                                                // (new frame zoom/old frame zoom)

    public static float3 zoomPivot = float3.zero;               // position of the zoom center ralative to the world
    public static float zoomGlobal = 1;                         // keep track of the cuurent zoom relative to the loading state
                                                                // the 3 fileds above are updated in Zoom.cs


    //user pos
    //parameters
    public static float colliderDist = 5;                       // colision render distance (in voxel)
    //update
    public static float3 userPos = float3.zero;                 // keep track of the user head position in the world
                                                                // updated in InputUpdater.cs


    //Reset
    public static bool doLineReset = false;                     // trigger for line reset
    public static bool doVoxelReset = false;                    // trigger for voxel reset

    //Filter
    public static bool doFilter = false;                        // trigger for filter changes
    public static Filters filterMode = Filters.None;            // current filter mode
    public static float valueFilter = 0;                        // value for value dependant filter

    //Color map
    public static bool changedColormap = false;                 // trigger for color map change
    public static DataReader.ColorMap colormap = DataReader.ColorMap.Grey;  // current colormap


    public static void Reset()
    {
        //Movement
        globalPosition = float3.zero;

        //Zoom
        zoomFactor = 1;
        zoomPivot = float3.zero;
        zoomGlobal = 1;

        //Reset
        doLineReset = true;
        doVoxelReset = true;
}
}
