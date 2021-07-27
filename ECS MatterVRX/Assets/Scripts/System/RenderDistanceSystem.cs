using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;



public class VoxelEnabler : ComponentSystem
{
    EntityQuery query;
    protected override void OnCreate()
    {
        var queryDesc = new EntityQueryDesc
        {
            Options = EntityQueryOptions.IncludeDisabled,
            All = new ComponentType[] { 
                ComponentType.ReadWrite<Disabled>(),
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Scale>(),
                ComponentType.ReadOnly<VoxelComponent>()
            }
            

        };
        query = GetEntityQuery(queryDesc);
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        float dist = InputManager.renderDist * InputManager.zoomGlobal;
        float distsq = dist * dist;
        float3 center = InputManager.zoomPivot;
        float3 globalPos = InputManager.globalPosition;
        float globalScale = InputManager.zoomGlobal;

        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
        NativeArray<VoxelComponent> voxels = query.ToComponentDataArray<VoxelComponent>(Allocator.Temp);


        for (int i = 0; i < entities.Length; i++)
        {
            float3 newPos = globalPos + globalScale * voxels[i].basePosition;
            if (math.distancesq(newPos, center) < distsq && !voxels[i].filtered)
            {
                PostUpdateCommands.RemoveComponent<Disabled>(entities[i]);
                PostUpdateCommands.SetComponent(entities[i], 
                    new Translation { Value = newPos }
                    );
                PostUpdateCommands.SetComponent(entities[i],
                    new Scale { Value = globalScale * voxels[i].baseScale }
                    );
                
            }
        }

        entities.Dispose();
        voxels.Dispose();
    }
}

public class EntityDisabler : ComponentSystem
{
    EntityQuery query;
    protected override void OnCreate()
    {
        var queryDesc = new EntityQueryDesc
        {
            None = new ComponentType[] { ComponentType.ReadOnly<Disabled>() },
            All = new ComponentType[]  { ComponentType.ReadOnly<Translation>() }
        };
        query = GetEntityQuery(queryDesc);
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        float dist = InputManager.renderDist * InputManager.zoomGlobal;
        float distsq = dist * dist;
        float3 center = InputManager.zoomPivot;
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
        NativeArray<Translation> positions = query.ToComponentDataArray<Translation>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            if (math.distancesq(positions[i].Value, center) > distsq)
            {
                PostUpdateCommands.AddComponent<Disabled>(entities[i]);
            }
        }

        entities.Dispose();
        positions.Dispose();
    }
}
