using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.Entities;
using Unity.Mathematics;

public class StylesheetLoader : MonoBehaviour
{
    public static StyleClass[] styleClasses;

    private static Dictionary<string, ComponentType> cssCorrespondance;

    public class StyleClass
    {
        public string Name;
        public SortedList<string, string> Attributes;
        public SortedList<string, string> Conditions;

        public EntityQueryDesc QueryDesc;

        private static char[] HexVals = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static List<StyleClass> FillClasses(string fileName = "stylesheet.css")
        {
            string full = File.ReadAllText(Path.Combine(Application.dataPath, "Configuration/" + fileName));
            var formattedClasses = full.Split('}');
            List<StyleClass> classes = new List<StyleClass>();

            for (int i = 0; i < formattedClasses.Length; i++)
            {
                var c = FillClass(formattedClasses[i]);
                if (c != null) classes.Add(c);
            }

            return classes;
        }

        public static StyleClass FillClass(string formattedString)
        {
            var halfSplit = formattedString.Split('{');
            if (halfSplit.Length <= 1) return null;

            StyleClass sclass = new StyleClass();
            sclass.Name = halfSplit[0].Trim();
            sclass.Attributes = new SortedList<string, string>();
            sclass.Conditions = new SortedList<string, string>();

            var names = sclass.Name.Split(' ');
            for (int i = 0; i < names.Length; i++)
            {
                var cond = names[i].Split(':');
                if (cond.Length > 1)
                {
                    sclass.Conditions.Add(cond[0], cond[1]);
                }
            }

            var attr = halfSplit[1].Trim().Split('\n');
            for (int i = 0; i < attr.Length; i++)
            {
                var split = attr[i].Trim(';').Trim().Split(':');
                if (split.Length <= 1) continue;
                split[1].Trim();
                sclass.Attributes.Add(split[0], split[1]);
            }

            sclass.BuildQueryDesc();
            return sclass;
        }

        public override string ToString()
        {
            string txt = Name + " class :\n";
            
            foreach (var c in Conditions)
            {
                txt += "if " + c.Key + " " + c.Value + "\n";
            }

            foreach (var c in Attributes)
            {
                txt += c.Key + ": " + c.Value + "\n";
            }

            return txt;
        }

        public void BuildQueryDesc()
        {
            List<ComponentType> all = new List<ComponentType>
            {
                ComponentType.ReadOnly<VoxelComponent>(),
                ComponentType.ReadWrite<MainColorComponent>(),
                ComponentType.ReadWrite<OutlineColorComponent>()
            };

            foreach (var k in cssCorrespondance)
            {
                if (Name.Contains(k.Key)) all.Add(k.Value);
            }

            QueryDesc = new EntityQueryDesc
            {
                Options = EntityQueryOptions.IncludeDisabled,
                All = all.ToArray()
            };
        }

        public static float4 ParseHexColor(string colorString)
        {
            string s = colorString.Trim('#').Trim();
            float r = 16 * Array.IndexOf(HexVals, s[0]) + Array.IndexOf(HexVals, s[1]) / 255f;
            float g = 16 * Array.IndexOf(HexVals, s[2]) + Array.IndexOf(HexVals, s[3]) / 255f;
            float b = 16 * Array.IndexOf(HexVals, s[4]) + Array.IndexOf(HexVals, s[5]) / 255f;
            return new float4(r, g, b, 1f);
        }
    }

    void Start()
    {
        cssCorrespondance = new Dictionary<string, ComponentType>
        {
            { "selected", typeof(SelectedFlag) },
        };
        styleClasses = StyleClass.FillClasses().ToArray();
    }
}
