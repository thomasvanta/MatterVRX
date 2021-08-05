using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MinimapDot : MonoBehaviour
{
    [SerializeField] private Vector3 dotScale;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 minimapScale;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = dotScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3((-InputManager.globalPosition.x + offset.x) * minimapScale.x,
                -(-InputManager.globalPosition.z + offset.z) * minimapScale.z,
                (-InputManager.globalPosition.y + offset.y) * minimapScale.y);
        transform.localPosition = newPos;
    }
}