using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct ZderzeniaJOB : IJobParallelFor
{
    // [ReadOnly]
    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<Translation> positions;
    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<Entity> entities;

    //NativeList nie jest dostepne dla IJobParallelFor
    //[DeallocateOnJobCompletion]
    [WriteOnly]
    public NativeQueue<Entity>.ParallelWriter e2D;
    
    

    //public NativeList<Entity> e2R;
    public const float wielkoscAsterooidy = 1f;  //kwadrat dlugosci!
                                                 //public NativeArray<bool> czyGameOver;

    /* public void Execute()
     {
         for (int i = 0; i < positions.Length; i++)
         {
             for (int j = 0; j < positions.Length; j++)
             {

                 if (i == j) continue;
                 if (CheckCollision(positions[i].Value, positions[j].Value, wielkoscAsterooidy))
                 {
                     e2D.Add(entities[i]);
                     e2D.Add(entities[j]);
                     //* Debug.Log($"zderzenie {i}, {j}");
                     // Debug.Log("zderzenie "+ positions[i].Value.x+" "+ positions[j].Value.x);
                     // Debug.Log("zderzenie " + positions[i].Value.y + " " + positions[j].Value.y);*//*

                 }



             }

         }

     }*/
   
    public void Execute(int index)
    {
       

        for (int j = 0; j < positions.Length; j++)
        {

            if (index == j) continue;
            if (CheckCollision(positions[index].Value, positions[j].Value, wielkoscAsterooidy))
            {
                e2D.Enqueue(entities[index]);
                e2D.Enqueue(entities[j]);
               
            }


        }
    }
    private bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
    {
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.y * delta.y;

        return distanceSquare <= radiusSqr;
    }


}