using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenShader : MonoBehaviour
{
    public Transform prefab;

    public int size = 10;

    // parameter to export in config file
    public float minScale = 0.05f;
    public float maxScale = 0.3f;

    void Start()
    {
        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Transform t = Instantiate(prefab);
                    t.SetParent(transform);
                    t.localPosition = new Vector3(x, y, z);
                    t.GetComponent<VisualManager>().SetScale(Random.Range(minScale, maxScale));
                    properties.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                    t.GetComponent<MeshRenderer>().SetPropertyBlock(properties);
                }
            }
        }
    }

    /*
    // Update is called once per frame
    void Update(){}
    */
}
