using UnityEngine;
using System.Collections;

public class Player : Character2
{
    public override void Start()
    {
        base.Start();
        trackCamera = FindObjectOfType<TrackingCamera2>();
    }

    void Update()
    {
        //AutoAppear
        if (GetState() == CharacterState2.NoAppear)
        {
            float dline = Mathf.Min(StaticParameter.FALL, StaticParameter.FALL_NUTRAL);
            Appear(Vector3.up * (dline + 5), Vector3.zero);
        }

        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        jump = Input.GetAxis("Jump");
        fire1 = Input.GetButtonDown("Fire1");

        //test
        //スタンやポーズを使わないならUpdateに直接記述で十分だが・・・
        UpdateAction();
        //test
    }

    public void UpdateAction()
    {
        Vector3 move;
        if (trackCamera != null)
        {
            move = trackCamera.ReviseHorizonVector(new Vector2(moveX, moveY));
        }
        else
        {
            move = new Vector3(moveX, 0, moveY);
        }
        float inputSpeed = Mathf.Min(1, move.magnitude);
        switch (GetState())
        {
            case CharacterState2.Ground:
                groundNavi.SetGround(landingGround, behavior.transform.position + Vector3.down * agent.baseOffset);

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
                    jumpVelocity = new Vector3(agent.velocity.x * 0.5f, jumpPower, agent.velocity.z * 0.5f);
                    move += jumpVelocity;
                    ChangeState(CharacterState2.Sky);
                    behavior.LoseCarry();
                    controller.Move(move * Time.deltaTime);
                }
                break;

            case CharacterState2.Sky:
                move = move * speed * inputSpeed;
                move += jumpVelocity;
                jumpVelocity += Vector3.up * fallAcceleration * Time.deltaTime;
                controller.Move(move * Time.deltaTime);
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
                controller.Move(Vector3.zero);
                break;

            case CharacterState2.Clamber:
                move = Vector3.zero;
                Vector3 remainVec = groundNavi.transform.position + Vector3.up * controller.height * 0.5f - behavior.transform.position;
                behavior.transform.position += remainVec * Mathf.Min(1, (Time.deltaTime / (clamberTime - clamber)));
                clamber += Time.deltaTime;
                if (clamber >= clamberTime)
                {
                    behavior.transform.position = groundNavi.transform.position + Vector3.up * controller.height * 0.5f;
                    ChangeState(CharacterState2.Ground);
                }
                break;

            case CharacterState2.Down:
                if (jump == 1)
                {
                    ChangeState(CharacterState2.Sky);
                }
                break;

            case CharacterState2.Fall:
                falledTime += Time.deltaTime;
                if (falledTime > StaticParameter.DISAPPEAR_TIME)
                {
                    falledTime = 0;
                    downBehavior.gameObject.SetActive(false);
                    state = CharacterState2.NoAppear;
                }
                break;
        }

        if (fire1)
        {
            behavior.Catch();
        }

        //test
        if (Input.GetButtonDown("Fire2"))
        {
            if (GetState() == CharacterState2.Down)
            {
                ChangeState(CharacterState2.Sky);
            }
            else
            {
                ChangeState(CharacterState2.Down);
            }
        }
        //test
    }
}
