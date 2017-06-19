using UnityEngine;
using System.Collections;
using System;

public class Character2Behavior : MonoBehaviour
{
    Character2 attributionCharacter;
    UnityEngine.AI.NavMeshAgent agent;
    CharacterController controller;

    GroundNavigation2 groundNavi;
    TestCarry2 grabbingCarry;

    public void SetAttribution(Character2 c)
    {
        attributionCharacter = c;

        controller = GetComponent<CharacterController>();
        controller.enabled = false;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            agent.height = controller.height;
            agent.radius = controller.radius;
        }
        agent.enabled = false;

        GameObject g = new GameObject("navigation_" + gameObject.name);
        g.transform.position = transform.position;
        groundNavi = g.AddComponent<GroundNavigation2>();
        groundNavi.init(this);

        ClimbSystem.SetGameObject(transform);
    }

    public Character2 GetAttribution()
    {
        return attributionCharacter;
    }

    public GroundNavigation2 GetGroundNavi()
    {
        return groundNavi;
    }

    public Vector3 GetVelocity()
    {
        if (agent.isActiveAndEnabled)
        {
            return agent.velocity;
        }

        if (controller.enabled)
        {
            return controller.velocity;
        }

        return Vector3.zero;
    }

    void Update()
    {
        if (transform.position.y < StaticParameter.FALL)
        {
            attributionCharacter.ChangeState(CharacterState2.Fall);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit h)
    {
        attributionCharacter.ControllerHit(h);
    }

    void OnCollisionEnter(Collision c)
    {
        CollisionableToCharacter col = c.gameObject.GetComponent<CollisionableToCharacter>();
        if (col != null)
        {
            col.CollisionAction(attributionCharacter, c);
        }
    }

    public void Catch()
    {
        if (grabbingCarry == null)
        {
            if (attributionCharacter.GetState() == CharacterState2.Ground)
            {
                Collider[] c = Physics.OverlapBox(
                transform.position + transform.forward * controller.radius * 2f,
                Vector3.one * controller.radius,
                transform.rotation);

                CarryGrabPoint2 carry;
                for (int i = 0; i < c.Length; i++)
                {
                    carry = c[i].gameObject.transform.GetComponent<CarryGrabPoint2>();
                    if (carry != null)
                    {
                        if (carry.SetGrabbing(this, controller.radius))
                        {
                            grabbingCarry = carry.GetCarry();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            LoseCarry();
        }
    }

    public void LoseCarry()
    {
        if (grabbingCarry != null)
        {
            grabbingCarry.LoseJoint();
            grabbingCarry = null;
        }
    }
}

public enum CharacterState2
{
    NoAppear,
    Ground,
    Sky,
    Hang,
    Clamber,
    Down,
    Fall,

}