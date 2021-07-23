using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class FilterSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float3 hiddenOffset = new float3(0, 100000, 0) * InputManager.zoomGlobal;
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
                    jobHandle = Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled).WithNone<SelectedFlag>().ForEach((ref Translation translation, ref VoxelComponent voxel) => {

                        voxel.filtered = true;
                        translation.Value += hiddenOffset;

                    }).Schedule(inputDeps);
                    break;

                case Filters.Unselected:
                    inputDeps = ResetFilters(inputDeps);
                    jobHandle = Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled).ForEach((ref Translation translation, ref VoxelComponent voxel, in SelectedFlag flag) => {

                        voxel.filtered = true;
                        translation.Value += hiddenOffset;

                    }).Schedule(inputDeps);
                    break;

                case Filters.OnValue:
                    inputDeps = ResetFilters(inputDeps);

                    float value = InputManager.valueFilter;
                    jobHandle = Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled).ForEach((ref Translation translation, ref VoxelComponent voxel) => {

                        if (voxel.value < value)
                        {
                            voxel.filtered = true;
                            translation.Value += hiddenOffset;
                        }

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
        float3 hiddenOffset = new float3(0, 100000, 0) * InputManager.zoomGlobal;
        JobHandle jobHandle = Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled).ForEach((ref Translation translation, ref VoxelComponent voxel) => {

            if (voxel.filtered)
            {
                voxel.filtered = false;
                translation.Value -= hiddenOffset;
            }

        }).Schedule(inputDeps);

        return jobHandle;
    }
}
