
using PionGames.Structs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//[BurstCompile]
public struct ZderzeniaJOB : IJob
{
    [ReadOnly]
    public NativeArray<EntityData> positions;
    //public NativeArray<Entity> entities;
    public NativeList<Entity> e2D;
    public NativeList<Entity> e2R;
    public const float wielkoscAsterooidy = 1f;  //kwadrat dlugosci!
    //public NativeArray<bool> czyGameOver;

    public void Execute()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = 0; j < positions.Length; j++)
            {

                if (i == j) continue;
                if (CheckCollision(positions[i].position, positions[j].position, wielkoscAsterooidy))
                {
                   // e2D.Add(entities[i]);
                   // e2D.Add(entities[j]);
                    /* Debug.Log($"zderzenie {i}, {j}");
                     Debug.Log("zderzenie "+ positions[i].Value.x+" "+ positions[j].Value.x);
                     Debug.Log("zderzenie " + positions[i].Value.y + " " + positions[j].Value.y);*/

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
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.y * delta.y;

        return distanceSquare <= radiusSqr;
    }
}