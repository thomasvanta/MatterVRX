using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIFillInfo : MonoBehaviour
{
    public struct Voxel
    {
        public float scale;
        public Color color;
    }

    public void Init(Vector3 pos, Voxel v)
    {
        TMP_Text title = transform.Find("Title").GetComponent<TMP_Text>();
        TMP_Text info = transform.Find("Info").GetComponent<TMP_Text>();

        title.text = pos.ToString();

        info.text = "scale : " + v.scale.ToString() + "\n" +
                    "color : " + v.color.ToString() + "\n";
    }
}
