using UnityEngine;
using System.Collections;

public class TestCharacter2 : MonoBehaviour
{
    /*
    NavMeshAgent agent;
    CharacterController controller;
    Rigidbody rigid;
    CharacterState2 state;
    float speed = 4f;
    float rotateSpeed = 360f;
    float accel = 8f;
    float carryingSpeed = 2f;
    float carryingRotateSpeed = 90f;
    float power = 2f;
    float fallAcceleration = -9.8f;
    float jumpPower = 10f;
    float fallWaitTime = 0.1f;
    float clamberTime = 0.5f;

    Ground landingGround;
    GroundNavigation2 groundNavi;
    Vector3 jumpVelocity = Vector3.zero;
    float fallWait;
    Vector3 hangPosition;
    float clamber;
    TestCarry2 grabbingCarry;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.height = controller.height;
            agent.radius = controller.radius;
        }
        agent.speed = speed;
        agent.angularSpeed = rotateSpeed;
        agent.autoRepath = false;
        agent.enabled = false;

        GameObject g = new GameObject("navigation_" + gameObject.name);
        groundNavi = g.AddComponent<GroundNavigation2>();
        groundNavi.init(this);

        state = CharacterState2.Sky;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");
        bool fire1 = Input.GetButtonDown("Fire1");

        Vector3 move = new Vector3(moveX, 0, moveY);
        float inputSpeed = Mathf.Min(1, move.magnitude);
        switch (state)
        {
            case CharacterState2.Ground:
                if (grabbingCarry == null)
                {
                    agent.speed = speed;
                    agent.angularSpeed = rotateSpeed;
                }
                else
                {
                    agent.speed = carryingSpeed;
                    agent.angularSpeed = carryingRotateSpeed;
                }
                agent.velocity = move * agent.speed;

                if (jump == 1)
                {
                    jumpVelocity = new Vector3(moveX * 0.5f, jumpPower, moveY * 0.5f);
                    move += jumpVelocity;
                    ChangeState(CharacterState2.Sky);
                    LoseCarry();
                }
                break;

            case CharacterState2.Sky:
                move = move * speed * inputSpeed;
                move += jumpVelocity;
                jumpVelocity += Vector3.up * fallAcceleration * Time.deltaTime;
                Move(move);
                break;

            case CharacterState2.Hang:
                clamber = 0;
                if (jump == 1)
                {
                    ChangeState(CharacterState2.Clamber);
                }
                else if (Vector3.Dot(move, hangPosition) < 0)
                {
                    ChangeState(CharacterState2.Sky);
                }
                move = Vector3.zero;
                break;

            case CharacterState2.Clamber:
                move = Vector3.zero;
                Vector3 remainVec = groundNavi.transform.position + Vector3.up * controller.height * 0.5f - transform.position;
                transform.position += remainVec * Mathf.Min(1, (Time.deltaTime / (clamberTime - clamber)));
                clamber += Time.deltaTime;
                if (clamber >= clamberTime)
                {
                    transform.position = groundNavi.transform.position + Vector3.up * controller.height * 0.5f;
                    ChangeState(CharacterState2.Ground);
                }
                break;
        }

        if (fire1)
        {
            Catch();
        }

        //test
        if (Input.GetButtonDown("Fire2"))
        {
            if(state == CharacterState2.Down)
            {
                ChangeState(CharacterState2.Sky);
            }
            else
            {
                ChangeState(CharacterState2.Down);
            }
        }
        //test

        //Fall test
        if (transform.position.y < -25)
        {
            transform.position = new Vector3(10, 10, 0);
            ChangeState(CharacterState2.Sky);
        }
        //Fall test
    }

    public void Move(Vector3 vec)
    {
        switch (state)
        {
            case CharacterState2.Ground:
                agent.Move(vec * Time.deltaTime);
                break;

            case CharacterState2.Sky:
                controller.Move(vec * Time.deltaTime);
                break;
        }
    }

    void Catch()
    {
        if (grabbingCarry == null)
        {
            if (state == CharacterState2.Ground)
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

    void OnControllerColliderHit(ControllerColliderHit h)
    {
        Vector3 localPoint = h.point - transform.position;

        Ground g = h.gameObject.GetComponent<Ground>();
        if (g != null)
        {
            switch (state)
            {
                case CharacterState2.Ground:
                    landingGround = g;
                    fallWait = 0;
                    break;

                case CharacterState2.Sky:
                    if (localPoint.y <= -controller.height * 0.4f)
                    {
                        ChangeState(CharacterState2.Ground);
                        landingGround = g;
                    }
                    else if (g.isHangable() && localPoint.y >= controller.height * 0.25f)
                    {
                        hangPosition = localPoint;
                        ChangeState(CharacterState2.Hang);
                        groundNavi.SetGround(g, h.point);
                        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(localPoint, Vector3.up));
                    }
                    break;

                case CharacterState2.Hang:
                    if (localPoint.y <= -controller.height * 0.4f)
                    {
                        ChangeState(CharacterState2.Ground);
                        landingGround = g;
                    }
                    break;
            }

            if (state == CharacterState2.Ground)
            {
                groundNavi.SetGround(landingGround, h.point);
            }
        }

        if (state == CharacterState2.Sky)
        {
            Vector3 loseVelocity = Vector3.Project(jumpVelocity, localPoint);
            if (Vector3.Dot(loseVelocity, localPoint) > 0)
            {
                jumpVelocity -= loseVelocity;
            }

            TestCarry carry = h.gameObject.transform.root.GetComponent<TestCarry>();
            if (carry != null)
            {
                LoseCarry();
            }
        }

        Rigidbody r = h.gameObject.GetComponent<Rigidbody>();
        if (r == null)
        {
            r = h.transform.root.GetComponent<Rigidbody>();
        }
        if (r != null)
        {
            r.AddForceAtPosition(h.moveDirection, h.point, ForceMode.VelocityChange);
        }
    }

    public void ChangeState(CharacterState2 newState)
    {
        if (state != newState)
        {
            switch (newState)
            {
                case CharacterState2.Ground:
                    state = CharacterState2.Ground;
                    jumpVelocity = Vector3.down;
                    controller.enabled = false;
                    agent.enabled = true;
                    fallWait = 0;
                    break;

                case CharacterState2.Sky:
                    state = CharacterState2.Sky;
                    landingGround = null;
                    controller.enabled = true;
                    agent.enabled = false;
                    rigid.isKinematic = true;
                    groundNavi.SetGround(landingGround, Vector3.zero);
                    break;

                case CharacterState2.Hang:
                    state = CharacterState2.Hang;
                    jumpVelocity = Vector3.zero;
                    agent.enabled = false;
                    break;

                case CharacterState2.Clamber:
                    state = CharacterState2.Clamber;
                    controller.enabled = false;
                    agent.enabled = false;
                    break;

                case CharacterState2.Down:
                    state = CharacterState2.Down;
                    controller.enabled = false;
                    agent.enabled = false;
                    rigid.isKinematic = false;
                    break;
            }
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
    */
}