using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadDebris : MonoBehaviour
{
    private void Start()
    {
        transform.DetachChildren();
    }
    void Update()
    {
        if(transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
