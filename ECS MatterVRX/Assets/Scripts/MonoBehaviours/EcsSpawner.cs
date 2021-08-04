using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Physics;
using E7.ECS.LineRenderer;
using System.Collections.Generic;
using System.IO;

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
        Load("5tt.nii");
    }

    void Load(string fileName = "T1w_acpc_dc_restore_brain.nii.gz")
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
        Nifti.NET.Nifti<float> nifti = DataReader.ParseNifti(out maxAmp, fileName);
        Debug.Log("dimensions : " + nifti.Dimensions[0] + " ; " + nifti.Dimensions[1] + " ; " + nifti.Dimensions[2]);

        int sizeX = size;
        int sizeY = size;
        int sizeZ = size;
        int offset = 50;
        if (wholeFile)
        {
            sizeX = nifti.Dimensions[0];
            sizeY = nifti.Dimensions[1];
            sizeZ = nifti.Dimensions[2];
            offset = 0;
        }

        Dictionary<int3, int4> annotations = new Dictionary<int3, int4>();
        string annotationPath = "Assets/Resources/Saves/" + fileName.Split('.')[0] + ".txt";
        if (File.Exists(annotationPath))
        {
            StreamReader reader = new StreamReader(annotationPath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var halfs = line.Split(':');
                var coords = halfs[0].Split(',');
                int3 k = new int3(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
                coords = halfs[1].Split(',');
                int4 v = new int4(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3]));

                annotations.Add(k, v);
            }
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float voxelValue = nifti[offset + x, offset + y, offset + z] / maxAmp;
                    if (voxelValue <= 0) continue;

                    Entity entity = entityManager.CreateEntity(voxelArchetype); //entities[x + y * size + z * size2];
                    entityManager.SetComponentData(entity, new Translation { Value = new float3(x, y, z) });

                    Vector4 color = DataReader.ConvertAmplitudeToColor(voxelValue, DataReader.ColorMap.Grey);
                    entityManager.SetComponentData(entity, new MainColorComponent { value = color });
                    entityManager.SetComponentData(entity, new OutlineColorComponent { value = color });

                    float scale = voxelValue <= 0 ? minSize : UnityEngine.Random.Range(minSize, maxSize);
                    entityManager.SetComponentData(entity, new Scale { Value = scale });

                    int3 pos = new int3(x, y, z);
                    int4 annot = new int4(-1, -1, -1, -1);
                    if (annotations.ContainsKey(pos)) annot = annotations[pos];

                    entityManager.SetComponentData(entity, new VoxelComponent
                    {
                        basePosition = new float3(x, y, z),
                        baseScale = scale,
                        filtered = false,
                        value = voxelValue,
                        //annotationsIds = new DynamicBuffer<BufferInt>()
                        annotationsIds = annot
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
