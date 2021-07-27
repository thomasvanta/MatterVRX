using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public static class InputManager
{
    //Movement
    //parameters
    public static float verticalSpeed = 1;
    public static float horizontalSpeed = 1;
    //updates
    public static float3 direction = float3.zero;
    public static float3 globalPosition = float3.zero;


    //Zoom
    //parameters
    public static float zoomSpeed = 1;
    public static float zoomCenterOffset = 1.5f;
    public static float renderDist = 50;
    //update
    public static float zoomFactor = 1;
    public static float3 zoomPivot = float3.zero;
    public static float zoomGlobal = 1;


    //user pos
    //parameters
    public static float colliderDist = 50;
    //update
    public static float3 userPos = float3.zero;


    //Reset
    public static bool doLineReset = false;
    public static bool doVoxelReset = false;

    //Filter
    public static bool doFilter = false;
    public static Filters filterMode = Filters.None;
    public static float valueFilter = 0;

    //Color map
    public static bool changedColormap = false;
    public static DataReader.ColorMap colormap = DataReader.ColorMap.Grey;


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
