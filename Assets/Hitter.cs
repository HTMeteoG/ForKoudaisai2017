using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace koudaigame2017
{
    public class Hitter : MonoBehaviour
    {
        Rigidbody r;
        Character c;
        void Start()
        {
            r = GetComponent<Rigidbody>();
            c = GetComponentInParent<Character>();
        }

        void Update()
        {

        }

        public virtual void Damage(Vector3 vec)
        {
            c.Damage(vec);
            Debug.Log("Damage_character");
        }
    }
}
