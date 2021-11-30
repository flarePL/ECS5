using PionGames.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace PionGames.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))] 
    public class KolizyjnySystem : SystemBase
    {
        private const int DLUGOSC_KOMORKI = 10;
        private EndInitializationEntityCommandBufferSystem _endInitECBSystem;

        private BeginPresentationEntityCommandBufferSystem _endSimulationECBSystem;
        private NativeList<Entity> asteroidy2Destroy;
       
        protected override void OnCreate()
        {
            _endInitECBSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            _endSimulationECBSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
            
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
            SprawdzZderzenia();
            UsunAsteroidyPoZderzeniu();
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