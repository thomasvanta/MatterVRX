using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Physics;
using E7.ECS.LineRenderer;
using System.Collections.Generic;

public class EcsSpawner : MonoBehaviour
{
    [SerializeField] private bool wholeFile = false;
    [SerializeField] private int size = 10;
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 0.5f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private UnityEngine.Material voxelMaterial;
    [SerializeField] private UnityEngine.Material lineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (wholeFile) fullLoad();
        else LimitedLoad();
    }

    void LimitedLoad()
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

        //NativeArray<Entity> entities = new NativeArray<Entity>(size * size * size, Allocator.Temp);
        //entityManager.CreateEntity(voxelArchetype, entities);

        float maxAmp;
        Nifti.NET.Nifti<float> nifti = DataReader.ParseNifti(out maxAmp);
        Debug.Log("dimensions : " + nifti.Dimensions[0] + " ; " + nifti.Dimensions[1] + " ; " + nifti.Dimensions[2]);

        //int size2 = size * size;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    float voxelValue = nifti[50 + x, 50 + y, 50 + z] / maxAmp;
                    if (voxelValue <= 0) continue;

                    Entity entity = entityManager.CreateEntity(voxelArchetype); //entities[x + y * size + z * size2];
                    entityManager.SetComponentData(entity, new Translation { Value = new float3(x, y, z) });

                    Vector4 color = DataReader.ConvertAmplitudeToColor(voxelValue, DataReader.ColorMap.Grey);
                    entityManager.SetComponentData(entity, new MainColorComponent { value = color });
                    entityManager.SetComponentData(entity, new OutlineColorComponent { value = color });

                    float scale = voxelValue <= 0 ? minSize : UnityEngine.Random.Range(minSize, maxSize);
                    entityManager.SetComponentData(entity, new Scale { Value = scale });

                    entityManager.SetComponentData(entity, new VoxelComponent
                    {
                        basePosition = new float3(x, y, z),
                        baseScale = scale,
                        filtered = false,
                        value = voxelValue,
                        //annotationsIds = new DynamicBuffer<BufferInt>()
                        annotationsIds = new int4(-1, -1, -1, -1)
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

        //entities.Dispose();

        //GenerateIntLines(ref entityManager);
    }

    private void fullLoad()
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


        float maxAmp;
        Nifti.NET.Nifti<float> nifti = DataReader.ParseNifti(out maxAmp);
        Debug.Log("dimensions : " + nifti.Dimensions[0] + " ; " + nifti.Dimensions[1] + " ; " + nifti.Dimensions[2]);


        for (int x = 0; x < nifti.Dimensions[0]; x++)
        {
            for (int y = 0; y < nifti.Dimensions[1]; y++)
            {
                for (int z = 0; z < nifti.Dimensions[2]; z++)
                {
                    float voxelValue = nifti[x, y, z] / maxAmp;
                    if (voxelValue <= 0) continue;

                    Entity entity = entityManager.CreateEntity(voxelArchetype);
                    entityManager.SetComponentData(entity, new Translation { Value = new float3(x, y, z) });

                    Vector4 color = DataReader.ConvertAmplitudeToColor(voxelValue, DataReader.ColorMap.Grey);
                    entityManager.SetComponentData(entity, new MainColorComponent { value = color });
                    entityManager.SetComponentData(entity, new OutlineColorComponent { value = color });

                    float scale = voxelValue <= 0 ? minSize : UnityEngine.Random.Range(minSize, maxSize);
                    entityManager.SetComponentData(entity, new Scale { Value = scale });

                    entityManager.SetComponentData(entity, new VoxelComponent
                    {
                        basePosition = new float3(x, y, z),
                        baseScale = scale,
                        filtered = false,
                        value = voxelValue,
                        //annotationsIds = new DynamicBuffer<BufferInt>()
                        annotationsIds = new int4(-1, -1, -1, -1)
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
    }

    private void GenerateIntLines(ref EntityManager entityManager)
    {
        EntityArchetype lineArchetype = entityManager.CreateArchetype(
            typeof(LineComponent),
            typeof(LineSegment),
            typeof(LineStyle),
            typeof(MainColorComponent),
            typeof(OutlineColorComponent)
            );

        List<DataReader.IntStreamline> streamlines = DataReader.ReadIntLines();

        foreach (DataReader.IntStreamline line in streamlines)
        {
            if (line.strength <= 0 || line.line.Count <= 0) continue;

            int n = line.line.Count;
            NativeArray<Entity> lines = entityManager.CreateEntity(lineArchetype, n, Allocator.Temp);

            float3 v = new float3(line.start.x, line.start.y, line.start.z);
            for (int i = 0; i < n; i++)
            {
                float3 dv = new float3(line.line[i].x, line.line[i].y, line.line[i].z);

                float lineWidth = 0.1f * line.strength;
                entityManager.SetComponentData(lines[i], new LineComponent { baseFrom = v, baseTo = v + dv, filtered = false, baseWidth = lineWidth });
                entityManager.SetComponentData(lines[i], new LineSegment(v, v + dv));
                entityManager.SetSharedComponentData(lines[i], new LineStyle { material = lineMaterial });
                entityManager.SetComponentData(lines[i], new MainColorComponent { value = new float4(line.mixedColor.r / 255f, line.mixedColor.g / 255f, line.mixedColor.b / 255f, 1) });
                entityManager.SetComponentData(lines[i], new OutlineColorComponent { value = new float4(0, 0, 0, 0) });

                v += dv;
            }

            lines.Dispose();
        }
    }
}
