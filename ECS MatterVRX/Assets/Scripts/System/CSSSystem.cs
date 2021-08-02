using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;

public class CSSSystem : ComponentSystem
{
    public static bool applyCSS = false;

    protected override void OnUpdate()
    {
        if (!applyCSS) return;
        applyCSS = false;

        foreach (StylesheetLoader.StyleClass style in StylesheetLoader.styleClasses)
        {
            EntityQuery query = GetEntityQuery(style.QueryDesc);

            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            NativeArray<VoxelComponent> voxels = query.ToComponentDataArray<VoxelComponent>(Allocator.Temp);
            NativeArray<OutlineColorComponent> outlines = query.ToComponentDataArray<OutlineColorComponent>(Allocator.Temp);
            NativeArray<MainColorComponent> colors = query.ToComponentDataArray<MainColorComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                VoxelComponent voxel = voxels[i];
                bool apply = true;
                foreach (var k in style.Conditions)
                {
                    if (k.Key.Contains("mana_lt") && voxel.value >= float.Parse(k.Value, CultureInfo.InvariantCulture)) apply = false;
                    else if (k.Key.Contains("annotated"))
                    {
                        int a = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            if (voxel.annotationsIds[j] >= 0) a++;
                        }
                        if ((k.Value.Contains("true") && a <= 0) || (k.Value.Contains("false") && a > 0)) apply = false;
                    }
                    else if (k.Key.Contains("map"))
                    {
                        string[] colorMapNames = new string[] { "grey", "hot", "cool", "jet" };
                        if (k.Value != colorMapNames[(int)InputManager.colormap]) apply = false;
                    }
                }

                if (!apply) continue;

                foreach (var k in style.Attributes)
                {
                    if (k.Key.Contains("outline-color")) PostUpdateCommands.SetComponent(entities[i],
                        new OutlineColorComponent { value = StylesheetLoader.StyleClass.ParseHexColor(k.Value) }
                    );
                    else if (k.Key.Contains("color")) PostUpdateCommands.SetComponent(entities[i],
                        new MainColorComponent { value = StylesheetLoader.StyleClass.ParseHexColor(k.Value) }
                    );
                }
            }

            entities.Dispose();
            voxels.Dispose();
            outlines.Dispose();
            colors.Dispose();
        }
    }
}
