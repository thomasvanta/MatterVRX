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
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 0.5f;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private UnityEngine.Material voxelMaterial;
    [SerializeField] private UnityEngine.Material lineMaterial;
    [SerializeField] private Transform dummyTumor;
    public static string filename;
    public static string loadMode;
    public static ConfigurationLoader.LoadRegion region;
    public static bool loadStreamlines;
    public static ConfigurationLoader.StreamlineFiles streamlineFiles;
    public static int3 dummyTumorPos;
    public static float dummyTumorRadius;
    public static float dummyTumorPeripherySize;

    [SerializeField] private GameObject brainMap;
    private MeshFilter brainMesh;
    private Transform mapRegion;
    private MinimapDot mapDot;

    // Start is called before the first frame update
    void Start()
    {
        brainMesh = brainMap.GetComponent<MeshFilter>();
        mapRegion = brainMap.transform.GetChild(0);
       // mapDot = mapIndicator.GetComponent<MinimapDot>();
        Load(filename);
    }

    void SetMap(float3 dimensions, float3 start, float3 chunkSize)
    {
        var size = brainMesh.sharedMesh.bounds.size;
        mapRegion.localPosition = start * size * brainMap.transform.localScale / dimensions;
        mapRegion.localScale = chunkSize * size / dimensions;
        print("size: " + mapRegion.localScale.ToString());
        //mapDot.SetMapScale(size / dimensions);
    }

    float3 GetMillimeters(int x, int y, int z, Nifti.NET.Nifti<float> nifti)
    {
        return new float3(
                        x * nifti.Header.srow_x[0] + y * nifti.Header.srow_x[1] + z * nifti.Header.srow_x[2] + nifti.Header.srow_x[3],
                        x * nifti.Header.srow_y[0] + y * nifti.Header.srow_y[1] + z * nifti.Header.srow_y[2] + nifti.Header.srow_y[3],
                        x * nifti.Header.srow_z[0] + y * nifti.Header.srow_z[1] + z * nifti.Header.srow_z[2] + nifti.Header.srow_z[3]
                );
    }

    void Load(string fileName = "T1w_acpc_dc_restore_brain.nii.gz")
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // define voxel archetype
        // note : the voxels are spawn without collision box because of perf issues
        EntityArchetype voxelArchetype = entityManager.CreateArchetype(
            typeof(VoxelComponent), // contain initial values
            typeof(Translation),    // position of the voxel
            typeof(Scale),          // size of the voxel
            typeof(Rotation),       // rotation of the voxel
            typeof(RenderMesh),     // needed for render purpose
            typeof(LocalToWorld),   // needed for render purpose 
            typeof(RenderBounds),   // needed for render purpose
            typeof(MainColorComponent), // needed for shader overide
            typeof(OutlineColorComponent),  // needed for shader outline overide 
            typeof(OutlineComponent)    // needed to controle the outline color
            );


        // define loaded region
        float maxAmp;
        Nifti.NET.Nifti<float> nifti = DataReader.ParseNifti(out maxAmp, fileName);
        Debug.Log("dimensions : " + nifti.Dimensions[0] + " ; " + nifti.Dimensions[1] + " ; " + nifti.Dimensions[2]);

        int offX, offY, offZ, sizeX, sizeY, sizeZ;
        if (loadMode.ToLower() != "region")
        {
            offX = 0; offY = 0; offZ = 0;
            sizeX = nifti.Dimensions[0];
            sizeY = nifti.Dimensions[1];
            sizeZ = nifti.Dimensions[2];
        }
        else
        {
            offX = region.x;
            offY = region.y;
            offZ = region.z;
            sizeX = region.sizeX;
            sizeY = region.sizeY;
            sizeZ = region.sizeZ;
        }

        // load annotations
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

        float3 startOffset;
        float3 max;

        // load voxels
        if (loadMode.ToLower() != "dummytumor")
        {
            startOffset = GetMillimeters(offX, offY, offZ, nifti);
            max = GetMillimeters(offX + sizeX, offY + sizeY, offZ + sizeZ, nifti);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        float voxelValue = nifti[offX + x, offY + y, offZ + z] / maxAmp;
                        if (voxelValue <= 0) continue; // ignore negative voxel

                        float3 millimetersPos = GetMillimeters(offX + x, offY + y, offZ + z, nifti);

                        CreateVoxel(entityManager, voxelArchetype, offX + x, offY + y, offZ + z, millimetersPos, voxelValue, annotations, startOffset);
                    }
                }
            }

            if (loadStreamlines) GenerateFloatLines(ref entityManager, startOffset, max);
        }
        else
        {
            LoadDummyTumor(nifti, maxAmp, entityManager, voxelArchetype, annotations, out startOffset, out max);

            if (loadStreamlines) GenerateFloatLines(ref entityManager, startOffset);
        }

        SetMap(GetMillimeters(nifti.Dimensions[0], nifti.Dimensions[1], nifti.Dimensions[2], nifti), startOffset, max - startOffset);
    }

    private void CreateVoxel(EntityManager entityManager, EntityArchetype voxelArchetype, int x, int y, int z, float3 millimetersPos, float voxelValue, Dictionary<int3, int4> annotations, float3 startOffset)
    {
        Entity entity = entityManager.CreateEntity(voxelArchetype);

        entityManager.SetComponentData(entity, new Translation { Value = millimetersPos - startOffset });

        Vector4 color = DataReader.ConvertAmplitudeToColor(voxelValue, DataReader.ColorMap.Grey);

        entityManager.SetComponentData(entity, new MainColorComponent { value = color });
        entityManager.SetComponentData(entity, new OutlineColorComponent { value = color });

        float scale = UnityEngine.Random.Range(minSize, maxSize);
        entityManager.SetComponentData(entity, new Scale { Value = scale });

        int3 pos = new int3(x, y, z);
        int4 annot = new int4(-1, -1, -1, -1);
        if (annotations.ContainsKey(pos)) annot = annotations[pos];

        entityManager.SetComponentData(entity, new VoxelComponent
        {
            matrixPosition = pos,
            basePosition = millimetersPos - startOffset,
            baseScale = scale,
            filtered = false,
            value = voxelValue,
            //annotationsIds = new DynamicBuffer<BufferInt>()
            annotationsIds = annot
        });

        entityManager.SetComponentData(entity, new OutlineComponent { isSelected = false, color = new float4(1, 1, 1, 1) });

        int mesh = UnityEngine.Random.Range(0, meshes.Length);
        if (mesh == 0) entityManager.SetComponentData(entity, new Rotation { Value = Quaternion.Euler(-15, 0, 90) });
        else if (mesh == 2) entityManager.SetComponentData(entity, new Rotation { Value = Quaternion.Euler(45, 45, 0) });

        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = meshes[mesh],
            material = voxelMaterial
        });
    }

    private void LoadDummyTumor(Nifti.NET.Nifti<float> nifti, float maxAmp, EntityManager entityManager, EntityArchetype voxelArchetype, Dictionary<int3, int4> annotations, out float3 startOffset, out float3 max)
    {
        float3 center = GetMillimeters(dummyTumorPos.x, dummyTumorPos.y, dummyTumorPos.z, nifti);
        startOffset = center - dummyTumorPeripherySize - dummyTumorRadius;
        max = startOffset + 2 * dummyTumorRadius + 2 * dummyTumorPeripherySize;

        int sizeX = nifti.Dimensions[0];
        int sizeY = nifti.Dimensions[1];
        int sizeZ = nifti.Dimensions[2];

        int n = 0;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float voxelValue = nifti[x, y, z] / maxAmp;
                    if (voxelValue <= 0) continue; // ignore negative voxel

                    float3 millimetersPos = GetMillimeters(x, y, z, nifti);
                    float dist = Length(millimetersPos - center);
                    if (dist < dummyTumorRadius || dist > dummyTumorRadius + dummyTumorPeripherySize) continue;

                    n++;
                    CreateVoxel(entityManager, voxelArchetype, x, y, z, millimetersPos, voxelValue, annotations, startOffset);
                }
            }
        }
        print("created " + n + " voxels in periphery of dummy tumor");

        dummyTumor.position = center - startOffset;
        dummyTumor.localScale = 2 * dummyTumorRadius * Vector3.one;
    }

    // unsed methode that aproximate steamlines to the closest voxel
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

                float lineWidth = 0.05f * line.strength;
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

    // methode that generate line entities based on a file
    private void GenerateFloatLines(ref EntityManager entityManager, float3 startOffet, float3 max)
    {
        EntityArchetype lineArchetype = entityManager.CreateArchetype(
            typeof(LineComponent),
            typeof(LineSegment),
            typeof(LineStyle),
            typeof(MainColorComponent),     // needed since the lines use the same shader asa the voxels
            typeof(OutlineColorComponent)   // same
            );

        List<DataReader.FloatStreamline> streamlines = DataReader.ReadFloatLines(
            streamlineFiles.adjacencyMatrix,
            streamlineFiles.assignments,
            streamlineFiles.nodes,
            streamlineFiles.streamlineTemplate,
            streamlineFiles.digitsNumber,
            streamlineFiles.size,
            streamlineFiles.ignoreAssignmentFirstLine
        );

        foreach (DataReader.FloatStreamline line in streamlines)
        {
            if (line.strength <= 0 || line.line.Count <= 0) continue;

            int n = line.line.Count;

            float3 v = new float3(line.start.x, line.start.y, line.start.z);
            for (int i = 0; i < n; i++)
            {
                float3 dv = new float3(line.line[i].x, line.line[i].y, line.line[i].z);

                if (SegmentIsInCube(v, dv, startOffet, max))
                {
                    Entity lineEntity = entityManager.CreateEntity(lineArchetype);
                    float lineWidth = 0.05f * line.strength;
                    entityManager.SetComponentData(lineEntity, new LineComponent { baseFrom = v - startOffet, baseTo = v + dv - startOffet, filtered = false, baseWidth = lineWidth });
                    entityManager.SetComponentData(lineEntity, new LineSegment(v - startOffet, v + dv - startOffet));
                    entityManager.SetSharedComponentData(lineEntity, new LineStyle { material = lineMaterial });
                    entityManager.SetComponentData(lineEntity, new MainColorComponent { value = new float4(line.mixedColor.r / 255f, line.mixedColor.g / 255f, line.mixedColor.b / 255f, 1) });
                    entityManager.SetComponentData(lineEntity, new OutlineColorComponent { value = new float4(0, 0, 0, 0) });
                }

                v += dv;
            }

        }
    }

    private void GenerateFloatLines(ref EntityManager entityManager, float3 startOffet)
    {
        EntityArchetype lineArchetype = entityManager.CreateArchetype(
            typeof(LineComponent),
            typeof(LineSegment),
            typeof(LineStyle),
            typeof(MainColorComponent),     // needed since the lines use the same shader asa the voxels
            typeof(OutlineColorComponent)   // same
            );

        List<DataReader.FloatStreamline> streamlines = DataReader.ReadFloatLines(
            streamlineFiles.adjacencyMatrix,
            streamlineFiles.assignments,
            streamlineFiles.nodes,
            streamlineFiles.streamlineTemplate,
            streamlineFiles.digitsNumber,
            streamlineFiles.size,
            streamlineFiles.ignoreAssignmentFirstLine
        );

        foreach (DataReader.FloatStreamline line in streamlines)
        {
            if (line.strength <= 0 || line.line.Count <= 0) continue;

            int n = line.line.Count;

            float3 v = new float3(line.start.x, line.start.y, line.start.z);
            for (int i = 0; i < n; i++)
            {
                float3 dv = new float3(line.line[i].x, line.line[i].y, line.line[i].z);

                Entity lineEntity = entityManager.CreateEntity(lineArchetype);
                float lineWidth = 0.05f * line.strength;
                entityManager.SetComponentData(lineEntity, new LineComponent { baseFrom = v - startOffet, baseTo = v + dv - startOffet, filtered = false, baseWidth = lineWidth });
                entityManager.SetComponentData(lineEntity, new LineSegment(v - startOffet, v + dv - startOffet, lineWidth));
                entityManager.SetSharedComponentData(lineEntity, new LineStyle { material = lineMaterial });
                entityManager.SetComponentData(lineEntity, new MainColorComponent { value = new float4(line.mixedColor.r / 255f, line.mixedColor.g / 255f, line.mixedColor.b / 255f, 1) });
                entityManager.SetComponentData(lineEntity, new OutlineColorComponent { value = new float4(0, 0, 0, 0) });

                v += dv;
            }

        }
    }

    bool SegmentIsInCube(float3 v, float3 dv, float3 cubeStart, float3 cubeEnd)
    {
        return (v.x <= Mathf.Max(cubeStart.x, cubeEnd.x) && v.x >= Mathf.Min(cubeStart.x, cubeEnd.x) &&
                v.y <= Mathf.Max(cubeStart.y, cubeEnd.y) && v.y >= Mathf.Min(cubeStart.y, cubeEnd.y) &&
                v.z <= Mathf.Max(cubeStart.z, cubeEnd.z) && v.z >= Mathf.Min(cubeStart.z, cubeEnd.z)) ||
               ((v + dv).x <= Mathf.Max(cubeStart.x, cubeEnd.x) && (v + dv).x >= Mathf.Min(cubeStart.x, cubeEnd.x) &&
                (v + dv).y <= Mathf.Max(cubeStart.y, cubeEnd.y) && (v + dv).y >= Mathf.Min(cubeStart.y, cubeEnd.y) &&
                (v + dv).z <= Mathf.Max(cubeStart.z, cubeEnd.z) && (v + dv).z >= Mathf.Min(cubeStart.z, cubeEnd.z));
    }

    float Length(float3 v)
    {
        if (v.x == 0 && v.y == 0 && v.z == 0) return 0;
        return Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }
}
