using PionGames.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

namespace PionGames.ECS.ECS_Systems
{
   
    public class MoveSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
           
            //Enabled = false;
        }
        [BurstCompile]
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float dt = Time.DeltaTime;
            var jobHandle = Entities
                //.ForEach((ref Translation translation, in Kierunek kierunek, in Predkosc predkosc) =>
                .ForEach((ref Translation translation, in Kierunek kierunek) =>
                {
                    //translation.Value += kierunek.Value * predkosc.Value * dt;
                    translation.Value += kierunek.Value * 1 * dt;
                })
                .Schedule(inputDeps);

            return jobHandle;
        }
    }

}

