using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class ZoomSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        /*
        float3 pivot = InputManager.zoomPivot;
        float zoomFactor = InputManager.zoomFactor;
        float deltaTime = Time.DeltaTime;
        */

        JobHandle jobHandle = Entities.ForEach((ref Translation translation, in VoxelFlag flag) => {
            /*
            float3 delta = translation.Value - pivot;
            translation.Value = pivot + delta * zoomFactor * deltaTime; 
            */
        }).Schedule(inputDeps);

        return jobHandle;
    }

}
