
using PionGames.Structs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public struct ZderzeniaJOB : IJob
{
    // [ReadOnly]
    //public NativeArray<Translation> positions;
    [ReadOnly]
    public NativeList<EntityData> entitiesData;
    public NativeList<Entity> e2D;
    public NativeList<Entity> e2R;
    public const float wielkoscAsterooidy = 1f;  //kwadrat dlugosci!
    //public NativeArray<bool> czyGameOver;

    public void Execute()
    {

        for (int i = 0; i < entitiesData.Length; i++)
        {
            for (int j = 0; j < entitiesData.Length; j++)
            {

                if (i == j) continue;
                if (CheckCollision(entitiesData[i].position, entitiesData[j].position, wielkoscAsterooidy))
                {

                    //Debug.Log("--------------------------MAM entity 2D");
                    e2D.Add(entitiesData[i].entity);
                    e2D.Add(entitiesData[j].entity);
                    //* Debug.Log($"zderzenie {i}, {j}");
                    // Debug.Log("zderzenie "+ positions[i].Value.x+" "+ positions[j].Value.x);
                    // Debug.Log("zderzenie " + positions[i].Value.y + " " + positions[j].Value.y);*//*

                }



            }

        }
        /*float2 d1;
        foreach (Translation idPos in tabelaDoSprawdzenia)
        {
            d1 = idPos.poz;
            float2 d2;
            foreach (Translation idPos2 in tabelaDoSprawdzenia)
            {
                if (idPos.index == idPos2.index) continue;

                d2 = idPos2.poz;

                if (math.distance(d1, d2) < wielkoscAsterooidy)
                {
                    if (idPos.typ == Typ.ASTEROIDA) e2R.Add(idPos.entity);
                    else if (idPos.typ == Typ.POCISK) e2D.Add(idPos.entity);
                    else if (idPos.typ == Typ.STATEK)
                    {
                        //GAMEOVER
                        czyGameOver[0] = true;

                    }
                }
            }
        }*/
    }
    private bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
    {

        //Debug.Log("posA "+ posA+ "posB " + posB);
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.y * delta.y;

        return distanceSquare <= radiusSqr;
    }
}