using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using E7.ECS.LineRenderer;

public class VoxelResetSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (InputManager.doReset)
        {
            InputManager.doReset = false;
            JobHandle jobHandle = Entities.ForEach((ref Translation translation, ref Scale scale, ref VoxelComponent voxel) =>
            {

                translation.Value = voxel.basePosition;
                scale.Value = voxel.baseScale;
                voxel.filtered = false;

            }).Schedule(inputDeps);

            return jobHandle;
        }
        else return default;
    }
}

public class LineResetSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (InputManager.doReset)
        {
            InputManager.doReset = false;
            JobHandle jobHandle = Entities.ForEach((ref LineSegment line, ref LineComponent lineData) =>
            {

                line.from = lineData.baseFrom;
                line.to = lineData.baseTo;
                lineData.filtered = false;

            }).Schedule(inputDeps);

            return jobHandle;
        }
        else return default;
    }
}
