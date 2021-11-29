using UnityEngine;


namespace PionGames.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject asteroidaPrefab;
        [SerializeField] private GameObject statekPrefab;
        private int grid = 10;

        public AsteroidsManager asteroidsManager { get; set; }

        void Start()
        {
            asteroidsManager = new AsteroidsManager(asteroidaPrefab);
            asteroidsManager.TworzAsteroidy(grid);
           
        }


       

        
    }
}
