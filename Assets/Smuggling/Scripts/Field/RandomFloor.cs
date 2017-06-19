using UnityEngine;
using System.Collections;

public class RandomFloor : FloorData
{

    public override void Start()
    {
        Ground[] groundData = transform.GetComponentsInChildren<Ground>();
        for (int i = 0; i < groundData.Length; i++)
        {
            groundData[i].init(RandomGround());
        }
        Destroy(this);
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
