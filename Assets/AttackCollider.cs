using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace koudaigame2017
{
    public class AttackCollider : MonoBehaviour
    {
        TestPlayer owner;

        public void SetOwner(GameObject g)
        {
            owner = g.GetComponent<TestPlayer>();
        }

        public void OnTriggerEnter(Collider other)
        {
            GameObject hitterobject = other.transform.root.gameObject;
            Hitter hitter = hitterobject.GetComponentInChildren<Hitter>();
            if (hitter != null)
            {
                Vector3 damVec = (hitterobject.transform.position - owner.transform.position).normalized * 5;
                hitter.Damage(damVec);
            }
        }
    }
}
