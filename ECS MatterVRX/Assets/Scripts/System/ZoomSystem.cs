using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using E7.ECS.LineRenderer;

public class VoxelZoomSystem : JobComponentSystem
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

public class LineZoomSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float3 pivot = InputManager.zoomPivot;
        float zoomFactor = InputManager.zoomFactor;

        JobHandle jobHandle = Entities.ForEach((ref LineSegment line, in LineComponent flag) => {

            float3 delta = line.to - pivot;
            line.to = pivot + delta * zoomFactor;

            delta = line.from - pivot;
            line.from = pivot + delta * zoomFactor;

        }).Schedule(inputDeps);

        return jobHandle;
    }

}
