using UnityEngine;
using System.Collections;

public class NPC_Follow : Character2
{
    [SerializeField]
    Character2 followingCharacter;

    bool stopping;

    void Update()
    {
        //AutoAppear
        if (GetState() == CharacterState2.NoAppear)
        {
            float dline = Mathf.Min(StaticParameter.FALL, StaticParameter.FALL_NUTRAL);
            Vector3 randomPoint = new Vector3(Random.Range(-10, 10), 5, Random.Range(-10, 10));
            Appear(randomPoint + Vector3.up * dline, Vector3.zero);
        }

        //test wakeUp
        else if (GetState() == CharacterState2.Down)
        {
            jump += Time.deltaTime * 0.5f;
        }

        //test
        //スタンやポーズを使わないならUpdateに直接記述で十分だが・・・
        UpdateAction();
        //test
    }

    public void UpdateAction()
    {
        switch (GetState())
        {
            case CharacterState2.Ground:
                RaycastHit[] rayData;
                rayData = Physics.RaycastAll(behavior.transform.position, Vector3.down * (agent.baseOffset + 0.1f));
                for (int i = 0; i < rayData.Length; i++)
                {
                    Ground newG = rayData[i].transform.GetComponent<Ground>();
                    if (newG != null)
                    {
                        landingGround = newG;
                        break;
                    }
                }

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

                if (!agent.hasPath && !agent.pathPending)
                {
                    agent.ResetPath();
                    agent.stoppingDistance = 0;
                    if (followingCharacter != null && followingCharacter.isAlive())
                    {
                        agent.SetDestination(followingCharacter.GetPosition());
                    }
                    else
                    {
                        agent.SetDestination(Vector3.up * StaticParameter.FLOOR_HEIGHT * 2);
                    }
                    agent.Stop();
                    stopping = true;
                }
                else if (stopping && !agent.pathPending)
                {
                    if (agent.path.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                    {
                        UnityEngine.AI.NavMeshHit point = new UnityEngine.AI.NavMeshHit();
                        Vector3 random = new Vector3(Random.Range(-50f, 50f),
                                                     Random.Range(0f, StaticParameter.FLOOR_HEIGHT),
                                                     Random.Range(-50f, 50f));
                        if (UnityEngine.AI.NavMesh.SamplePosition(random + Vector3.up * transform.position.y, out point,
                                                   StaticParameter.FLOOR_HEIGHT, UnityEngine.AI.NavMesh.AllAreas))
                        {
                            agent.SetDestination(point.position);
                        }
                    }
                    else
                    {
                        agent.Resume();
                        stopping = false;
                    }

                }
                else if (agent.velocity.magnitude < agent.speed * 0.1f)
                {
                    agent.stoppingDistance += Time.deltaTime * 1f;
                    if (agent.stoppingDistance > 10)
                    {
                        agent.ResetPath();
                        jumpVelocity = Vector3.up * jumpPower;
                        ChangeState(CharacterState2.Sky);
                        behavior.LoseCarry();
                        controller.Move(jumpVelocity * Time.deltaTime);
                    }
                }

                //Jump Action
                if (agent.isOnOffMeshLink)
                {
                    UnityEngine.AI.OffMeshLinkData link = agent.currentOffMeshLinkData;
                    Vector3 linkVec = link.endPos - link.startPos;
                    linkVec = Vector3.ProjectOnPlane(linkVec, Vector3.up).normalized * agent.speed * 0.5f;
                    jumpVelocity = linkVec + Vector3.up * jumpPower;

                    ChangeState(CharacterState2.Sky);
                    behavior.LoseCarry();
                    controller.Move(jumpVelocity * Time.deltaTime);
                }

                break;

            case CharacterState2.Sky:
                //No Moving
                jumpVelocity += Vector3.up * fallAcceleration * Time.deltaTime;
                controller.Move(jumpVelocity * Time.deltaTime);
                break;

            case CharacterState2.Hang:
                clamber = 0;
                if (true)
                {
                    ChangeState(CharacterState2.Clamber);
                }
                controller.Move(Vector3.zero);
                break;

            case CharacterState2.Clamber:
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
                if (jump > 1)
                {
                    ChangeState(CharacterState2.Sky);
                    jump = 0;
                }
                followingCharacter = null;
                break;

            case CharacterState2.Fall:
                falledTime += Time.deltaTime;
                if (falledTime > StaticParameter.DISAPPEAR_TIME)
                {
                    falledTime = 0;
                    downBehavior.gameObject.SetActive(false);
                    state = CharacterState2.NoAppear;
                }
                followingCharacter = null;
                break;
        }
    }

    public override void SightAction(Collider c)
    {
        if (followingCharacter == null)
        {
            Character2Behavior behavior = c.GetComponent<Character2Behavior>();
            if (behavior != null)
            {
                followingCharacter = behavior.GetAttribution();
                if (state == CharacterState2.Ground)
                {
                    agent.ResetPath();
                }
            }
        }
    }
}
