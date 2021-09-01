using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Collections;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.Physics.Systems;

public class PickupSystem : JobComponentSystem
{
    private BeginInitializationEntityCommandBufferSystem bs;

    private BuildPhysicsWorld bpw;
    private StepPhysicsWorld spw;

    protected override void OnCreate()
    {
        bs = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        bpw = World.GetOrCreateSystem<BuildPhysicsWorld>();
        spw = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ECB = bs.CreateCommandBuffer();


        TriggerJob triggerJob = new TriggerJob
        {
            speedEntities = GetComponentDataFromEntity<PlayerSpeedData>(),
            entitiesToDelete = GetComponentDataFromEntity<DeleteTag>(),
            commandBuffer = ECB
        };
        //
        inputDeps = triggerJob.Schedule(spw.Simulation, ref bpw.PhysicsWorld, inputDeps);
        bs.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }

    private struct TriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<PlayerSpeedData> speedEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public EntityCommandBuffer commandBuffer;

        public void Execute(Unity.Physics.TriggerEvent triggerEvent)
        {
            TestEntityTrigger(triggerEvent.EntityA, triggerEvent.EntityB);
            TestEntityTrigger(triggerEvent.EntityB, triggerEvent.EntityA);
        }

        private void TestEntityTrigger(Entity entity1, Entity entity2)
        {
            if (speedEntities.HasComponent(entity1))
            {
                if (entitiesToDelete.HasComponent(entity2))
                {
                    return;
                    //DeleteTag deleteTag = entitiesToDelete[triggerev]
                }
                commandBuffer.AddComponent(entity2, new DeleteTag());
            }
        }
    }
}
