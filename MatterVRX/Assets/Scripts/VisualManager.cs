using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
    
    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
