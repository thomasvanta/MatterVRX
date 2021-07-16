using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class ResetSystem : JobComponentSystem
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
