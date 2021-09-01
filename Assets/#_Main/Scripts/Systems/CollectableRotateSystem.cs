using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[AlwaysSynchronizeSystem]
public class CollectableRotateSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in CollectableData collectable) =>
         {
             rotation.Value = math.mul(rotation.Value, quaternion.EulerXYZ(collectable.rotateSpeed * deltaTime));
         }).Run();
        return default;
    }
}
