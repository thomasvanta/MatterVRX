using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using System.IO;

public class DictationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!DictationEngine.recording) return default;

        int dictIndex = DictationEngine.currentIndex;
        JobHandle jobHandle = Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled).ForEach((ref VoxelComponent voxel, in SelectedFlag flag) => { // .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled) if we want to annotate disabled but selected voxels

            for (int i = 0; i < 4; i++)
            {
                if (voxel.annotationsIds[i] == dictIndex) return;
            }
            
            for (int i = 0; i < 4; i++)
            {
                if (voxel.annotationsIds[i] < 0)
                {
                    voxel.annotationsIds[i] = dictIndex;
                    voxel.annotationsIds[(i + 1) % 4] = -1; // cyclical overridance of annotations
                    return;
                }
            }

        }).Schedule(inputDeps);

        return jobHandle;
    }
}