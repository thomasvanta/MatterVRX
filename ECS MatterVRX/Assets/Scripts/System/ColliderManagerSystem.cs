using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;


public class AddColliderSystem : ComponentSystem
{
    EntityQuery query;
    protected override void OnCreate()
    {
        var queryDesc = new EntityQueryDesc
        {
            None = new ComponentType[] { ComponentType.ReadOnly<PhysicsCollider>() },
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<VoxelFlag>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Scale>()
            }
        };
        query = GetEntityQuery(queryDesc);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        float3 userPos = InputManager.userPos;
        float dist = InputManager.colliderDist;

        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
        NativeArray<Translation> positions = query.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<Scale> scales = query.ToComponentDataArray<Scale>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            if (math.distance(positions[i].Value, userPos) < dist)
            {
                
                BoxGeometry box = new BoxGeometry
                {
                    Center = float3.zero,
                    Orientation = quaternion.identity,
                    Size = new float3(scales[i].Value)
                };
                PostUpdateCommands.AddComponent(entities[i], 
                    new PhysicsCollider { Value = Unity.Physics.BoxCollider.Create(box) });
            }
        }

        entities.Dispose();
        positions.Dispose();
        scales.Dispose();
                
    }
}


public class RemoveColliderSystem : ComponentSystem
{
    EntityQuery query;
    protected override void OnCreate()
    {
        var queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<VoxelFlag>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadWrite<PhysicsCollider>()
            }
        };
        query = GetEntityQuery(queryDesc);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        float3 userPos = InputManager.userPos;
        float dist = InputManager.colliderDist;

        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
        NativeArray<Translation> positions = query.ToComponentDataArray<Translation>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            if (math.distance(positions[i].Value, userPos) > dist)
            {

                PostUpdateCommands.RemoveComponent<PhysicsCollider>(entities[i]);
            }
        }

        entities.Dispose();
        positions.Dispose();
    }
}
