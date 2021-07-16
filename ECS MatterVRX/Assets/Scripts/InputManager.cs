using Unity.Mathematics;
public static class InputManager
{
    //Movement
    public static float3 direction  = float3.zero;

    //Zoom
    public static float zoomFactor = 1;
    public static float3 zoomPivot = float3.zero;

    //user pos
    public static float3 userPos = float3.zero;
    public static float colliderDist = 50;

    //Camera
    public static float3 camPos;
}
