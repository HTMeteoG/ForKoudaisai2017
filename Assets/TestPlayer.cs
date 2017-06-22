using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestPlayer : MonoBehaviour
{
    Camera c;
    NavMeshAgent agent;
    Animator anime;
    CharacterController cc;
    float camHeight = 1.5f;
    float camForward = 5;
    Vector3 xAxis = Vector3.forward;
    Vector3 newXAxis;

    float rotateTime = 1;

    float fallvelocity; // unit vector is Vector3.down

    void Start()
    {
        c = FindObjectOfType<Camera>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        anime = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        fallvelocity = 0;

        c.transform.rotation = Quaternion.Euler(5, -Vector3.Angle(Vector3.right, xAxis), 0);
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        if (anime.GetBool("landing"))
        {
            agent.velocity = xAxis * moveX * agent.speed;
            anime.SetFloat("speed", agent.velocity.magnitude);

            if (Input.GetButtonDown("Jump"))
            {
                cc.enabled = true;
                agent.enabled = false;
                anime.SetBool("landing", false);
                fallvelocity = -10;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                anime.SetTrigger("AttackTrigger");
            }
        }
        else if (anime.GetBool("catchCriff"))
        {
            if (Input.GetButtonDown("Jump"))
            {
                anime.SetTrigger("JumpTrigger");
            }
        }
        else
        {
            fallvelocity += 9.8f * Time.deltaTime;
            Vector3 moveVelocity = xAxis * moveX * agent.speed + Vector3.down * fallvelocity;
            cc.Move(moveVelocity * Time.deltaTime);
            anime.SetFloat("speed", 0);
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
            c.transform.rotation = Quaternion.LookRotation(Vector3.Cross(xAxis, Vector3.up));
            //+ Quaternion.Euler(5, 0, 0);
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") >= 0)
            {
                newXAxis = Vector3.Cross(xAxis, Vector3.up);
            }
            else
            {
                newXAxis = -Vector3.Cross(xAxis, Vector3.up);
            }
            rotateTime = 0;
        }


        if (c != null)
        {
            c.transform.position = transform.position
                + Vector3.up * camHeight - Vector3.Cross(xAxis, Vector3.up) * camForward;
        }

    }

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
                fallvelocity = 0;
                anime.SetBool("catchCriff", true);
                anime.SetTrigger("catchCriffTrigger");
            }
            else if (Vector3.ProjectOnPlane(hit.point - transform.position, Vector3.up).magnitude < cc.radius)
            {
                fallvelocity = 0;
            }
        }
    }
}
