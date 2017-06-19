using UnityEngine;
using System.Collections.Generic;

public class FloorData : MonoBehaviour
{
    [SerializeField]
    List<GroundType> groundTypeData = new List<GroundType>();

    Ground[] groundData;

    public virtual void Start()
    {
        groundData = transform.GetComponentsInChildren<Ground>();

        Debug.Log(gameObject.name + "_Grounds = " + groundData.Length);

        for (int i = 0; i < groundData.Length; i++)
        {
            if (i < groundTypeData.Count)
            {
                groundData[i].init(groundTypeData[i]);
            }
        }
        Destroy(this);
    }

    void Update()
    {

    }
}
