using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Valve.VR;

public class Raycast : MonoBehaviour
{
    [SerializeField] private SteamVR_Behaviour_Pose pose;
    [SerializeField] private SteamVR_Input_Sources handType;
    [SerializeField] private GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;

    private Entity lastEntity = Entity.Null;
    private EntityManager entityManager;
    private Entity DoRaycast(float3 from, float3 to, out float3 hitPos)
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
            hitPos = raycastHit.Position;
            Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return hitEntity;
        }
        else
        {
            hitPos = float3.zero;
            return Entity.Null;
        }
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    private void Update()
    {
        float3 hitPos;
        Entity hit = DoRaycast(pose.transform.position, 100f * pose.transform.forward + pose.transform.position, out hitPos);

        if (hit == Entity.Null && lastEntity != Entity.Null)
        {
            entityManager.SetComponentData(lastEntity, new OutlineComponent { isSelected = false });
            lastEntity = Entity.Null;

            laser.SetActive(false);
        }
        else if (hit != Entity.Null)
        {
            if (lastEntity != Entity.Null)
            {
                entityManager.SetComponentData(lastEntity, new OutlineComponent { isSelected = false });
            }
            entityManager.SetComponentData(hit, new OutlineComponent { isSelected = true, color = new float4(1, 1, 1, 1) });
            lastEntity = hit;

            ShowLaser(hitPos);
        }
    }

    private void ShowLaser(float3 hitPos)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(pose.transform.position, hitPos, .5f);
        laserTransform.LookAt(hitPos);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, math.distance(hitPos, pose.transform.position));
    }
}
