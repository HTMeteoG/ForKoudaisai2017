using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace koudaigame2017
{
    public class Hitter : MonoBehaviour
    {
        Rigidbody r;
        Character c;
        bool resetTrigger;
        Vector3 position;
        Quaternion rotation;
        void Start()
        {
            r = GetComponent<Rigidbody>();
            r.constraints = RigidbodyConstraints.FreezeAll;
            c = GetComponentInParent<Character>();
            position = transform.position;
        }

        /*
        private void Update()
        {
            if (resetTrigger)
            {
                float moveChecker = (transform.position - position).sqrMagnitude;
                if(moveChecker > Mathf.Epsilon)
                {
                    transform.position = position;
                    r.velocity = Vector3.zero;
                }
            }
        }
        */

        void OnCollisionEnter(Collision collision)
        {
            //c.Stop();
        }

        public virtual void Damage(Vector3 vec)
        {
            c.Damage(vec);
        }
    }
}
