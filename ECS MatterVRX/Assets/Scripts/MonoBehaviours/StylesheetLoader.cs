using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public class StylesheetLoader : MonoBehaviour
{
    private static StyleClass[] styleClasses;

    private static Dictionary<string, ComponentType> cssCorrespondance;
    private static char[] HexVals = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

    [ReadOnly] public static List<string> AllStyleNames = new List<string>();
    [ReadOnly] public static List<string> AllAttributeNames = new List<string>();
    [ReadOnly] public static List<string> AllAttributesValues = new List<string>();
    [ReadOnly] public static List<string> AllConditionNames = new List<string>();
    [ReadOnly] public static List<string> AllConditionValues = new List<string>();

    public struct StyleClass
    {
        public int Name;
        public int4 AttributeNames;
        public int4 AttributeValues;
        public int4 ConditionNames;
        public int4 ConditionValues;

        public EntityQueryDesc BuildQueryDesc()
        {
            List<ComponentType> all = new List<ComponentType>
            {
                ComponentType.ReadOnly<VoxelComponent>(),
                ComponentType.ReadWrite<MainColorComponent>(),
                ComponentType.ReadWrite<OutlineComponent>()
            };

            foreach (var k in cssCorrespondance)
            {
                if (AllStyleNames[Name].Contains(k.Key)) all.Add(k.Value);
            }

            return new EntityQueryDesc
            {
                Options = EntityQueryOptions.IncludeDisabled,
                All = all.ToArray()
            };
        }
    }

    void Start()
    {
        cssCorrespondance = new Dictionary<string, ComponentType>
        {
            { "selected", typeof(SelectedFlag) },
        };
        styleClasses = FillClasses().ToArray();
    }

    public static NativeArray<StyleClass> GetStyleArray()
    {
        NativeArray<StyleClass> res = new NativeArray<StyleClass>(styleClasses, Allocator.TempJob);
        return res;
    }

    private List<StyleClass> FillClasses(string fileName = "stylesheet.css")
    {
        string full = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Configuration/" + fileName));
        var formattedClasses = full.Split('}');
        List<StyleClass> classes = new List<StyleClass>();

        for (int i = 0; i < formattedClasses.Length; i++)
        {
            var c = FillClass(formattedClasses[i]);
            if (c.Name >= 0) classes.Add(c);
        }

        return classes;
    }

    private StyleClass FillClass(string formattedString)
    {
        var halfSplit = formattedString.Split('{');
        if (halfSplit.Length <= 1) return new StyleClass { Name = -1 };

        StyleClass sclass = new StyleClass();
        sclass.Name = AllStyleNames.Count;
        AllStyleNames.Add(halfSplit[0].Trim());

        sclass.AttributeNames = new int4(-1, -1, -1, -1);
        sclass.AttributeValues = new int4(-1, -1, -1, -1);
        sclass.ConditionNames = new int4(-1, -1, -1, -1);
        sclass.ConditionValues = new int4(-1, -1, -1, -1);

        var names = AllStyleNames[sclass.Name].Split(' ');
        for (int i = 0; i < names.Length && i < 4; i++)
        {
            var cond = names[i].Split(':');
            if (cond.Length > 1)
            {
                int acn = AllConditionNames.IndexOf(cond[0]);
                int acv = AllConditionValues.IndexOf(cond[1]);
                if (acn < 0)
                {
                    acn = AllConditionNames.Count;
                    AllConditionNames.Add(cond[0]);
                }
                if (acv < 0)
                {
                    acv = AllConditionValues.Count;
                    AllConditionValues.Add(cond[1]);
                }
                sclass.ConditionNames[i] = acn;
                sclass.ConditionValues[i] = acv;
            }
        }

        var attr = halfSplit[1].Trim().Split('\n');
        for (int i = 0; i < attr.Length; i++)
        {
            var split = attr[i].Trim(';').Trim().Split(':');
            if (split.Length <= 1) continue;

            int aan = AllAttributeNames.IndexOf(split[0]);
            int aav = AllAttributesValues.IndexOf(split[1]);
            if (aan < 0)
            {
                aan = AllAttributeNames.Count;
                AllAttributeNames.Add(split[0]);
            }
            if (aav < 0)
            {
                aav = AllAttributesValues.Count;
                AllAttributesValues.Add(split[1]);
            }
            sclass.AttributeNames[i] = aan;
            sclass.AttributeValues[i] = aav;
        }

        sclass.BuildQueryDesc();
        return sclass;
    }

    public static float4 ParseHexColor(string colorString)
    {
        string s = colorString.Trim().Trim('#');
        float r = (16f * Array.IndexOf(HexVals, s[0]) + Array.IndexOf(HexVals, s[1])) / 255f;
        float g = (16f * Array.IndexOf(HexVals, s[2]) + Array.IndexOf(HexVals, s[3])) / 255f;
        float b = (16f * Array.IndexOf(HexVals, s[4]) + Array.IndexOf(HexVals, s[5])) / 255f;
        return new float4(r, g, b, 1f);
    }
}
