using UnityEngine;
using Unity.Mathematics;

public class InputUpdater : MonoBehaviour
{
    [SerializeField] private Camera user;
    [SerializeField] private float colliderDist = 10;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.userPos = float3.zero;
        InputManager.colliderDist = colliderDist;
    }

    // Update is called once per frame
    void Update()
    {
        InputManager.userPos = user.transform.position;
    }
}
