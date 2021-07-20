using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class VoxelInfo : MonoBehaviour
{
    private Text title;
    private Text color;
    private Text value;

    // Start is called before the first frame update
    void Start()
    {
        title = transform.Find("Title").GetComponent<Text>();
        color = transform.Find("Color").GetComponent<Text>();
        value = transform.Find("Value").GetComponent<Text>();
    }

    public void FillInfo(Vector3 pos, float4 c, float val)
    {
        title.text = "Voxel at " + pos.ToString();
        color.text = "Color : " + (255 * c).ToString();
        value.text = "Value : " + val.ToString();
    }
}
