using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class FilterSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (InputManager.doFilter)
        {
            InputManager.doFilter = false;
            JobHandle jobHandle = default;

            switch (InputManager.filterMode)
            {
                case Filters.None:
                    jobHandle = ResetFilters(inputDeps);
                    break;

                case Filters.Selected:
                    inputDeps = ResetFilters(inputDeps);
                    jobHandle = Entities.WithNone<SelectedFlag>().ForEach((Entity entity, ref Translation translation, ref VoxelComponent voxel) => {

                        float3 hiddenOffset = new float3(0, 100000, 0);
                        voxel.filtered = true;
                        translation.Value += hiddenOffset;

                    }).Schedule(inputDeps);
                    break;

                case Filters.Unselected:
                    inputDeps = ResetFilters(inputDeps);
                    jobHandle = Entities.ForEach((Entity entity, ref Translation translation, ref VoxelComponent voxel, in SelectedFlag flag) => {

                        float3 hiddenOffset = new float3(0, 100000, 0);
                        voxel.filtered = true;
                        translation.Value += hiddenOffset;

                    }).Schedule(inputDeps);
                    break;

                default:
                    break;
            }

            return jobHandle;
        }
        else return default;
    }

    private JobHandle ResetFilters(JobHandle inputDeps)
    {
        JobHandle jobHandle = Entities.ForEach((ref Translation translation, ref VoxelComponent voxel) => {

            float3 hiddenOffset = new float3(0, 100000, 0);
            if (voxel.filtered)
            {
                voxel.filtered = false;
                translation.Value -= hiddenOffset;
            }

        }).Schedule(inputDeps);

        return jobHandle;
    }
}
