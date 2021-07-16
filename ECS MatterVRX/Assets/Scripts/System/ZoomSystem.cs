using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class ZoomSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float3 pivot = InputManager.zoomPivot;
        float zoomFactor = InputManager.zoomFactor;

        JobHandle jobHandle = Entities.ForEach((ref Translation translation, ref Scale scale, in VoxelComponent flag) => {

            float3 delta = translation.Value - pivot;
            translation.Value = pivot + delta * zoomFactor;
            scale.Value *= zoomFactor;

        }).Schedule(inputDeps);

        return jobHandle;
    }

}
