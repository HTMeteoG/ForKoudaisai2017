using UnityEngine;
using System.Collections.Generic;
namespace HTGames
{
    namespace BattleMain
    {
        //Characterと同じGameObjectのコンポーネントに追加して使用する
        public class MobController : Controller
        {
            MobMoveCommandList commandList;
            MobMoveCommand command;
            float nextTime;

            float height;

            void Start()
            {
                SetCharacter(GetComponent<Character>());
                if (character == null)
                {
                    Debug.LogError("Character was not found!!");
                    Destroy(this);
                }
                commandList = new MobMoveCommandList();
            }

            void Update()
            {
                if (character == null)
                {
                    Destroy(this);
                }

                if (command == null)
                {
                    nextTime -= Time.deltaTime;
                    if (nextTime < 0)
                    {
                        nextTime = 0;
                        height = 0;
                        command = commandList.GetCommand();
                    }
                }
                else
                {
                    character.Move(command.moveHorizon);

                    if (height < command.height)
                    {
                        height += Time.deltaTime * 2;
                        character.Jump(height);
                    }
                    else
                    {
                        character.Jump(0);
                    }

                    nextTime += Time.deltaTime;
                    if (nextTime > command.moveTime)
                    {
                        nextTime -= command.moveTime * Random.Range(-0.1f, 1f);
                        command = null;
                    }
                }
            }
        }

        class MobMoveCommand
        {
            public Vector3 moveHorizon;
            public float height;
            public float moveTime;

            public MobMoveCommand(Vector3 mh, float h, float mt)
            {
                moveHorizon = mh;
                height = h;
                moveTime = mt;
            }

            public static MobMoveCommand CreateRandom()
            {
                List<float> rand = new List<float>();
                for (int i = 0; i < 4; i++)
                {
                    rand.Add(Random.Range(-10, 10));
                }
                return new MobMoveCommand(new Vector3(rand[0], 0, rand[1]).normalized,
                   Mathf.Max(0, rand[2]) * 0.1f, (rand[3] + 20) * 0.1f);
            }
        }

        class MobMoveCommandList
        {
            List<MobMoveCommand> command = new List<MobMoveCommand>();

            public MobMoveCommandList()
            {
                int commandNum = Random.Range(1, 20);
                for (int i = 0; i < commandNum; i++)
                {
                    command.Add(MobMoveCommand.CreateRandom());
                }
            }

            public MobMoveCommand GetCommand()
            {
                MobMoveCommand get = command[0];
                command.RemoveAt(0);
                command.Add(get);
                return get;
            }
        }
    }
}
