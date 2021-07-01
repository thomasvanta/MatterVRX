using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGen : MonoBehaviour
{
    public int size = 3;
    public GameObject spherePrefab;
    public Camera cam;

    // parameter to export in config file
    public float minScale = 0.05f;
    public float maxScale = 0.3f;

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

                    Billboard b = sphere.GetComponent<Billboard>();
                    if (b != null) b.cam = cam;

                    sphere.GetComponent<VisualManager>().SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                    sphere.transform.parent = this.transform;
                    sphere.transform.position = new Vector3(x, y, z);
                    sphere.GetComponent<VisualManager>().SetScale(Random.Range(minScale, maxScale));
                }
            }
        } 
    }

    /*
    // Update is called once per frame
    void Update() {}
    */
}
