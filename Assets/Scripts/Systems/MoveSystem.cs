using PionGames.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
/*
namespace PionGames.Systems
{
 */

//** UWAGA JobComponentSystem should not be used in new code: **/ //https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/chunk_iteration.html
//**  The ComponentSystem and JobComponentSystem classes, along with IJobForEach, are depreciated
//public class MoveSystem : JobComponentSystem
//{
//        protected override void OnCreate()
//        {

//            //Enabled = false;
//        }
//        [BurstCompile]
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            float dt = Time.DeltaTime;
//            var jobHandle = Entities
//                //.ForEach((ref Translation translation, in Kierunek kierunek, in Predkosc predkosc) =>
//                .ForEach((ref Translation translation, in Kierunek kierunek) =>
//                {
//                    //translation.Value += kierunek.Value * predkosc.Value * dt;
//                    translation.Value += kierunek.Value * 1 * dt;
//                })
//                .Schedule(inputDeps);

//            return jobHandle;
//        }
//    }

//}

namespace PionGames.Systems
{

    public class MoveSystem : SystemBase
    {
       
       
        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;
            Entities                
                .ForEach((ref Translation translation, in Kierunek kierunek) =>
                {
                    //translation.Value += kierunek.Value * predkosc.Value * dt;
                    translation.Value += kierunek.Value * 1 * dt;
                })
                //lub .ScheduleParallel(); - nie wiem jaka roznica
                //lub .ScheduleParallel(this.Dependency);
                .Schedule();

        }
    }

}

