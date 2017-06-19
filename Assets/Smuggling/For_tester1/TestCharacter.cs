using UnityEngine;
using System.Collections;

public class TestCharacter : MonoBehaviour
{
    CharacterController characterController;
    CharacterState state;
    float speed = 4f;
    float rotateSpeed = 100f;
    float carryingSpeed = 2f;
    float carryingRotateSpeed = 25f;
    float power = 2f;
    float fallAcceleration = -9.8f;
    float jumpPower = 10f;
    float fallWaitTime = 0.1f;
    float clamberTime = 0.5f;

    Ground landingGround;
    GroundNavigation groundNavi;
    Vector3 jumpVelocity = Vector3.zero;
    float fallWait;
    Vector3 hangPosition;
    float clamber;
    TestCarry grabbingCarry;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        GameObject g = new GameObject("navigation_" + gameObject.name);
        groundNavi = g.AddComponent<GroundNavigation>();
        groundNavi.init(this);

        state = CharacterState.Sky;
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
            case CharacterState.Ground:
                Vector3 groundNormal = Vector3.up;
                if (landingGround != null)
                {
                    groundNormal = landingGround.getGroundNormal();
                }

                move = Vector3.ProjectOnPlane(move, groundNormal).normalized * inputSpeed;
                if (grabbingCarry == null)
                {
                    move *= speed;
                    move -= groundNormal * 0.1f * speed;
                }
                else
                {
                    move *= carryingSpeed;
                    move -= groundNormal * 0.1f * carryingSpeed;
                }

                if (fallWait > fallWaitTime)
                {
                    ChangeState(CharacterState.Sky);
                }
                else
                {
                    fallWait += Time.deltaTime;
                }

                if (jump == 1)
                {
                    jumpVelocity = new Vector3(moveX * 0.5f, jumpPower, moveY * 0.5f);
                    move += jumpVelocity;
                    ChangeState(CharacterState.Sky);
                    LoseCarry();
                }
                break;

            case CharacterState.Sky:
                move = move * speed * inputSpeed;
                move += jumpVelocity;
                jumpVelocity += Vector3.up * fallAcceleration * Time.deltaTime;
                break;

            case CharacterState.Hang:
                clamber = 0;
                if (jump == 1)
                {
                    ChangeState(CharacterState.Clamber);
                }
                else if (Vector3.Dot(move, hangPosition) < 0)
                {
                    ChangeState(CharacterState.Sky);
                }
                move = Vector3.zero;
                break;

            case CharacterState.Clamber:
                move = Vector3.zero;
                Vector3 remainVec = groundNavi.transform.position + Vector3.up * characterController.height * 0.5f - transform.position;
                transform.position += remainVec * Mathf.Min(1, (Time.deltaTime / (clamberTime - clamber)));
                clamber += Time.deltaTime;
                if (clamber >= clamberTime)
                {
                    transform.position = groundNavi.transform.position + Vector3.up * characterController.height * 0.5f;
                    ChangeState(CharacterState.Ground);
                }
                break;
        }

        Move(move);
        if (fire1)
        {
            Catch();
        }

        //Fall test
        if (transform.position.y < -25)
        {
            transform.position = new Vector3(10, 10, 0);
            ChangeState(CharacterState.Sky);
        }
        //Fall test
    }

    public void Move(Vector3 vec)
    {
        switch (state)
        {
            case CharacterState.Ground:
                Vector3 move = Vector3.ProjectOnPlane(vec, transform.right);
                float moveFront = Vector3.Dot(vec, transform.forward);
                float moveRot = Vector3.Dot(vec, transform.right);

                if (grabbingCarry == null)
                {
                    transform.Rotate(transform.up, moveRot * rotateSpeed * Time.deltaTime);
                }
                else if (moveFront >= -0.1f)
                {
                    transform.Rotate(transform.up, moveRot * carryingRotateSpeed * Time.deltaTime);
                }
                else
                {
                    transform.Rotate(transform.up, -moveRot * carryingRotateSpeed * Time.deltaTime);
                }

                characterController.Move(move * Time.deltaTime);
                break;

            case CharacterState.Sky:
                characterController.Move(vec * Time.deltaTime);
                break;
        }
    }

    void Catch()
    {
        if (grabbingCarry == null)
        {
            if (state == CharacterState.Ground)
            {
                Collider[] c = Physics.OverlapBox(
                transform.position + transform.forward * characterController.radius * 2f,
                Vector3.one * characterController.radius,
                transform.rotation);

                CarryGrabPoint carry;
                for (int i = 0; i < c.Length; i++)
                {
                    carry = c[i].gameObject.transform.GetComponent<CarryGrabPoint>();
                    if (carry != null)
                    {
                        if (carry.SetGrabbing(this, characterController.radius))
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
                case CharacterState.Ground:
                    landingGround = g;
                    fallWait = 0;
                    break;

                case CharacterState.Sky:
                    if (localPoint.y <= -characterController.height * 0.4f)
                    {
                        ChangeState(CharacterState.Ground);
                        landingGround = g;
                    }
                    else if (g.isHangable() && localPoint.y >= characterController.height * 0.25f)
                    {
                        hangPosition = localPoint;
                        ChangeState(CharacterState.Hang);
                        groundNavi.SetGround(g, h.point);
                        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(localPoint, Vector3.up));
                    }
                    break;

                case CharacterState.Hang:
                    if (localPoint.y <= -characterController.height * 0.4f)
                    {
                        ChangeState(CharacterState.Ground);
                        landingGround = g;
                    }
                    break;
            }

            if (state == CharacterState.Ground)
            {
                groundNavi.SetGround(landingGround, h.point);
            }
        }

        if (state == CharacterState.Sky)
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

    void ChangeState(CharacterState newState)
    {
        if (state != newState)
        {
            switch (newState)
            {
                case CharacterState.Ground:
                    state = CharacterState.Ground;
                    jumpVelocity = Vector3.down;
                    characterController.enabled = true;
                    fallWait = 0;
                    break;

                case CharacterState.Sky:
                    state = CharacterState.Sky;
                    landingGround = null;
                    characterController.enabled = true;
                    groundNavi.SetGround(landingGround, Vector3.zero);
                    break;

                case CharacterState.Hang:
                    state = CharacterState.Hang;
                    jumpVelocity = Vector3.zero;
                    break;

                case CharacterState.Clamber:
                    state = CharacterState.Clamber;
                    characterController.enabled = false;
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
        //transform.rotation = Quaternion.identity;
    }
}

public enum CharacterState
{
    Ground,
    Sky,
    Hang,
    Clamber,

}