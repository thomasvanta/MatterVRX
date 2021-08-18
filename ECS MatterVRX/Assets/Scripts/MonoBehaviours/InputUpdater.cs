using UnityEngine;
using Unity.Mathematics;

/* Uptade the position of the user head in the input manager
 */
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
