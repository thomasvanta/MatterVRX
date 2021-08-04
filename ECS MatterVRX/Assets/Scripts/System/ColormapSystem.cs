using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

public class ColormapSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!InputManager.changedColormap) return default;
        InputManager.changedColormap = false;

        DataReader.ColorMap map = InputManager.colormap;
        JobHandle jobHandle = Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled).ForEach((ref MainColorComponent mainColor, in VoxelComponent voxel) => {

            mainColor.value = DataReader.ConvertAmplitudeToColor(voxel.value, map);

        }).Schedule(inputDeps);

        jobHandle.Complete();
        CSSSystem.applyCSS = true;

        return jobHandle;
    }
}
