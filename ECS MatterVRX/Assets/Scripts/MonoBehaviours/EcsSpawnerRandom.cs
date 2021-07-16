using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Physics;
using E7.ECS.LineRenderer;
using System.Collections.Generic;

public class EcsSpawnerRandom : MonoBehaviour
{
    [SerializeField] private int size = 10;
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 0.5f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private UnityEngine.Material voxelMaterial;
    [SerializeField] private UnityEngine.Material lineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        randomGen();
    }

    void randomGen()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype voxelArchetype = entityManager.CreateArchetype(
            typeof(VoxelComponent),
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(MainColorComponent),
            typeof(OutlineColorComponent),
            typeof(OutlineComponent)
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
                    entityManager.SetComponentData(entity, new OutlineColorComponent { value = color });

                    float scale = UnityEngine.Random.Range(minSize, maxSize);
                    entityManager.SetComponentData(entity, new Scale { Value = scale });

                    entityManager.SetComponentData(entity, new VoxelComponent
                    {
                        basePosition = new float3(x, y, z),
                        baseScale = scale,
                        filtered = false
                    });

                    entityManager.SetComponentData(entity, new OutlineComponent { isSelected = false, color = new float4(1, 1, 1, 1) });

                    entityManager.SetSharedComponentData(entity, new RenderMesh
                    {
                        mesh = mesh,
                        material = voxelMaterial
                    });
                }
            }
        }

        Vector3Int v3i;
        List<Vector3Int> list = DataReader.ReadStreamlineInt(out v3i, "fake-output.txt");
        float3 v = new float3(v3i.x, v3i.y, v3i.z);

        foreach (Vector3Int dv3i in list)
        {
            float3 dv = new float3(dv3i.x, dv3i.y, dv3i.z);

            var e = entityManager.CreateEntity();
            entityManager.AddComponentData(e, new LineSegment(v, v + dv));
            entityManager.AddSharedComponentData(e, new LineStyle { material = lineMaterial });

            v += dv;
        }

        DataReader.PrintParsed();

        entities.Dispose();
    }
}
