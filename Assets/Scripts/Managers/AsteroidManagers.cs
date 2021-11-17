using PionGames.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PionGames.Managers
{


    public class AsteroidsManager
    {
        private EntityManager entityManager;
        private Entity asteroidaEntityPrefab;
        private const int ODSTEP_POMIEDZY_ASTEROIDAMI = 2;
        private const float MAX_PREDKOSC_LINIOWA = 2;


        public AsteroidsManager(GameObject asteroidaPrefab)
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            asteroidaEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(asteroidaPrefab, new GameObjectConversionSettings()
            {
                DestinationWorld = World.DefaultGameObjectInjectionWorld
            });
        }
        public void TworzAsteroidy(int grid)
        {
            NativeArray<Entity> listaAsteroid = new NativeArray<Entity>(grid * grid, Allocator.TempJob);
            entityManager.Instantiate(asteroidaEntityPrefab, listaAsteroid);

            float3 poz;
            float3 kierunek;



            for (int i = 0; i < grid; i++)
            {
                for (int j = 0; j < grid; j++)
                {
                    poz = new float3(((i - grid / 2) * ODSTEP_POMIEDZY_ASTEROIDAMI), ((j - grid / 2) * ODSTEP_POMIEDZY_ASTEROIDAMI), 0);

                    kierunek = new float3(Random.Range(-MAX_PREDKOSC_LINIOWA, MAX_PREDKOSC_LINIOWA), Random.Range(-MAX_PREDKOSC_LINIOWA, MAX_PREDKOSC_LINIOWA), 0);
                    kierunek = math.normalize(kierunek);
                    //przesuniecie srodkowej asteroidy ktora inaczej koloduje ze statkiem i automatycznie konczy gre 
                    if (i == grid / 2 && j == grid / 2) poz = new float3(10 * ((grid / 2 + 1) * ODSTEP_POMIEDZY_ASTEROIDAMI), 10 * ((grid / 2 + 1) * ODSTEP_POMIEDZY_ASTEROIDAMI), 0);


                    entityManager.SetComponentData(listaAsteroid[i * grid + j], new Translation { Value = poz });
                    entityManager.AddComponentData(listaAsteroid[i * grid + j], new Kierunek { Value = kierunek });


                    //entityManager.AddComponentData(listaAsteroid[i * grid + j], new Kolizyjny { typ = Typ.ASTEROIDA }); 


                    /* 
                     entityManager.AddComponentData(listaAsteroid[i * grid + j], new Predkosc { Value = 10 });*/
                    //entityManager.AddComponentData(listaAsteroid[i * grid + j], new Scale { Value = 0.5f });

                }
            }

            listaAsteroid.Dispose();

        }

    }


}
