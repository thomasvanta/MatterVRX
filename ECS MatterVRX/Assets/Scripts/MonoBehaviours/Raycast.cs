using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Valve.VR;

public class Raycast : MonoBehaviour
{
    [SerializeField] private SteamVR_Behaviour_Pose pose;
    [SerializeField] private SteamVR_Input_Sources handType;

    private Entity DoRaycast(float3 from, float3 to)
    {
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

        RaycastInput raycastInput = new RaycastInput
        {
            Start = from,
            End = to,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

        if (collisionWorld.CastRay(raycastInput, out raycastHit))
        {
            Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return hitEntity;
        }
        else
        {
            return Entity.Null;
        }
    }

    private void Update()
    {
        Entity hit = DoRaycast(pose.transform.position, 100f * pose.transform.forward + pose.transform.position);

        if (hit != Entity.Null)
        {
            Debug.Log("hit");
        }
    }
}
