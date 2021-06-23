using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGen : MonoBehaviour
{
    public int size = 3;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < size;  x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.AddComponent<MeshRenderer>();
                    sphere.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    sphere.transform.parent = this.transform;
                    sphere.transform.position = new Vector3(x, y, z);
                    float randomScale = Random.Range(-0.3f, 0.3f);
                    sphere.transform.localScale = new Vector3(0.4f + randomScale, 0.4f + randomScale, 0.4f + randomScale);
                }
            }
        } 
    }

    // Update is called once per frame
    void Update(){}
}
