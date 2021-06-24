using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGen : MonoBehaviour
{
    public int size = 3;
    public GameObject spherePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < size;  x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    GameObject sphere = Instantiate(spherePrefab);
                    sphere.GetComponent<VisualManager>().SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                    sphere.transform.parent = this.transform;
                    sphere.transform.position = new Vector3(x, y, z);
                    sphere.GetComponent<VisualManager>().SetScale(Random.Range(0.05f, 0.5f));
                }
            }
        } 
    }

    // Update is called once per frame
    void Update(){}
}
