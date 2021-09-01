using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float2 deltaSpeed = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Entities.ForEach((ref PhysicsVelocity vel, in PlayerSpeedData speedData) =>
         {
             float2 newVel = vel.Linear.xz;
             newVel += deltaSpeed * deltaTime * speedData.speed;
             //
             vel.Linear.xz = newVel;
         }).Run();

        return default;
    }
}
