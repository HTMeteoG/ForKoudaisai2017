using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitterDummy : MonoBehaviour
{
    Rigidbody r;
    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < -10)
        {
            transform.localPosition = Vector3.zero;
            r.useGravity = false;
            r.velocity = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("BattleLayer");
        }
    }

    public void Damage(Vector3 vec)
    {
        r.AddForce(vec, ForceMode.VelocityChange);
        r.useGravity = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        Debug.Log("Damage");
    }
}
