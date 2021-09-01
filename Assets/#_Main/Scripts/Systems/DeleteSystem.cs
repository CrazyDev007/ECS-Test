using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(PickupSystem))]
public class DeleteSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        Entities
               .WithAll<DeleteTag>()
               .ForEach((Entity entity) =>
               {
                   commandBuffer.DestroyEntity(entity);
               }).Run();
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;
    }
}
