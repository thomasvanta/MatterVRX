using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using E7.ECS.LineRenderer;

public class VoxelMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float3 dir = InputManager.direction;

        JobHandle jobHandle = Entities.ForEach((ref Translation translation, in VoxelComponent flag) => {
            translation.Value += dir * deltaTime;
        }).Schedule(inputDeps);

        return jobHandle;
    }
}

public class LineMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float3 dir = InputManager.direction;

        JobHandle jobHandle = Entities.ForEach((ref LineSegment line, in LineComponent flag) => {
            line.to += dir * deltaTime;
            line.from += dir * deltaTime;
        }).Schedule(inputDeps);

        return jobHandle;
    }
}
