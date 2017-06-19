using UnityEngine;
using System.Collections;
using HTGames.BattleMain;
using System;

public class Bullet : AttackObject, Blowable
{
    [SerializeField]
    GameObject effecter;
    [SerializeField]
    float attackableSpeed = 3f;

    Rigidbody rigid;
    Character owner;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (rigid == null)
        {
            Debug.LogError("No Rigidbody was found!!");
            Destroy(this);
        }
    }

    void Update()
    {
        float mag = rigid.velocity.magnitude;
        if (mag > attackableSpeed)
        {
            Activate(true, 0);
            blowMagnitude = mag * 0.2f;
            if (effecter != null)
            {
                effecter.SetActive(true);
                effecter.transform.parent.LookAt(transform.position + rigid.velocity);
            }
        }
        else
        {
            Activate(false, 0);
            owner = null;
            if (effecter != null)
            {
                effecter.SetActive(false);
            }
        }

        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    void Blowable.Blow(Vector3 vec)
    {
        rigid.AddForce(vec, ForceMode.VelocityChange);
    }

    public void Blow(Vector3 vec, Character newOwner)
    {
        rigid.AddForce(vec, ForceMode.VelocityChange);
        owner = newOwner;
        Activate(true, 0);
    }

    public override void Touch(Character c)
    {
        if (c != owner)
        {
            base.Touch(c);
            if (owner != null)
            {
                owner.AddReward(1);
            }
        }
    }
}
