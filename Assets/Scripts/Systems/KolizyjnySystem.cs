using Piongames;
using PionGames.Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace PionGames.Systems
{
    // [UpdateInGroup(typeof(InitializationSystemGroup))] 
    public class KolizyjnySystem : SystemBase
    {

        private EntityCommandBufferSystem _endInitECBSystem;

        //private BeginPresentationEntityCommandBufferSystem _endSimulationECBSystem;   moge miec kilka systemow w tym samym pkcie/grupie/fazie 
        //Tips and Best Practices
        //Use the existing EntityCommandBufferSystems instead of adding new ones, if possible
        //tu: https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/system_update_order.html
        //dlatego wywalilem BeginPresentationEntityCommandBufferSystem


        private EntityCommandBufferSystem _endSimulationECBSystem;
        private NativeList<Entity> asteroidy2Destroy;
        //[DeallocateOnJobCompletion]
        //Native​Hash​Map<int, NativeArray<Translation>> positionsALL = new NativeHashMap<int, NativeArray<Translation>>();
        //[DeallocateOnJobCompletion]
        Dictionary<int, NativeArray<Translation>> positionsALL = new Dictionary<int, NativeArray<Translation>>();
        //[DeallocateOnJobCompletion]
        //Native​Hash​Map<int, NativeArray<Entity>> entitiesALL = new NativeHashMap<int, NativeArray<Entity>>();
        Dictionary<int, NativeArray<Entity>> entitiesALL = new Dictionary<int, NativeArray<Entity>>();


        NativeMultiHashMap<int, Entity> komorkiAllEntities = new NativeMultiHashMap<int, Entity>(30000, Allocator.Persistent);

        protected override void OnCreate()
        {
            Enabled = false;
            _endInitECBSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            _endSimulationECBSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

            asteroidy2Destroy = new NativeList<Entity>(Allocator.Persistent);
            //entityCommandBuffer = new EntityCommandBuffer(Allocator.Persistent);
            //UtworzTabeleHashMap(komorkiAllEntities2);

        }

        public void UtworzTabeleHashMap()
        {
            
            NativeMultiHashMap<int, Entity>.ParallelWriter komorkiAll = komorkiAllEntities.AsParallelWriter();

            Entities.ForEach((in Entity entity, in KomorkaID komorkaID) =>
            {
                komorkiAll.Add(komorkaID.Value, entity);

            })
            .ScheduleParallel();

            UpdateHashMap();

        }
        public void UpdateHashMap()
        {
            NativeArray<int> listaKluczy = komorkiAllEntities.GetKeyArray(Allocator.Temp);

            Debug.Log("listaKluczy.Length"); 




        }
        private void UtworzTabeleHashMapParallel()
        {
            /* NativeHashMap<int, NativeList<Entity>>.ParallelWriter komorkiAllEntitiesParallel = komorkiAllEntities.AsParallelWriter();

             Entities
               .ForEach((Entity entity, int entityInQueryIndex, in KomorkaID komorkaID) =>
               {
                   //jesli jest juz taka tabela
                   if (komorkiAllEntitiesParallel.TryAdd(komorkaID.Value, new NativeList<Entity>()))
                   {
                       komorkiAllEntitiesParallel[komorkaID.Value].Add(entity);
                   }
                   else
                   {
                       //komorkiAllEntities[komorkaID.Value] = new NativeList<Entity>();
                       komorkiAllEntitiesParallel[komorkaID.Value].Add(entity);
                   }

               })
              .ScheduleParallel();
 */

        }
        protected override void OnUpdate()
        {
            Debug.Log("onupdate kolizyjny");
            /*
            // Debug.Log("--------------------");
            // Debug.Log("OnUpdate");
            asteroidy2Destroy.Clear();
            //eCB = new EntityCommandBuffer(Allocator.Temp);
            UtworzTabele();
            // SprawdzZderzenia2();
            //UsunAsteroidyPoZderzeniu();
            SprZdarzenia();*/
        }

        private void SprZdarzenia()
        {
            //tu by sie przydalo miec mozliwosc tworzenia tabel z entities by component entitiesData


        }

        private void UtworzTabele()
        {


            //w oparciu o https://www.youtube.com/watch?v=hP4Vu6JbzSo&list=PLzDRvYVwl53s40yP5RQXitbT--IRcHqba&index=19
            //czyli z NativeHashMap concurrent ect 
            EntityCommandBuffer.ParallelWriter entityCommandBuffer = _endInitECBSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
             .ForEach((Entity entity, int entityInQueryIndex, ref Translation position, in KomorkaID komorkaID) =>
             {
                 int poczatekX = (int)math.floor(position.Value.x / Settings.DLUGOSC_KOMORKI);
                 int poczatekY = (int)math.floor(position.Value.y / Settings.DLUGOSC_KOMORKI);
                 int nowaKomorkaID = poczatekX + poczatekY * 100000;

                 //tylko jesli jest zmiana
                 if (nowaKomorkaID != komorkaID.Value)
                 {
                     entityCommandBuffer.SetComponent(entityInQueryIndex, entity, new KomorkaID { Value = nowaKomorkaID });
                 }



             })
            .ScheduleParallel();

            _endInitECBSystem.AddJobHandleForProducer(this.Dependency);

        }
        private void SprawdzZderzenia2()
        {
            List<KomorkaGrupa> komorki = new List<KomorkaGrupa>();
            EntityManager.GetAllUniqueSharedComponentData<KomorkaGrupa>(komorki);
            EntityQuery queryGrupa = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<KomorkaGrupa>());
            // NativeArray<Entity> entities;
            // NativeArray<Translation> positions;

            //NativeArray<EntityData> entitiesData;
            NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.TempJob);

            List<ZderzeniaJOB> ZderzeniaJOBy = new List<ZderzeniaJOB>();

            //Debug.Log(" komorki ile grup " + komorki.Count);
            foreach (KomorkaGrupa komorkaJedna in komorki)
            {
                // Debug.Log("aaa " + komorkaJedna.komorkaID);
                queryGrupa.ResetFilter();
                queryGrupa.SetSharedComponentFilter(new KomorkaGrupa { komorkaID = komorkaJedna.komorkaID });
                // entities = queryGrupa.ToEntityArray(Allocator.TempJob);

                //positions = queryGrupa.ToComponentDataArray<Translation>(Allocator.TempJob);

                entitiesALL[komorkaJedna.komorkaID] = queryGrupa.ToEntityArray(Allocator.TempJob);
                positionsALL[komorkaJedna.komorkaID] = queryGrupa.ToComponentDataArray<Translation>(Allocator.TempJob);
                // Debug.Log("ile entities w grupie " + entitiesALL[komorkaJedna.komorkaID].Length);

                /*entitiesData = new NativeArray<EntityData>(positions.Length, Allocator.TempJob);
                for (int i = 0; i < entitiesData.Length; i++)
                {
                    entitiesData[i] = new EntityData { position = positions[i].Value, entity = entities[i], index = entities[i].Index };

                }
                var zderzeniaJob = new ZderzeniaJOB { positions = entitiesData, e2D = new NativeList<Entity>(Allocator.TempJob), e2R = new NativeList<Entity>(Allocator.TempJob) };
               */
                var zderzeniaJob = new ZderzeniaJOB { positions = positionsALL[komorkaJedna.komorkaID], entities = entitiesALL[komorkaJedna.komorkaID], e2D = new NativeList<Entity>(Allocator.TempJob), e2R = new NativeList<Entity>(Allocator.TempJob) };

                ZderzeniaJOBy.Add(zderzeniaJob);
                JobHandle jh = zderzeniaJob.Schedule();
                jobHandles.Add(jh);
                /* positions.Dispose();
                 entitiesData.Dispose();
                 entities.Dispose();*/


            }
            // positionsALL.Dispose();
            //entitiesALL.Dispose();
            JobHandle.CompleteAll(jobHandles);


            foreach (var job in ZderzeniaJOBy)
            {
                //entity2Destroy.AddRange(job.e2D);
                //entity2Relocate.AddRange(job.e2R);

                asteroidy2Destroy.AddRange(job.e2D);


                job.e2D.Dispose();
                job.e2R.Dispose();
                job.positions.Dispose();
                job.entities.Dispose();


            }
            /*foreach (KomorkaGrupa komorkaJedna in komorki)
            {
                positionsALL[komorkaJedna.komorkaID].Dispose();
            }*/

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

                    if (i == j) continue;
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
            komorkiAllEntities2.Dispose();
        }
    }

}