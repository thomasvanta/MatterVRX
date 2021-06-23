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
                    sphere.transform.parent = this.transform;
                    sphere.transform.position = new Vector3(x, y, z);
                    sphere.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }
        } 
    }

    // Update is called once per frame
    void Update(){}
}
