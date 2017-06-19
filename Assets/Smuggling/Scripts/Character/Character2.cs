using UnityEngine;
using System.Collections;

public class Character2 : MonoBehaviour
{
    [SerializeField]
    GameObject characterBehavior;
    [SerializeField]
    GameObject characterDown;

    protected Character2Behavior behavior;
    protected Character2Down downBehavior;
    protected TrackingCamera2 trackCamera;

    protected CharacterState2 state;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected CharacterController controller;
    protected GroundNavigation2 groundNavi;

    //test parameter
    protected float speed = 4f;
    protected float rotateSpeed = 360f;
    protected float accel = 8f;
    protected float carryingSpeed = 2f;
    protected float carryingRotateSpeed = 90f;
    protected float power = 2f;
    protected float fallAcceleration = -9.8f;
    protected float jumpPower = 10f;
    protected float clamberTime = 0.5f;
    protected float life = 10000f;
    //test parameter

    //test input
    protected float moveX;
    protected float moveY;
    protected float jump;
    protected bool fire1;
    //test input

    protected Ground landingGround;
    protected Vector3 jumpVelocity = Vector3.zero;
    protected Vector3 hangPosition;
    protected float clamber;
    protected TestCarry2 grabbingCarry;
    protected float falledTime;

    public virtual void Start()
    {
        GameObject behaviorInstance = Instantiate(characterBehavior);
        behavior = behaviorInstance.GetComponent<Character2Behavior>();
        behavior.gameObject.name = gameObject.name + "_behavior";
        behavior.SetAttribution(this);
        behaviorInstance.SetActive(false);

        GameObject downInstance = Instantiate(characterDown);
        downBehavior = downInstance.GetComponent<Character2Down>();
        downBehavior.gameObject.name = gameObject.name + "_down";
        downBehavior.SetAttribution(this);
        downInstance.SetActive(false);

        state = CharacterState2.NoAppear;
    }

    public void Appear(Vector3 position, Vector3 eularAngle)
    {
        if (GetState() == CharacterState2.NoAppear)
        {
            if (isDead())
            {
                Destroy(gameObject);
                return;
            }

            ClimbSystem climb = FindObjectOfType<ClimbSystem>();
            if(climb.getFloorNum() < 3)
            {
                return;
            }

            if (position.y < StaticParameter.FALL)
            {
                position += Vector3.up * (StaticParameter.FALL_NUTRAL + 5 - position.y);
            }

            UnityEngine.AI.NavMeshHit point;
            if (UnityEngine.AI.NavMesh.SamplePosition(position, out point, StaticParameter.FLOOR_HEIGHT, UnityEngine.AI.NavMesh.AllAreas))
            {
                behavior.transform.position = point.position + Vector3.up;
                behavior.transform.rotation = Quaternion.Euler(eularAngle);
                behavior.gameObject.SetActive(true);

                agent = behavior.GetComponent<UnityEngine.AI.NavMeshAgent>();
                agent.autoRepath = false;
                agent.enabled = false;
                controller = behavior.GetComponent<CharacterController>();

                groundNavi = behavior.GetGroundNavi();
                ChangeState(CharacterState2.Sky);
                jumpVelocity = Vector3.zero;

                if (trackCamera != null)
                {
                    trackCamera.SetTrackTarget(behavior.transform);
                }
            }
        }
    }

