using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StylesheetLoader : MonoBehaviour
{
    public class StyleClass
    {
        public string Name;
        public SortedList<string, string> Attributes;
        public SortedList<string, string> Conditions;

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
    }

    void Start()
    {
        foreach (StyleClass s in StyleClass.FillClasses())
        {
            Debug.Log(s.ToString());
        }
    }
}
