using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

public class DictationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!DictationEngine.recording) return default;

        int i = DictationEngine.currentIndex;
        JobHandle jobHandle = Entities.ForEach((ref VoxelComponent voxel, in SelectedFlag flag) => {

            //voxel.annotationsIds.Add(new BufferInt { value = i });
            voxel.annotationId = i;

        }).Schedule(inputDeps);

        return jobHandle;
    }
}
