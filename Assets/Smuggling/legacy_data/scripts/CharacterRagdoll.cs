using UnityEngine;
using System.Collections.Generic;
using System;

namespace HTGames
{
    namespace BattleMain
    {
        public class CharacterRagdoll : MonoBehaviour, Controlable
        {
            Character character;
            Rigidbody blowRigidbody;

            [SerializeField]
            float rigidMass_sum = 1;

            public void SetCharacter(Character c)
            {
                character = c;
            }

            void Awake()
            {
                blowRigidbody = GetComponent<Rigidbody>();
                gameObject.layer = LayerMask.NameToLayer("Hitter");
            }

            void Start()
            {

            }

            void Update()
            {
                //testing(落死処理)
                if (blowRigidbody.transform.position.y < -10)
                {
                    character.Delete();
                }
                //testing
            }

            public void Move(Vector3 vec)
            {

            }

            void OnTriggerEnter(Collider c)
            {
                AttackObject attack = c.gameObject.GetComponent<AttackObject>();
                if (attack != null)
                {
                    attack.Touch(GetOwner());
                }
            }

            public void Blow(Vector3 vec)
            {
                blowRigidbody.AddForce(vec, ForceMode.VelocityChange);
            }

            public void Broken()
            {
                List<GameObject> childs = new List<GameObject>();
                childs.Add(gameObject);
                while (childs.Count > 0)
                {
                    Transform parent = childs[0].transform;
                    childs.RemoveAt(0);
                    parent.SetParent(null);
                    Joint j = parent.GetComponent<Joint>();
                    if (j != null)
                    {
                        j.breakForce = 1;
                        j.breakTorque = 1;
                    }
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        childs.Add(parent.GetChild(i).gameObject);
                    }
                }
                character.Delete();
            }

            public Character GetOwner()
            {
                return character;
            }
        }
    }
}