    public void ControllerHit(ControllerColliderHit h)
    {
        Vector3 localPoint = h.point - behavior.transform.position;

        Ground g = h.gameObject.GetComponent<Ground>();
        if (g != null)
        {
            switch (GetState())
            {
                case CharacterState2.Ground:
                    landingGround = g;
                    break;

                case CharacterState2.Sky:
                    if (g.GetGroundType() == GroundType.Danger)
                    {
                        ChangeState(CharacterState2.Down);
                    }
                    else
                    {
                        if (localPoint.y <= -controller.height * 0.4f)
                        {
                            ChangeState(CharacterState2.Ground);
                            landingGround = g;
                            g.CollisionAction(this);
                        }
                        else if (g.isHangable() && localPoint.y >= controller.height * 0.25f)
                        {
                            hangPosition = localPoint;
                            ChangeState(CharacterState2.Hang);
                            groundNavi.SetGround(g, h.point);
                            behavior.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(localPoint, Vector3.up));
                        }
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

            if (GetState() == CharacterState2.Ground)
            {
                groundNavi.SetGround(landingGround, h.point);
            }
        }
        else
        {

            if (GetState() == CharacterState2.Sky)
            {
                UnityEngine.AI.NavMeshObstacle obst = h.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();
                if (obst != null && localPoint.y <= -controller.height * 0.4f)
                {
                    ChangeState(CharacterState2.Down);
                }
                else
                {
                    Vector3 loseVelocity = Vector3.Project(jumpVelocity, localPoint);
                    if (Vector3.Dot(loseVelocity, localPoint) > 0)
                    {
                        jumpVelocity -= loseVelocity;
                    }
                }

                TestCarry carry = h.gameObject.transform.root.GetComponent<TestCarry>();
                if (carry != null)
                {
                    behavior.LoseCarry();
                }
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

    public CharacterState2 GetState()
    {
        return state;
    }

    public void ChangeState(CharacterState2 newState)
    {
        if (state != newState)
        {
            switch (newState)
            {
                case CharacterState2.Ground:
                    if (ActiveToBehavior())
                    {
                        state = CharacterState2.Ground;
                        jumpVelocity = Vector3.up;
                        controller.enabled = false;
                        agent.enabled = true;
                    }
                    break;

                case CharacterState2.Sky:
                    if (ActiveToBehavior())
                    {
                        state = CharacterState2.Sky;
                        landingGround = null;
                        controller.enabled = true;
                        agent.enabled = false;
                        groundNavi.SetGround(landingGround, Vector3.zero);
                    }
                    break;

                case CharacterState2.Hang:
                    if (ActiveToBehavior())
                    {
                        state = CharacterState2.Hang;
                        jumpVelocity = Vector3.zero;
                        controller.enabled = true;
                        agent.enabled = false;
                    }
                    break;

                case CharacterState2.Clamber:
                    if (ActiveToBehavior())
                    {
                        state = CharacterState2.Clamber;
                        controller.enabled = false;
                        agent.enabled = false;
                    }
                    break;

                case CharacterState2.Down:
                    state = CharacterState2.Down;
                    ActiveToDown();
                    controller.enabled = false;
                    agent.enabled = false;
                    break;

                case CharacterState2.Fall:
                    state = CharacterState2.Fall;
                    ActiveToDown();
                    controller.enabled = false;
                    agent.enabled = false;
                    if (groundNavi == null)
                    {
                        GameObject g = new GameObject("navigation_" + behavior.gameObject.name);
                        groundNavi = g.AddComponent<GroundNavigation2>();
                        groundNavi.init(behavior);
                    }
                    if (trackCamera != null)
                    {
                        trackCamera.SetTrackTarget(null);
                    }
                    Damage(StaticParameter.FALL_DAMAGE);
                    break;
            }
        }
    }

    public bool isAlive()
    {
        if (GetState() == CharacterState2.Fall)
        {
            return false;
        }
        if (GetState() == CharacterState2.NoAppear)
        {
            return false;
        }

        return true;
    }

    bool ActiveToBehavior()
    {
        if (isDead())
        {
            return false;
        }

        if (!behavior.isActiveAndEnabled)
        {
            behavior.transform.position = downBehavior.transform.position;
            behavior.transform.rotation =
                Quaternion.LookRotation(Vector3.ProjectOnPlane(downBehavior.transform.forward, Vector3.up));
            jumpVelocity = downBehavior.GetVelocity();

            behavior.gameObject.SetActive(true);
            downBehavior.gameObject.SetActive(false);

            if (trackCamera != null)
            {
                trackCamera.SetTrackTarget(behavior.transform);
            }
        }
        return true;
    }

    void ActiveToDown()
    {
        if (!downBehavior.isActiveAndEnabled)
        {
            downBehavior.transform.position = behavior.transform.position;
            downBehavior.transform.rotation = behavior.transform.rotation;
            downBehavior.SetVelocity(behavior.GetVelocity());

            downBehavior.gameObject.SetActive(true);
            behavior.gameObject.SetActive(false);

            if (trackCamera != null)
            {
                trackCamera.SetTrackTarget(downBehavior.transform);
            }
        }
    }

    public Vector3 GetPosition()
    {
        if (behavior.isActiveAndEnabled)
        {
            return behavior.transform.position;
        }

        if (downBehavior.isActiveAndEnabled)
        {
            return downBehavior.transform.position;
        }

        return Vector3.zero;
    }

    public virtual void SightAction(Collider c)
    {
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void ForceMove(Vector3 vec)
    {
        switch (state)
        {
            case CharacterState2.Ground:
                ChangeState(CharacterState2.Sky);
                jumpVelocity += vec + Vector3.up;
                break;

            case CharacterState2.Down:
                downBehavior.SetVelocity(downBehavior.GetVelocity() + vec);
                break;

            case CharacterState2.Hang:
                ChangeState(CharacterState2.Sky);
                jumpVelocity += vec;
                break;

            case CharacterState2.Sky:
                jumpVelocity += vec;
                break;
        }
    }

    public void Damage(float damVal)
    {
        if (life > 0)
        {
            life -= damVal;
        }

        if (life <= 0 && isAlive())
        {
            ChangeState(CharacterState2.Down);
        }
    }

    public bool isDead()
    {
        return life <= 0;
    }

    void OnDestroy()
    {
        if (behavior != null)
        {
            Destroy(behavior.gameObject);
        }
        if (downBehavior != null)
        {
            Destroy(downBehavior.gameObject);
        }
        if(groundNavi != null)
        {
            Destroy(groundNavi.gameObject);
        }
    }
}
