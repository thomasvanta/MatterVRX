using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public static class InputManager
{
    //Movement
    public static float3 direction = float3.zero;
    public static float3 globalPosition = float3.zero;

    //Zoom
    public static float zoomFactor = 1;
    public static float3 zoomPivot = float3.zero;
    public static float zoomGlobal = 1;

    //user pos
    public static float3 userPos = float3.zero;
    public static float colliderDist = 50;

    //Camera
    public static float3 camPos;

    //Reset
    public static bool doReset = false;

    //Filter
    public static bool doFilter = false;
    public static Filters filterMode = Filters.None;
}
