using PionGames.Components;
using PionGames.Structs;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngineTime = UnityEngine.Time;

namespace PionGames.Systems
{
   // [UpdateInGroup(typeof(InitializationSystemGroup))] 
    public class KolizyjnySystem : SystemBase
    {
        private const int DLUGOSC_KOMORKI = 10;
        private EntityCommandBufferSystem _endInitECBSystem;

        //private BeginPresentationEntityCommandBufferSystem _endSimulationECBSystem;   moge miec kilka systemow w tym samym pkcie/grupie/fazie 
        //Tips and Best Practices
        //Use the existing EntityCommandBufferSystems instead of adding new ones, if possible
        //tu: https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/system_update_order.html
        //dlatego wywalilem BeginPresentationEntityCommandBufferSystem


        private EntityCommandBufferSystem _endSimulationECBSystem;
        private NativeList<Entity> asteroidy2Destroy;
       
        protected override void OnCreate()
        {
            _endInitECBSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            _endSimulationECBSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            
            asteroidy2Destroy = new NativeList<Entity>(Allocator.Persistent);
            //entityCommandBuffer = new EntityCommandBuffer(Allocator.Persistent);
        }
        protected override void OnUpdate()
        {
           // Debug.Log("--------------------");
           // Debug.Log("OnUpdate");
            asteroidy2Destroy.Clear();
            //eCB = new EntityCommandBuffer(Allocator.Temp);
            UtworzTabele();
            SprawdzZderzenia2();
            //UsunAsteroidyPoZderzeniu();

        }


        private void UtworzTabele()
        {
           
           
            //Debug.Log("UtworzTabele");

            EntityCommandBuffer.ParallelWriter entityCommandBuffer = _endInitECBSystem.CreateCommandBuffer().AsParallelWriter(); 
           
            Entities
             .ForEach((Entity entity, int entityInQueryIndex, ref Translation position) =>
             {
                 int poczatekX = (int)(position.Value.x / DLUGOSC_KOMORKI);// * DLUGOSC_KOMORKI;
                 int poczatekY = (int)(position.Value.y / DLUGOSC_KOMORKI);// * DLUGOSC_KOMORKI;
                 int komorkaID = poczatekX + poczatekY * 100000;
                 KomorkaGrupa komorka = new KomorkaGrupa { komorkaID = komorkaID };
                 entityCommandBuffer.AddSharedComponent<KomorkaGrupa>(entityInQueryIndex, entity, komorka);
                 
             })
            .ScheduleParallel();

            _endInitECBSystem.AddJobHandleForProducer(this.Dependency);

        }
        private void SprawdzZderzenia2()
        {
            List<KomorkaGrupa> komorki = new List<KomorkaGrupa>();
            EntityManager.GetAllUniqueSharedComponentData<KomorkaGrupa>(komorki);
            EntityQuery queryGrupa = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<KomorkaGrupa>());
            NativeArray<Entity> entities;
            NativeArray<Translation> positions;
           
            NativeArray<EntityData> entitiesData;
            NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.TempJob);
          
            List<ZderzeniaJOB> ZderzeniaJOBy = new List<ZderzeniaJOB>();


            //memory leak 
            foreach (KomorkaGrupa komorkaJedna in komorki)
            {
               // Debug.Log("aaa " + komorkaJedna.komorkaID);
                queryGrupa.ResetFilter();
                queryGrupa.SetSharedComponentFilter(new KomorkaGrupa { komorkaID = komorkaJedna.komorkaID });
                entities = queryGrupa.ToEntityArray(Allocator.TempJob);
                positions = queryGrupa.ToComponentDataArray<Translation>(Allocator.TempJob);
                entitiesData = new NativeArray<EntityData>(positions.Length, Allocator.TempJob);
                for (int i = 0; i < entitiesData.Length; i++)
                {
                    entitiesData[i] = new EntityData { position = positions[i].Value, entity = entities[i], index = entities[i].Index };

                }
                var zderzeniaJob = new ZderzeniaJOB { positions = entitiesData, e2D = new NativeList<Entity>(Allocator.TempJob), e2R = new NativeList<Entity>(Allocator.TempJob) };

                ZderzeniaJOBy.Add(zderzeniaJob);
                JobHandle jh = zderzeniaJob.Schedule();
                jobHandles.Add(jh);
               /* positions.Dispose();
                entitiesData.Dispose();
                entities.Dispose();*/


            }
            JobHandle.CompleteAll(jobHandles);

           
            foreach (var job in ZderzeniaJOBy)
            {
                //entity2Destroy.AddRange(job.e2D);
               //entity2Relocate.AddRange(job.e2R);

                job.e2D.Dispose();
                job.e2R.Dispose();
                job.positions.Dispose();
                

            }
           
            jobHandles.Dispose();
           
        }
        private void SprawdzZderzenia()
        {
            //Debug.Log("SprawdzZderzenia");
            float squreRadius = 1f;
            EntityQuery m_Group = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<KomorkaGrupa>());


            // tu skonczylem
            //jak zrobic aby bylo po wszystkich chunkach
            //moze to >>?? https://forum.unity.com/threads/sharedcomponent-burst-compile.564154/
            m_Group.SetSharedComponentFilter(new KomorkaGrupa { komorkaID = 0 });
            
            NativeArray<Entity> entities = m_Group.ToEntityArray(Allocator.Temp);
            NativeArray<Translation> positions = m_Group.ToComponentDataArray<Translation>(Allocator.Temp);
           
            for (int i = 0; i < positions.Length; i++)
            {
                for (int j = 0; j < positions.Length; j++)
                {

                    if(i==j)continue;
                     if (CheckCollision(positions[i].Value, positions[j].Value, squreRadius))
                     {
                         asteroidy2Destroy.Add(entities[i]);
                         asteroidy2Destroy.Add(entities[j]);
                        /* Debug.Log($"zderzenie {i}, {j}");
                         Debug.Log("zderzenie "+ positions[i].Value.x+" "+ positions[j].Value.x);
                         Debug.Log("zderzenie " + positions[i].Value.y + " " + positions[j].Value.y);*/

                     }
                   

                   
                }

            }



        }
        private void UsunAsteroidyPoZderzeniu()
        {
            EntityCommandBuffer entityCommandBuffer2 = _endSimulationECBSystem.CreateCommandBuffer(); ;
            //Debug.Log("UsunAsteroidyPoZderzeniu");
            foreach (var asteroida in asteroidy2Destroy)
            {
                entityCommandBuffer2.DestroyEntity(asteroida);
            }
            //eCB.Dispose();

        }
        private bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
        {
            float3 delta = posA - posB;
            float distanceSquare = delta.x * delta.x + delta.y * delta.y;

            return distanceSquare <= radiusSqr;
        }
        protected override void OnDestroy()
        {
            asteroidy2Destroy.Dispose();
        }
    }

}