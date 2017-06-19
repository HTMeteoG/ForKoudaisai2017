using UnityEngine;
using System.Collections;
using System;

namespace HTGames
{
    namespace BattleMain
    {
        public class CharacterBehavior : MonoBehaviour, Controlable
        {
            Character character;

            CharacterController characterController;

            /*ここから地上と空中の移行処理関係*/
            //落下速度(地上では一定値をとる)
            float gravityMagnitude;
            const float gravityMagnitude_def = 1f;
            //groundTime_def以上の時間床に接地してないと落下(空中処理へ)
            float groundTime;
            const float groundTime_def = 0.1f;
            //慣性(もしかしたらCharacterで処理した方がよい？)
            Vector3 inertia = Vector3.zero;
            //地上移動時のY軸ベクトル
            Vector3 groundNormal;

            public void SetCharacter(Character c)
            {
                character = c;
            }

            void Awake()
            {
                characterController = GetComponent<CharacterController>();
                if (characterController == null)
                {
                    characterController = gameObject.AddComponent<CharacterController>();
                }
                gameObject.layer = LayerMask.NameToLayer("Hitter");
            }

            void Start()
            {

            }

            void Update()
            {
                Vector3 moves = Vector3.down * gravityMagnitude + inertia;
                switch (character.GetCharacterState())
                {
                    case CharacterState.Ground:
                        characterController.Move(moves * Time.deltaTime);
                        gravityMagnitude = gravityMagnitude_def;

                        groundTime -= Time.deltaTime;
                        if (groundTime < 0)
                        {
                            character.SetCharacterState(CharacterState.Sky);
                        }
                        break;

                    case CharacterState.Sky:
                        characterController.Move(moves * Time.deltaTime);
                        gravityMagnitude += 9.8f * Time.deltaTime;
                        break;
                }

                //testing(落死処理)
                if (transform.position.y < -10)
                {
                    character.Delete();
                }
                //testing
            }

            void OnControllerColliderHit(ControllerColliderHit hit)
            {
                if (hit.gameObject.tag.CompareTo(TagName.Field) == 0)
                {
                    groundNormal = hit.normal;
                    groundTime = groundTime_def;
                    character.SetCharacterState(CharacterState.Ground);
                }
                else
                {
                    AttackObject attack = hit.gameObject.GetComponent<AttackObject>();
                    if (attack != null)
                    {
                        attack.Touch(GetOwner());
                    }

                    Coin coin = hit.gameObject.GetComponent<Coin>();
                    if(coin != null)
                    {
                        coin.Catched(GetOwner());
                    }

                    Blowable blowTarget = hit.gameObject.GetComponent<Blowable>();
                    if (blowTarget != null)
                    {
                        blowTarget.Blow(hit.normal);
                    }
                }
            }

            public void Move(Vector3 vec)
            {
                switch (character.GetCharacterState())
                {
                    case CharacterState.Ground:
                        float vecMag = vec.magnitude;
                        Vector3 moveVec = Vector3.ProjectOnPlane(vec, groundNormal).normalized * vecMag;
                        characterController.Move(moveVec);
                        break;
                    case CharacterState.Sky:
                        characterController.Move(vec);
                        break;
                }
            }

            public void Rotate(Vector3 vec)
            {
                if(character.GetCharacterState() == CharacterState.Ground)
                {
                    vec = Vector3.ProjectOnPlane(vec, groundNormal);
                }

                if (vec.sqrMagnitude > 0.01)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(vec);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2);
                }
            }

            public void Jump(float height)
            {
                if (height > 0 && height < 1)
                {
                    gravityMagnitude = -5;//ジャンプ性能関数、現在は定数仮置き
                }
            }

            public void Down()
            {
                gravityMagnitude = 0;
                inertia = Vector3.zero;
            }

            //現在未使用
            public void Blow(Vector3 vec)
            {
                inertia += vec;
            }

            public Character GetOwner()
            {
                return character;
            }
        }
    }
}
