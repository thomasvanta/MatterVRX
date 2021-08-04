using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;

public class CSSSystem : ComponentSystem
{
    public static bool applyCSS = true;

    struct ApplyCSSJob: IJobParallelFor
    {
        [ReadOnly] public NativeArray<StylesheetLoader.StyleClass> styles;
        public int classIndex;

        public NativeArray<Entity> entities;
        [ReadOnly] public NativeArray<VoxelComponent> voxels;
        public NativeArray<OutlineComponent> outlines;
        public NativeArray<MainColorComponent> colors;

        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public int styleNumber;

        public void Execute(int index)
        {
            VoxelComponent voxel = voxels[index];
            StylesheetLoader.StyleClass style = styles[classIndex];

            for (int i = 0; i < 4; i++)
            {
                if (style.ConditionNames[i] < 0) break;
                var key = StylesheetLoader.AllConditionNames[style.ConditionNames[i]];
                var value = StylesheetLoader.AllConditionValues[style.ConditionValues[i]];
                if (key.Contains("value_lt") && voxel.value >= float.Parse(value, CultureInfo.InvariantCulture)) return;
                else if (key.Contains("annotated"))
                {
                    int a = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        if (voxel.annotationsIds[j] >= 0) a++;
                    }
                    if ((value.Contains("true") && a <= 0) || (value.Contains("false") && a > 0)) return;
                }
                else if (key.Contains("map"))
                {
                    string[] colorMapNames = new string[] { "grey", "hot", "cool", "jet" };
                    if (value != colorMapNames[(int)InputManager.colormap]) return;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (style.AttributeNames[i] < 0) break;
                var key = StylesheetLoader.AllAttributeNames[style.AttributeNames[i]];
                var value = StylesheetLoader.AllAttributesValues[style.AttributeValues[i]];
                if (key.Contains("outline-color"))
                {
                    var outline = outlines[index];
                    outline.color = StylesheetLoader.ParseHexColor(value);
                    commandBuffer.SetComponent(index, entities[index], outline);
                    
                }
                else if (key.Contains("color"))
                {
                    var color = colors[index];
                    color.value = StylesheetLoader.ParseHexColor(value);
                    commandBuffer.SetComponent(styleNumber, entities[index], color);
                }
            }
        }
    }

    EndSimulationEntityCommandBufferSystem ECB;

    protected override void OnCreate()
    {
        ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        if (!applyCSS) return;
        applyCSS = false;

        var buffer = ECB.CreateCommandBuffer().AsParallelWriter();
        JobHandle jobHandle = default;

        var styles = StylesheetLoader.GetStyleArray();
        int n = styles.Length;

        for (int i = 0; i < n; i++)
        {
            EntityQuery query = GetEntityQuery(styles[i].BuildQueryDesc());

            NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
            NativeArray<VoxelComponent> voxels = query.ToComponentDataArray<VoxelComponent>(Allocator.TempJob);
            NativeArray<OutlineComponent> outlines = query.ToComponentDataArray<OutlineComponent>(Allocator.TempJob);
            NativeArray<MainColorComponent> colors = query.ToComponentDataArray<MainColorComponent>(Allocator.TempJob);

            ApplyCSSJob job = new ApplyCSSJob
            {
                styles = styles,
                classIndex = i,
                entities = entities,
                voxels = voxels,
                outlines = outlines,
                colors = colors,
                commandBuffer = buffer,
                styleNumber = i * 10
            };

            jobHandle = job.Schedule(voxels.Length, 1, jobHandle);
            ECB.AddJobHandleForProducer(jobHandle);
            jobHandle.Complete();

            entities.Dispose();
            voxels.Dispose();
            outlines.Dispose();
            colors.Dispose();
        }

        styles.Dispose();
    }
}
