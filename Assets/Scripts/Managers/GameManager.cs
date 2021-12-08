using Piongames;
using PionGames.Systems;
using Unity.Entities;
using UnityEngine;


namespace PionGames.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject asteroidaPrefab;
        [SerializeField] private GameObject statekPrefab;


        public AsteroidsManager asteroidsManager { get; set; }
        private KolizyjnySystem kolizyjnySystem;

        void Start()
        {
            asteroidsManager = new AsteroidsManager(asteroidaPrefab);
            asteroidsManager.TworzAsteroidy(Settings.GRID);
            kolizyjnySystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<KolizyjnySystem>();
            kolizyjnySystem.UtworzTabeleHashMap();
            kolizyjnySystem.Enabled = true;

        }





    }
}
