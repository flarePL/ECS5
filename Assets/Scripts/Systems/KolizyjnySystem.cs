using PionGames.Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace PionGames.Systems
{
    public class KolizyjnySystem : SystemBase
    {
        private const int DLUGOSC_KOMORKI = 50;
        private EntityCommandBufferSystem _endInitECBSystem;

        //private BeginPresentationEntityCommandBufferSystem _endSimulationECBSystem;   moge miec kilka systemow w tym samym pkcie/grupie/fazie 
        //Tips and Best Practices
        //Use the existing EntityCommandBufferSystems instead of adding new ones, if possible
        //tu: https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/system_update_order.html
        //dlatego wywalilem BeginPresentationEntityCommandBufferSystem


        private EntityCommandBufferSystem _endInit_2_ECBSystem;
        private NativeList<Entity> asteroidy2Destroy;


        //[DeallocateOnJobCompletion]
        Dictionary<int, NativeArray<Translation>> positionsALL = new Dictionary<int, NativeArray<Translation>>();

        Dictionary<int, NativeArray<Entity>> entitiesALL = new Dictionary<int, NativeArray<Entity>>();
        Dictionary<int, NativeList<Entity>> e2DbezParallel = new Dictionary<int, NativeList<Entity>>();
        protected override void OnCreate()
        {
            _endInitECBSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            _endInit_2_ECBSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

            asteroidy2Destroy = new NativeList<Entity>(Allocator.Persistent);

        }
        protected override void OnUpdate()
        {
            asteroidy2Destroy.Clear();

            UtworzTabele();
            SprawdzZderzenia2();

        }


        private void UtworzTabele()
        {

            EntityCommandBuffer.ParallelWriter entityCommandBuffer = _endInitECBSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
             .ForEach((Entity entity, int entityInQueryIndex, ref Translation position) =>
             {
                 int poczatekX = (int)math.floor(position.Value.x / DLUGOSC_KOMORKI);
                 int poczatekY = (int)math.floor(position.Value.y / DLUGOSC_KOMORKI);
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

            NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp); 
            List<ZderzeniaJOB> ZderzeniaJOBy = new List<ZderzeniaJOB>();


            foreach (KomorkaGrupa komorkaJedna in komorki)
            {
                queryGrupa.ResetFilter();
                queryGrupa.SetSharedComponentFilter(new KomorkaGrupa { komorkaID = komorkaJedna.komorkaID });

                entitiesALL[komorkaJedna.komorkaID] = queryGrupa.ToEntityArray(Allocator.TempJob);
                positionsALL[komorkaJedna.komorkaID] = queryGrupa.ToComponentDataArray<Translation>(Allocator.TempJob);
                e2DbezParallel[komorkaJedna.komorkaID] = new NativeList<Entity>(64, Allocator.TempJob);   //64 - poczatkowa wielkosc

                var zderzeniaJob = new ZderzeniaJOB { positions = positionsALL[komorkaJedna.komorkaID], entities = entitiesALL[komorkaJedna.komorkaID], e2D = e2DbezParallel[komorkaJedna.komorkaID].AsParallelWriter() };

                ZderzeniaJOBy.Add(zderzeniaJob);
                JobHandle jh = zderzeniaJob.Schedule(positionsALL[komorkaJedna.komorkaID].Length, 16);  //16 - na ile jobow dodatkowo dzielic tego joba
                jobHandles.Add(jh);


            }

            JobHandle.CompleteAll(jobHandles);

            EntityCommandBuffer entityCommandBuffer2 = _endInit_2_ECBSystem.CreateCommandBuffer(); ;

            foreach (KomorkaGrupa komorkaJedna in komorki)
            {

                foreach (var asteroida in e2DbezParallel[komorkaJedna.komorkaID])
                {
                    entityCommandBuffer2.DestroyEntity(asteroida);
                }

                e2DbezParallel[komorkaJedna.komorkaID].Dispose();


            }
            foreach (var job in ZderzeniaJOBy)
            {

            }


            jobHandles.Dispose();

        }


    }

}