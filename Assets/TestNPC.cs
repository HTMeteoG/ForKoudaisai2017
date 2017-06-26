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
        protected Vector3 newXAxis;

        protected float rotateTime = 1;

        protected Vector3 fallvelocity; // unit vector is Vector3.down

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
            float moveX = 0;//Input.GetAxis("Horizontal");

            if (!anime.GetBool("RejectInput"))
            {
                if (anime.GetBool("landing"))
                {
                    agent.velocity = xAxis * moveX * agent.speed;
                    anime.SetFloat("speed", agent.velocity.magnitude);

                    if (Input.GetButtonDown("Jump"))
                    {
                        //cc.enabled = true;
                        //agent.enabled = false;
                        //anime.SetBool("landing", false);
                        //fallvelocity = -10;
                    }

                    if (Input.GetButtonDown("Fire1"))
                    {
                        //anime.SetTrigger("AttackTrigger");
                    }
                }
                else if (anime.GetBool("catchCriff"))
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        //anime.SetTrigger("JumpTrigger");
                    }
                }
                else
                {
                    fallvelocity += Vector3.down * 9.8f * Time.deltaTime;
                    Vector3 moveVelocity = xAxis * moveX * agent.speed + fallvelocity;
                    fallvelocity *= 0.9f;
                    cc.Move(moveVelocity * Time.deltaTime);
                    anime.SetFloat("speed", 0);
                }
            }

            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }

            // CameraMotion (for Player only)
            if (rotateTime < 1)
            {
                rotateTime += Time.deltaTime;
                xAxis = Vector3.RotateTowards(xAxis, newXAxis, rotateTime, 0);
            }
            else if (Input.GetButtonDown("Vertical"))
            {
                //if (Input.GetAxis("Vertical") >= 0)
                //{
                //    newXAxis = Vector3.Cross(xAxis, Vector3.up);
                //}
                //else
                //{
                //    newXAxis = -Vector3.Cross(xAxis, Vector3.up);
                //}
                //rotateTime = 0;
            }
        }

        //着地、空中接触のみに用いる。ダメージ判定はしない。
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            NavMeshHit navhit;
            if (NavMesh.SamplePosition(transform.position, out navhit, cc.radius, agent.areaMask))
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
            }
        }

        public virtual void Damage(Vector3 vec)
        {
            HP -= 10;//仮置き状態

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
                fallvelocity = vec;
            }
        }

        protected virtual void Dead()
        {
            if(deadObject != null)
            {
                GameObject d = Instantiate(deadObject, transform.position, transform.rotation);
                d.transform.DetachChildren();
                Destroy(d, 0.1f);
            }
            Destroy(gameObject);
        }
    }

    public class TestNPC : Character
    {
        protected override void Start()
        {
            base.Start();
        }
    }
}