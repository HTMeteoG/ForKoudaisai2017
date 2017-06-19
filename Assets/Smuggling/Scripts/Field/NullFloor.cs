using UnityEngine;
using System.Collections;

public class NullFloor : FloorData
{

    public override void Start()
    {
        Ground[] groundData = transform.GetComponentsInChildren<Ground>();
        for (int i = 0; i < groundData.Length; i++)
        {
            groundData[i].init(GroundType.Disappear);
        }
        Destroy(this);
    }
}
