using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using koudaigame2017;

namespace EvolutionaryProgramming
{
    public class EPCharacter : Character
    {
        EP_Input epInput;
        Character targetCharacter;

        protected override void Start()
        {
            base.Start();
            epInput = GetComponent<EP_Input>();
        }

        protected override void Update()
        {
            base.Update();
            if (epInput != null && epInput.GetcomTime() > 0.5f)
            {
                if (agent.hasPath)
                {
                    if (agent.desiredVelocity.sqrMagnitude > 0 && agent.velocity.sqrMagnitude < 0.02f)
                    {
                        epInput.NextCommand();
                    }
                }
                else
                {
                    if (anime.GetBool("landing") && !anime.GetBool("RejectInput"))
                    {
                        epInput.NextCommand();
                    }
                }
            }
        }

        public void SetDestination(Vector3 position)
        {
            agent.SetDestination(position);
        }

        public void SetMoveVelocity(Vector3 vec, float ratio)
        {
            //ratio is 0 to 1
            //if (!anime.GetBool("landing") && !anime.GetBool("catchCriff"))
            //{
                moveVelocity = vec.normalized * agent.speed * ratio;
            //}
        }

        public void SetJumpFlag()
        {
            anime.SetTrigger("JumpTrigger");
        }

        public void SetAttackFlag()
        {
            anime.SetTrigger("AttackTrigger");
        }

        public Character GetTarget()
        {
            return targetCharacter;
        }

        public void FindTarget(float range)
        {
            if (targetCharacter == null)
            {
                Collider[] checker = Physics.OverlapBox(
                    transform.position + transform.forward * range,
                    Vector3.one * range * 0.5f, transform.rotation,
                    LayerMask.GetMask("BattleLayer"));
                for (int i = 0; i < checker.Length; i++)
                {
                    targetCharacter = checker[i].GetComponentInParent<TestNPC>();
                    if (targetCharacter != null)
                    {
                        break;
                    }
                }
            }
        }
    }
}