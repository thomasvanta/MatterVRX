using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;


/*
public class AddColliderSystem : JobComponentSystem
{
    EntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float3 userPos = InputManager.userPos;
        float dist = InputManager.colliderDist;

        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithAll<VoxelFlag>()
            .WithNone<PhysicsCollider>()
            .ForEach((Entity entity, int index, in Translation pos, in Scale scale) => {
                if (math.distance(pos.Value, userPos) < dist)
                {
                    
                    BoxGeometry box = new BoxGeometry
                    {
                        Center = float3.zero,
                        Orientation = quaternion.identity,
                        Size = new float3(scale.Value, scale.Value, scale.Value)
                    };
                    entityCommandBuffer.AddComponent(index, entity, new PhysicsCollider { Value = Unity.Physics.BoxCollider.Create(box) });
                }
        }).Schedule(inputDeps);

        entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}

public class RemoveColliderSystem : JobComponentSystem
{
    EntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float3 userPos = InputManager.userPos;
        float dist = InputManager.colliderDist;

        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithAll<VoxelFlag, PhysicsCollider>()
            .ForEach((Entity entity, int index, in Translation pos) => {
                if (math.distance(pos.Value, userPos) > dist)
                {
                    entityCommandBuffer.RemoveComponent(index, entity, ComponentType.ReadWrite<PhysicsCollider>());
                }
            }).Schedule(inputDeps);

        entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
*/