using UnityEngine;
using System.Collections.Generic;

public class PathKeepingFloor_least : FloorData
{
    int pathKeeping;

    float dangerRate = 0.9f;
    List<Ground> unRideableGround = new List<Ground>();
    UnityEngine.AI.NavMeshPath floorPath;

    /*  予定
        １．まずランダムにUnBrokenとDangerだけを設置
        　　(このときDanger率を多めにする、Disappearにすると後が面倒なのでしない)
        ２．Updateごとにこの階層をNavMeshだけで突破できるかどうか、パスを探す
        ３．パスがない場合
            ３－１．Dangerをランダムに選び、Breakableに変更して２に戻る
        ４．パスがあった場合
            ４－１．全てのDangerをDangerかDisappearにランダムで変え(確率未定)、処理終了
    */

    public override void Start()
    {
        Ground[] groundData = transform.GetComponentsInChildren<Ground>();
        //最初は必ずUnBrokenにする
        groundData[0].init(GroundType.UnBroken);
        for (int i = 1; i < groundData.Length; i++)
        {
            if (Random.Range(0f, 1f) < dangerRate)
            {
                groundData[i].init(GroundType.Danger);
                unRideableGround.Add(groundData[i]);
            }
            else
            {
                groundData[i].init(GroundType.UnBroken);
            }
        }
        floorPath = new UnityEngine.AI.NavMeshPath();
        pathKeeping = 0;
    }

    void Update()
    {
        if (pathKeeping < 5)
        {
            UnityEngine.AI.NavMeshHit navStart;
            UnityEngine.AI.NavMesh.SamplePosition(transform.position, out navStart, 10f, UnityEngine.AI.NavMesh.AllAreas);
            UnityEngine.AI.NavMeshHit navGoal;
            UnityEngine.AI.NavMesh.SamplePosition(transform.position + Vector3.up * StaticParameter.FLOOR_HEIGHT,
                                    out navGoal, 10f, UnityEngine.AI.NavMesh.AllAreas);

            UnityEngine.AI.NavMesh.CalculatePath(navStart.position, navGoal.position, UnityEngine.AI.NavMesh.AllAreas, floorPath);
            if (floorPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
            {
                pathKeeping = 0;
                Ground g = unRideableGround[Random.Range(0, unRideableGround.Count)];
                g.init(GroundType.Breakable);
                unRideableGround.Remove(g);
            }
            else
            {
                pathKeeping++;
            }
        }
        else
        {
            foreach (Ground g in unRideableGround)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    g.init(GroundType.Disappear);
                }
            }
            Destroy(this);
        }
    }

    GroundType RandomGround()
    {
        int i = Random.Range(0, 4);
        switch (i)
        {
            case 0:
                return GroundType.Breakable;
            case 1:
                return GroundType.Danger;
            case 2:
                return GroundType.Disappear;
            default:
                return GroundType.UnBroken;
        }
    }
}
