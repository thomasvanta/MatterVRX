using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;

public class EcsSpawnerRandom : MonoBehaviour
{

    [SerializeField] private int size = 10;
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 0.5f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    // Start is called before the first frame update
    void Start()
    {
        randomGen();
    }

    private void randomGen()
    {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(CustomColor),
            typeof(RenderBounds)
            );

        NativeArray<Entity> entities = new NativeArray<Entity>(size * size * size, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entities);

        int size2 = size * size;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Entity entity = entities[x + y * size + z * size2];
                    Vector4 color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    entityManager.SetComponentData(entity, new CustomColor { value = color });
                    entityManager.SetComponentData(entity, new Translation { Value = new float3(x, y, z) });
                    entityManager.SetComponentData(entity, new Scale { Value = UnityEngine.Random.Range(minScale, maxScale) });

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
