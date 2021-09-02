using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Collections;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.Physics.Systems;
using Unity.Burst;

[AlwaysSynchronizeSystem]
public class PickupSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem bs;

    private BuildPhysicsWorld bpw;
    private StepPhysicsWorld spw;

    protected override void OnCreate()
    {
        bs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        bpw = World.GetOrCreateSystem<BuildPhysicsWorld>();
        spw = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ECB = bs.CreateCommandBuffer();


        TriggerJob triggerJob = new TriggerJob();
        triggerJob.speedEntities = GetComponentDataFromEntity<PlayerSpeedData>(true);
        triggerJob.entitiesToDelete = GetComponentDataFromEntity<DeleteTag>(true);
        triggerJob.commandBuffer = ECB;

        //
        JobHandle jobHandle = triggerJob.Schedule(spw.Simulation, ref bpw.PhysicsWorld, inputDeps);
        //jobHandle.Complete();
        bs.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }


    private struct TriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PlayerSpeedData> speedEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public EntityCommandBuffer commandBuffer;

        public void Execute(Unity.Physics.TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            Debug.Log("aaaaaaaaaaa");
            /*if (entitiesToDelete.HasComponent(entityA) && entitiesToDelete.HasComponent(entityB))
            {
                return;
            }

            if (entitiesToDelete.HasComponent(entityA) && speedEntities.HasComponent(entityB))
            {
                Debug.Log("asdf");
            }
            else if (speedEntities.HasComponent(entityA) && entitiesToDelete.HasComponent(entityB))
            {
                Debug.Log("aassddff");
            }*/

            TestEntityTrigger(entityA, entityB);
            TestEntityTrigger(entityB, entityA);
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
                commandBuffer.DestroyEntity(entity2);
            }
        }
    }
}
