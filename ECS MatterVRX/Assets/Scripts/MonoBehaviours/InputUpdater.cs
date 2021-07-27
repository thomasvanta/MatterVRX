using UnityEngine;
using Unity.Mathematics;

public class InputUpdater : MonoBehaviour
{
    [SerializeField] private Camera user;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.userPos = float3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        InputManager.userPos = user.transform.position;
    }
}
