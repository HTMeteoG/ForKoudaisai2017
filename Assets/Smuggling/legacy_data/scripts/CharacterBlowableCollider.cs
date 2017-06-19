using UnityEngine;
using System.Collections;
using System;

namespace HTGames
{
    namespace BattleMain
    {
        //CharacterRagdollの子オブジェクト専用
        public class CharacterBlowableCollider : MonoBehaviour, Blowable
        {
            CharacterRagdoll owner;
            Rigidbody r;

            void Awake()
            {
                Transform t = transform;
                while (t != null)
                {
                    owner = t.GetComponent<CharacterRagdoll>();
                    if (owner != null)
                    {
                        break;
                    }
                    t = t.parent;
                }

                r = GetComponent<Rigidbody>();
            }

            void Update()
            {
                //消滅処理
                if (owner == null && transform.position.y < -10)
                {
                    Destroy(gameObject);
                }
            }

            public void Blow(Vector3 vec)
            {
                r.AddForce(vec, ForceMode.VelocityChange);
            }
        }
    }
}