using UnityEngine;
using System.Collections;
using HTGames.System;
using UnityEngine.UI;

namespace HTGames
{
    namespace BattleMain
    {
        //TrackingCameraと同じGameObjectのコンポーネントに追加して使用する
        public class UserController : Controller
        {
            [SerializeField]
            Vector3 spawnPosition;
            [SerializeField]
            GameObject information;
            [SerializeField]
            Text retryInformation;

            TrackingCamera trackingCamera;
            Controlable controlTarget;

            //UserControllerが機能しているかどうか
            bool activate = false;

            float height;

            int characterStock = 3;
            float characterAliveTime;

            void Start()
            {
                trackingCamera = GetComponent<TrackingCamera>();
                if (trackingCamera == null)
                {
                    Debug.LogError("TrackingCamera was not found!!");
                    Destroy(this);
                }
            }

            void Update()
            {
                if (activate)
                {
                    information.SetActive(false);

                    if (character == null)
                    {
                        if (GameSystem.score != null)
                        {
                            Debug.Log("Alive = " + characterAliveTime);
                            GameSystem.score.AddValue(ScoreName.AliveTime, characterAliveTime);
                        }
                        activate = false;
                    }
                    else
                    {
                        trackingCamera.SetTrackTarget(character.GetTargetTransform());
                        if (character.GetTargetCharacter() != null)
                        {
                            trackingCamera.SetTrackAnotherTarget
                                (character.GetTargetCharacter().GetTargetTransform());
                        }
                        else
                        {
                            trackingCamera.SetTrackAnotherTarget(null);
                        }

                        Vector2 moveInput = new Vector2(Input.GetAxis("MoveX"), Input.GetAxis("MoveY"));
                        Vector3 moveVec = trackingCamera.ReviseHorizonnVector(moveInput);
                        character.Move(moveVec);

                        if (Input.GetButton("jump"))
                        {
                            if (height < 1)
                            {
                                height += Time.deltaTime * 2;
                                character.Jump(height);
                            }
                        }
                        else
                        {
                            height = 0;
                            character.Jump(0);
                        }
                        /*
                        if (Input.GetButtonDown("attack"))
                        {
                            character.AttackDown();
                        }
                        if (Input.GetButtonUp("attack"))
                        {
                            character.AttackUp();
                        }
                        
                        if (Input.GetButtonDown("setTarget"))
                        {
                            character.SetTargetCharacter();
                        }
                        */
                        characterAliveTime += Time.deltaTime;
                    }
                }
                else
                {
                    information.SetActive(true);

                    if (character != null)
                    {
                        trackingCamera.SetTrackTarget(character.GetTargetTransform());
                        if (character.GetTargetCharacter() != null)
                        {
                            trackingCamera.SetTrackAnotherTarget
                                (character.GetTargetCharacter().GetTargetTransform());
                        }
                        else
                        {
                            trackingCamera.SetTrackAnotherTarget(null);
                        }
                        activate = true;
                    }
                    else
                    {
                        if (characterStock > 0)
                        {
                            retryInformation.text = "Retry : " + (characterStock - 1);

                            if (Input.GetButtonDown("jump"))
                            {
                                BattleMainManager b = BattleMainManager.Get();
                                b.AddCharacter();
                                Character c = b.GetNotAppearCharacter();
                                c.Appear(spawnPosition, trackingCamera.transform.rotation);
                                c.ControlledByPlayer(true);
                                SetCharacter(c);
                                characterStock -= 1;
                                characterAliveTime = 0;
                            }
                        }
                        else
                        {
                            characterStock -= 1;
                            if (characterStock * Time.deltaTime < -2.5f)
                            {
                                GameSystem gameSystem = GameSystem.GetGameSystem();
                                if (gameSystem != null)
                                {
                                    Debug.Log("AliveTotal = " + GameSystem.score.GetValue(ScoreName.AliveTime));
                                    Debug.Log("CoinTotal = " + GameSystem.score.GetValue(ScoreName.Coin));
                                    gameSystem.LoadScene("Result", UnityEngine.SceneManagement.LoadSceneMode.Single);
                                    characterStock = 3;
                                }
                            }
                        }
                    }
                }
            }

            public bool isActivate()
            {
                return activate;
            }
        }
    }
}
