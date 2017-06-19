using UnityEngine;
using System.Collections;

public class SightCollider : MonoBehaviour
{
    Character2Behavior body;

    void Start()
    {
        body = transform.parent.GetComponent<Character2Behavior>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject != body.gameObject)
        {
            body.GetAttribution().SightAction(c);
        }
    }
}
