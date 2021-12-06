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

        void Start()
        {
            asteroidsManager = new AsteroidsManager(asteroidaPrefab);
            asteroidsManager.TworzAsteroidy(Settings.GRID);
            WlaczWylaczSystemKolizyjny();

        }
        private void WlaczWylaczSystemKolizyjny()
        {
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<KolizyjnySystem>().UtworzTabeleHashMap();
        }




    }
}
