using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace koudaigame2017
{
    public class Character : MonoBehaviour
    {
        protected NavMeshAgent agent;
        protected Animator anime;
        protected CharacterController cc;
        protected Vector3 xAxis = Vector3.forward;

        protected Vector3 moveVelocity; // using only in Sky
        protected bool jumpFlag;
        protected bool attackFlag;

        protected Vector3 fallvelocity;

        protected float HP = 100; //仮設状態
        [SerializeField]
        protected GameObject deadObject;

        protected virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
            anime = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
            fallvelocity = Vector3.zero;
        }

        protected virtual void Update()
        {
            if (!anime.GetBool("RejectInput"))
            {
                if (anime.GetBool("landing"))
                {
                    anime.SetFloat("speed", agent.velocity.magnitude);

                    if (jumpFlag)
                    {
                        cc.enabled = true;
                        agent.enabled = false;
                        anime.SetBool("landing", false);
                        fallvelocity = Vector3.up * 10;
                    }

                    if (attackFlag)
                    {
                        anime.SetTrigger("AttackTrigger");
                    }
                }
                else if (anime.GetBool("catchCriff"))
                {
                    if (jumpFlag)
                    {
                        anime.SetTrigger("JumpTrigger");
                    }
                }
                else
                {
                    fallvelocity += Vector3.down * 9.8f * Time.deltaTime;
                    Vector3 moveVec = moveVelocity + fallvelocity;
                    cc.Move(moveVec * Time.deltaTime);
                    anime.SetFloat("speed", 0);
                }
            }
            else
            {
                Stop();
            }

            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }

            jumpFlag = false;
            attackFlag = false;
        }

        public Vector3 GetXaxis()
        {
            NavMeshHit edge;
            if (NavMesh.FindClosestEdge(transform.position, out edge, NavMesh.AllAreas))
            {
                Vector3 rightVec = edge.position - transform.position;
                return Vector3.Cross(rightVec, Vector3.up);
            }
            else
            {
                return Vector3.zero;
            }
        }

        //着地、空中接触のみに用いる。ダメージ判定はしない。
        public virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            NavMeshHit navhit;
            if (NavMesh.SamplePosition(transform.position, out navhit, cc.radius * 1.5f, agent.areaMask))
            {
                cc.enabled = false;
                agent.enabled = true;
                anime.SetBool("landing", true);
            }
            else
            {
                if (NavMesh.SamplePosition(transform.position + Vector3.up * cc.height, out navhit, cc.radius * 2.5f, agent.areaMask))
                {
                    Vector3 lookvec = Vector3.ProjectOnPlane(navhit.position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.LookRotation(lookvec, Vector3.up);
                    transform.position = navhit.position + Vector3.down * cc.height - transform.forward * cc.radius * 2f;
                    fallvelocity = Vector3.zero;
                    anime.SetBool("catchCriff", true);
                    anime.SetTrigger("catchCriffTrigger");
                }
                else if (Vector3.ProjectOnPlane(hit.point - transform.position, Vector3.up).magnitude < cc.radius)
                {
                    fallvelocity = Vector3.zero;
                }
                else
                {
                    fallvelocity = transform.position + Vector3.up * cc.height * 0.5f - hit.point;
                }
            }
        }

        public virtual void Damage(float val, Vector3 vec)
        {
            HP += val;//仮置き状態

            if (HP <= 0)
            {
                Dead();
            }
            else
            {
                anime.SetTrigger("damage");
                anime.SetBool("catchCriff", false);
                anime.SetBool("landing", false);
                cc.enabled = true;
                agent.enabled = false;

                Vector3 Xaxis = GetXaxis();
                if (Xaxis != Vector3.zero)
                {
                    fallvelocity = Vector3.Project(vec, Xaxis).normalized;
                }
                else
                {
                    //fallvelocity = vec;
                }
            }
        }

        public virtual void Damage(Vector3 vec)
        {
            Damage(-10, vec);
        }

        protected virtual void Dead()
        {
            if (deadObject != null)
            {
                GameObject d = Instantiate(deadObject, transform.position, transform.rotation);
                d.transform.DetachChildren();
                Destroy(d, 0.1f);
            }
            Destroy(gameObject);
        }

        public void Stop()
        {
            if (agent.enabled)
            {
                agent.ResetPath();
            }
        }
    }

    public class TestNPC : Character
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        public void SetDestination(Vector3 position)
        {
            if (anime.GetBool("landing"))
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, 5, agent.areaMask))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }

        public void SetMoveVelocity(Vector3 vec, float ratio)
        {
            //ratio is 0 to 1
            if (!anime.GetBool("landing") && !anime.GetBool("catchCriff"))
            {
                moveVelocity = vec.normalized * agent.speed * ratio;
            }
        }

        public void SetJumpFlag()
        {
            jumpFlag = true;
        }

        public void SetAttackFlag()
        {
            attackFlag = true;
        }
    }
}