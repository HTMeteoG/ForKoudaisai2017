using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using koudaigame2017;

namespace EvolutionaryProgramming
{
    //CommandListに従ってTestNPCを操作するComponent
    public class EP_Input : MonoBehaviour
    {
        EPCharacter myNPC;
        float findRange = 10f;

        Vector3 landPoint;

        CommandList commands;
        int commandIndex = 0;
        bool hasCommand = false;
        CommandName comName;
        float comArg;
        float comTime;

        void Start()
        {
            myNPC = GetComponent<EPCharacter>();
            //test
            commands = new CommandList(20);
            //test
        }

        public void GetCommands(CommandList c)
        {
            commands = c;
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
                            NextCommand();
                        }
                        break;

                    case CommandName.run:
                        if (comTime == 0)
                        {
                            Vector3 position = Vector3.zero;
                            if (myNPC.GetTarget() != null)
                            {
                                position = (myNPC.GetTarget().transform.position - myNPC.transform.position).normalized;
                            }
                            else
                            {
                                position = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                            }
                            position *= comArg - (CommandList.CommandArgMax / 2);
                            myNPC.SetDestination(position);
                        }
                        comTime += Time.deltaTime;
                        break;
                    case CommandName.jump:
                        if (comTime == 0)
                        {
                            Debug.Log("jump");

                            Vector3 vec = Vector3.zero;
                            if (myNPC.GetTarget() != null)
                            {
                                vec = (myNPC.GetTarget().transform.position - myNPC.transform.position).normalized;
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

                            Vector3 Xaxis = myNPC.GetXaxis();
                            if (Xaxis != Vector3.zero)
                            {
                                vec = Vector3.Project(vec, Xaxis).normalized;
                            }

                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(myNPC.transform.position + vec * ratio, out hit, ratio, NavMesh.AllAreas))
                            {
                                landPoint = hit.position;
                                myNPC.SetMoveVelocity(vec, ratio);
                                myNPC.SetJumpFlag();
                            }
                            else
                            {
                                Debug.Log("Reject jump");
                                NextCommand();
                            }
                        }
                        else
                        {
                            //Vector3 remainVec = Vector3.ProjectOnPlane(landPoint - myNPC.transform.position, Vector3.up);
                            //float ratio = (comArg - comTime) / comArg;
                            //myNPC.SetMoveVelocity(remainVec.normalized, ratio);
                        }
                        comTime += Time.deltaTime;
                        break;
                    case CommandName.attack_landing:
                        if (comTime == 0)
                        {
                            myNPC.SetAttackFlag();
                        }
                        comTime += Time.deltaTime;
                        break;
                }
            }
            else
            {
                comTime += Time.deltaTime;
            }
        }

        public float GetcomTime()
        {
            return comTime;
        }

        public void NextCommand()
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

            myNPC.FindTarget(findRange);
        }
    }

    //命令列、EPにおける"個体"
    public class CommandList
    {
        static int CommandNameNum = 4;
        public static float CommandArgMax = 10f;

        List<CommandName> commandName = new List<CommandName>();
        List<float> commandArg = new List<float>();

        //ランダム生成用
        public CommandList(int commandNum)
        {
            for (int i = 0; i < commandNum; i++)
            {
                //初期化時に限り、Noneは使用しないことにした
                CommandName newCommandN = (CommandName)Random.Range(1, CommandNameNum);
                commandName.Add(newCommandN);
                commandArg.Add(Random.Range(0f, CommandArgMax));
            }
        }

        public int GetCount()
        {
            return commandName.Count;
        }

        public CommandName GetName(int index)
        {
            int i = index % commandName.Count;
            return commandName[i];
        }

        public float GetArg(int index)
        {
            int i = index % commandArg.Count;
            return commandArg[i];
        }

        //ロード用
        public CommandList(string commandDataStr)
        {
            string[] commandStrings = commandDataStr.Split('|');
            for (int i = 0; i < commandStrings.Length; i++)
            {
                string[] commandStr = commandStrings[i].Split(',');
                commandName.Add((CommandName)int.Parse(commandStr[0]));
                commandArg.Add(float.Parse(commandStr[1]));
            }
        }

        //セーブ用
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < commandName.Count; i++)
            {
                str += ((int)commandName[i]).ToString() + "," + commandArg[i].ToString() + "|";
            }
            return str;
        }
    }

    public enum CommandName
    {
        none = 0,
        run = 1,
        jump = 2,
        attack_landing
    }
}