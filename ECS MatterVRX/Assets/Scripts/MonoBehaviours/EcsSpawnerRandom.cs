using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Physics;

public class EcsSpawnerRandom : MonoBehaviour
{
    [SerializeField] private int size = 10;
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 0.5f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private UnityEngine.Material material;

    // Start is called before the first frame update
    void Start()
    {
        randomGen();
    }

    void randomGen()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype voxelArchetype = entityManager.CreateArchetype(
            typeof(VoxelFlag),
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(MainColorComponent),
            //typeof(PhysicsCollider)
            );

        NativeArray<Entity> entities = new NativeArray<Entity>(size * size * size, Allocator.Temp);
        entityManager.CreateEntity(voxelArchetype, entities);

        int size2 = size * size;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Entity entity = entities[x + y * size + z * size2];
                    entityManager.SetComponentData(entity, new Translation { Value = new float3(x, y, z) });
                    
                    Vector4 color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    entityManager.SetComponentData(entity, new MainColorComponent { value = color });

                    float scale = UnityEngine.Random.Range(minSize, maxSize);
                    entityManager.SetComponentData(entity, new Scale { Value = scale });

                    //BlobAssetReference<Unity.Physics.Collider> collider = Unity.Physics.BoxCollider.Create(new BoxGeometry
                    //{
                    //    Center = new float3(x, y, z),
                    //    Orientation = quaternion.identity,
                    //    Size = new float3(scale, scale, scale),
                    //    BevelRadius = 0.05f
                    //});
                    //PhysicsCollider colliderComponent = new PhysicsCollider { Value = collider };
                    //entityManager.SetComponentData(entity, colliderComponent);

                    entityManager.SetSharedComponentData(entity, new RenderMesh
                    {
                        mesh = mesh,
                        material = material
                    });
                }
            }
        }
        entities.Dispose();
    }

}
