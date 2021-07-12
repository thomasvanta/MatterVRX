using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float3 dir = InputManager.direction;

        JobHandle jobHandle =  Entities.ForEach((ref Translation translation, in VoxelFlag flag) => {
            translation.Value += dir * deltaTime;
        }).Schedule(inputDeps);

        return jobHandle;
    }
}
