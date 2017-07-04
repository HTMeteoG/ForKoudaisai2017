using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using koudaigame2017;

namespace EvolutionaryProgramming
{
    public class EP_oldInput : MonoBehaviour
    {
        TestNPC myNPC;
        float findRange = 10f;

        TestNPC targetNPC;
        Vector3 landPoint;

        CommandList commands;
        int commandIndex = 0;
        bool hasCommand = false;
        CommandName comName;
        float comArg;
        float comTime;

        void Start()
        {
            myNPC = GetComponent<TestNPC>();
            //test
            commands = new CommandList(20);
            //test
        }

        void Update()
        {
            if (hasCommand)
            {
                switch (comName)
                {
                    case CommandName.none:
                        comTime += Time.deltaTime;
                        if (comTime >= comArg)
                        {
                            hasCommand = false;
                        }
                        break;
                    case CommandName.run:
                        if (comTime == 0)
                        {
                            Vector3 position = Vector3.zero;
                            if (targetNPC != null)
                            {
                                position = (targetNPC.transform.position - myNPC.transform.position).normalized;
                            }
                            else
                            {
                                position = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                            }
                            position *= comArg - (CommandList.CommandArgMax / 2);
                            myNPC.SetDestination(position);
                        }
                        else if (comTime >= comArg)
                        {
                            hasCommand = false;
                        }
                        comTime += Time.deltaTime;
                        break;
                    case CommandName.jump:
                        if (comTime == 0)
                        {
                            Vector3 vec = Vector3.zero;
                            if (targetNPC != null)
                            {
                                vec = (targetNPC.transform.position - myNPC.transform.position).normalized;
                            }
                            else
                            {
                                vec = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                            }
                            float ratio = comArg / (CommandList.CommandArgMax / 2) - 1;
                            if (ratio <= 0)
                            {
                                vec = -vec;
                                ratio = Mathf.Abs(ratio);
                            }

                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(myNPC.transform.position + vec * ratio, out hit, ratio, NavMesh.AllAreas))
                            {
                                landPoint = hit.position;
                                myNPC.SetJumpFlag();
                            }
                            else
                            {
                                hasCommand = false;
                            }
                        }
                        else
                        {
                            Vector3 remainVec = Vector3.ProjectOnPlane(landPoint - myNPC.transform.position, Vector3.up);
                            float ratio = (comArg - comTime) / comArg;
                            myNPC.SetMoveVelocity(remainVec.normalized, ratio);

                            if (comTime >= comArg)
                            {
                                hasCommand = false;
                            }
                        }
                        comTime += Time.deltaTime;
                        break;
                    case CommandName.attack_landing:
                        if (comTime == 0)
                        {
                            myNPC.SetAttackFlag();
                        }
                        else if (comTime >= comArg)
                        {
                            hasCommand = false;
                        }
                        comTime += Time.deltaTime;
                        break;
                }
            }
            else
            {
                comName = commands.GetName(commandIndex);
                comArg = commands.GetArg(commandIndex);
                comTime = 0;
                hasCommand = true;
                commandIndex++;
                if (commandIndex >= commands.GetCount())
                {
                    commandIndex -= commands.GetCount();
                }

                // Find target
                if (targetNPC == null)
                {
                    Collider[] checker = Physics.OverlapBox(
                        myNPC.transform.position + myNPC.transform.forward * findRange,
                        Vector3.one * findRange * 0.5f, myNPC.transform.rotation,
                        LayerMask.GetMask("BattleLayer"));
                    for (int i = 0; i < checker.Length; i++)
                    {
                        targetNPC = checker[i].GetComponentInParent<TestNPC>();
                        if (targetNPC != null)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}