using PionGames.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace PionGames.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class KolizyjnySystem : SystemBase
    {
        private const int DLUGOSC_KOMORKI = 400;
        private EndInitializationEntityCommandBufferSystem _endInitECBSystem;

        protected override void OnCreate()
        {
            _endInitECBSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            UtworzTabele();
            ZrobCos1();

        }


        private void UtworzTabele()
        {
            EntityCommandBuffer entityCommandBuffer = _endInitECBSystem.CreateCommandBuffer();

            Entities
             .ForEach((Entity entity, ref Translation position) =>
             {
                 int poczatekX = (int)(position.Value.x / DLUGOSC_KOMORKI) * DLUGOSC_KOMORKI;
                 int poczatekY = (int)(position.Value.y / DLUGOSC_KOMORKI) * DLUGOSC_KOMORKI;
                 int komorkaID = poczatekX + poczatekY * 100000;

                 KomorkaGrupa komorka = new KomorkaGrupa { komorkaID = komorkaID };
                 entityCommandBuffer.AddSharedComponent<KomorkaGrupa>(entity, komorka);
             })
            .Schedule();
           
            _endInitECBSystem.AddJobHandleForProducer(this.Dependency);
            
        }
        /*public void UtworzTabeleOLD()
        {

            int poczatekX;
            int poczatekY;
            int koniecX;
            int koniecY;
            int ileEntities = 0;

            Entities.WithoutBurst().ForEach((in Entity entity, in Translation position, in Kolizyjny k) =>
            {
                ileEntities++;
                poczatekX = (int)(position.Value.x / DLUGOSC_KOMORKI) * DLUGOSC_KOMORKI;
                poczatekY = (int)(position.Value.y / DLUGOSC_KOMORKI) * DLUGOSC_KOMORKI;
                if (position.Value.x > 0) koniecX = poczatekX + DLUGOSC_KOMORKI; else koniecX = poczatekX - DLUGOSC_KOMORKI;
                if (position.Value.y > 0) koniecY = poczatekY + DLUGOSC_KOMORKI; else koniecY = poczatekY - DLUGOSC_KOMORKI;

                TabelaID tabelaId = new TabelaID { pX = poczatekX, pY = poczatekY, kX = koniecX, kY = koniecY };
                MojEntity idPos = new MojEntity { poz = new float2(position.Value.x, position.Value.y), index = entity.Index, entity = entity, typ = k.typ };
                bool keyExists = tabelaKomorek.ContainsKey(tabelaId);
                if (keyExists)
                {
                    tabelaKomorek[tabelaId].Add(idPos);
                }
                else
                {
                    NativeList<MojEntity> lista = new NativeList<MojEntity>(Allocator.TempJob);
                    lista.Add(idPos);
                    tabelaKomorek.Add(tabelaId, lista);
                }
            }).Run();

        }*/
        private void ZrobCos1()
        {
            Entities
               .ForEach((ref Translation translation, in Kierunek kierunek) =>
               {

               })

               .Schedule();

        }
    }

}