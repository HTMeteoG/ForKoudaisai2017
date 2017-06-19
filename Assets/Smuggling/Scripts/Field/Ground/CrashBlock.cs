using UnityEngine;
using System.Collections;
using System;

public class CrashBlock : MonoBehaviour,CollisionableToCharacter
{

    void Start()
    {
        UnityEngine.AI.NavMeshObstacle obst = gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
        obst.size = Vector3.one * 0.8f;
    }

    void Update()
    {
        if (transform.position.y < StaticParameter.FALL)
        {
            Destroy(gameObject,2f);
        }
    }

    public void CollisionAction(Character2 chara, Collision c)
    {
        Vector3 rv = c.relativeVelocity;
        if(rv.magnitude > chara.GetSpeed())
        {
            chara.ForceMove(rv);
            chara.Damage(1);
        }
    }
}
