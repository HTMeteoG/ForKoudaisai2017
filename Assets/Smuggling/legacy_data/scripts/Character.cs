using UnityEngine;
using System.Collections.Generic;
using HTGames.System;

namespace HTGames
{
    namespace BattleMain
    {
        public enum CharacterState
        {
            NotAppear,  //未出現
            Ground,     //地上
            Sky,        //空中
            Down,       //ダウン
            KnockOut,   //ノックアウト(体力ゼロ、自動復活可能)
            Death,      //戦闘不能(体力ゼロ、自動復活不能)
        }

        public class Character : MonoBehaviour, Controlable
        {
            [SerializeField]
            GameObject characterBehavior;
            [SerializeField]
            GameObject characterRagdoll;
            [SerializeField]
            CharacterStatus status;

            //自分自身のインスタンス
            CharacterBehavior behaviorInstance;
            CharacterRagdoll ragdollInstance;

            //状態
            CharacterState characterState = CharacterState.NotAppear;
            bool isDown = false;
            bool isControlledByPlayer = false;

            //攻撃目標
            Character targetCharacter = null;

            float lastJumpInput;
            [SerializeField]
            GameObject bullet;
            bool chargeing;
            float bulletPower;
            float maxBulletPower = 50f;

            float rewardPoint;

            public Transform GetTargetTransform()
            {
                if (characterState == CharacterState.NotAppear)
                {
                    return null;
                }

                if (isDown)
                {
                    return ragdollInstance.transform;
                }
                else
                {
                    return behaviorInstance.transform;
                }
            }

            public void Appear(Vector3 appearPosition, Quaternion appearRotation)
            {
                if (characterState == CharacterState.NotAppear)
                {
                    //出現処理
                    GameObject CB_Instance = Instantiate<GameObject>(characterBehavior);
                    CB_Instance.transform.position = appearPosition;
                    CB_Instance.transform.rotation = appearRotation;

                    behaviorInstance = CB_Instance.GetComponent<CharacterBehavior>();
                    behaviorInstance.SetCharacter(this);

                    characterState = CharacterState.Sky;
                    status.ResetAll();
                    rewardPoint = 0;
                }
            }

            public void ControlledByPlayer(bool b)
            {
                isControlledByPlayer = b;
            }

            void Update()
            {
                if (chargeing && bulletPower < maxBulletPower)
                {
                    bulletPower += Time.deltaTime * 10f;
                }

                if (status.GetValue(StatusName.Invalid) > 0)
                {
                    status.AddValue(StatusName.Invalid, -Time.deltaTime);
                }

                if (targetCharacter != null && targetCharacter.GetCharacterState() == CharacterState.Death)
                {
                    SetTargetCharacter(null);
                }
            }

            public CharacterState GetCharacterState()
            {
                return characterState;
            }

            public void SetCharacterState(CharacterState cs)
            {
                switch (cs)
                {
                    case CharacterState.Ground:
                        Behavior();
                        Ground();
                        break;

                    case CharacterState.Sky:
                        Behavior();
                        Sky();
                        break;

                    case CharacterState.Down:
                        Ragdoll();
                        Down();
                        break;

                    case CharacterState.KnockOut:
                        Ragdoll();
                        KnockOut();
                        break;

                    case CharacterState.Death:
                        Ragdoll();
                        Death();
                        break;
                }
            }

            public CharacterStatus GetCharacterStatus()
            {
                return status;
            }

            void Ground()
            {
                if (characterState != CharacterState.Ground)
                {
                    characterState = CharacterState.Ground;
                    //各種処理
                }
            }

            void Sky()
            {
                if (characterState != CharacterState.Sky)
                {
                    characterState = CharacterState.Sky;
                    //各種処理
                    status.SetValue(StatusName.JumpTimes, status.GetValue(StatusName.MaxJump));
                }
            }

            void Down()
            {
                if (characterState != CharacterState.Down)
                {
                    characterState = CharacterState.Down;
                    //各種処理
                }
            }

            void KnockOut()
            {
                if (characterState != CharacterState.KnockOut)
                {
                    characterState = CharacterState.KnockOut;
                    //各種処理
                }
            }

            void Death()
            {
                if (characterState != CharacterState.Death)
                {
                    characterState = CharacterState.Death;

                    //スコアを記録
                    if (isControlledByPlayer)
                    {
                        if(GameSystem.score != null){
                            Debug.Log("Coin = " + status.GetValue(StatusName.Points));
                            GameSystem.score.AddValue(ScoreName.Coin, status.GetValue(StatusName.Points));
                        }
                    }
                    //持ち点を全放出
                    int lostPoint = (int)status.GetValue(StatusName.Points);
                    BattleMainManager.Get().AddCoinEmitter(
                        GetTargetTransform().position, lostPoint);
                    status.AddValue(StatusName.Points, -lostPoint);
                }
                ragdollInstance.Broken();
                BattleMainManager.Get().RemoveCharacter(this);
            }

            public void Delete()
            {
                if (ragdollInstance != null)
                {
                    Destroy(ragdollInstance.gameObject);
                }
                Destroy(behaviorInstance.gameObject);
                Destroy(gameObject);
            }

            void Behavior()
            {
                if (isDown)
                {
                    behaviorInstance.transform.position = ragdollInstance.transform.position;
                    behaviorInstance.transform.rotation = Quaternion.identity;
                    behaviorInstance.gameObject.SetActive(true);
                    Destroy(ragdollInstance.gameObject);

                    isDown = false;
                }
            }

            void Ragdoll()
            {
                if (!isDown)
                {
                    behaviorInstance.Down();
                    behaviorInstance.gameObject.SetActive(false);
                    ragdollInstance = Instantiate<GameObject>(characterRagdoll).GetComponent<CharacterRagdoll>();
                    ragdollInstance.SetCharacter(this);
                    ragdollInstance.transform.position = behaviorInstance.transform.position;
                    ragdollInstance.transform.rotation = behaviorInstance.transform.rotation;

                    isDown = true;
                }
            }

            public void Move(Vector3 vec)
            {
                if (behaviorInstance.isActiveAndEnabled)
                {
                    const float speed_test = 4f;
                    behaviorInstance.Move(vec * speed_test * Time.deltaTime);
                    if (targetCharacter != null)
                    {
                        behaviorInstance.Rotate(GetTargetVelocity());
                    }
                    else if (characterState == CharacterState.Ground)
                    {
                        behaviorInstance.Rotate(vec);
                    }
                }
            }

            public void Jump(float input)
            {
                if (behaviorInstance.isActiveAndEnabled)
                {
                    if (status.GetValue(StatusName.JumpTimes) > 0 ||
                        characterState == CharacterState.Ground)
                    {
                        if (input > 0 && input < 1)
                        {
                            behaviorInstance.Jump(input);
                            if (lastJumpInput == 0)
                            {
                                status.AddValue(StatusName.JumpTimes, -1);
                            }
                        }
                        lastJumpInput = input;
                    }
                }
                else if (input > 0 && isDown)
                {
                    Wake();
                }
            }

            //attack処理は「押したとき」と「離したとき」に分かれる
            //押したとき
            public void AttackDown()
            {
                bulletPower = 10;
                if (!isDown)
                {
                    chargeing = true;
                }
                else
                {
                    Wake();
                }
            }
            //離したとき
            public void AttackUp()
            {
                if (!isDown && chargeing)
                {
                    Transform pos = GetTargetTransform();
                    GameObject shot = Instantiate(bullet);
                    shot.transform.position = pos.position + pos.forward;
                    Bullet b = shot.GetComponent<Bullet>();
                    b.Blow(pos.forward * bulletPower, this);
                }
                chargeing = false;
            }

            //半径rangeの中で最も近いCharacterを返す
            public Character FindTarget(float range)
            {
                if (!isDown)
                {
                    Controlable targetCandidate = null;
                    float sqrTargetLength = Mathf.Infinity;

                    Collider[] finds =
                        Physics.OverlapSphere(behaviorInstance.transform.position, range);
                    for (int i = 0; i < finds.Length; i++)
                    {
                        if (finds[i].gameObject.layer == LayerMask.NameToLayer("Hitter"))
                        {
                            Controlable controledObject = finds[i].GetComponent<Controlable>();
                            if (controledObject != null && controledObject != behaviorInstance)
                            {
                                float length = (finds[i].transform.position
                                    - GetTargetTransform().position).sqrMagnitude;
                                if (length < sqrTargetLength)
                                {
                                    targetCandidate = controledObject;
                                    sqrTargetLength = length;
                                }
                            }
                        }
                    }

                    if (targetCandidate != null)
                    {
                        return targetCandidate.GetOwner();
                    }
                }
                return null;
            }

            public void SetTargetCharacter()
            {
                if (targetCharacter == null)
                {
                    targetCharacter = FindTarget(10f);
                }
                else
                {
                    targetCharacter = null;
                }
            }

            public void SetTargetCharacter(Character c)
            {
                targetCharacter = c;
            }

            public Character GetTargetCharacter()
            {
                return targetCharacter;
            }

            public Vector3 GetTargetVelocity()
            {
                if (targetCharacter != null)
                {
                    Vector3 targetVelocity = targetCharacter.GetTargetTransform().position;
                    targetVelocity -= GetTargetTransform().position;
                    return targetVelocity;
                }
                return Vector3.zero;
            }

            public void Damage(AttackData attackData)
            {
                if (status.GetValue(StatusName.Invalid) <= 0)
                {
                    //各種ダメージ処理
                    status.AddValue(StatusName.HP, -attackData.GetDamage());

                    if (status.GetValue(StatusName.HP) <= 0)
                    {
                        if (status.GetValue(StatusName.Life) <= 0)
                        {
                            SetCharacterState(CharacterState.Death);
                        }
                        else
                        {
                            SetCharacterState(CharacterState.KnockOut);
                        }
                    }
                    else
                    {
                        SetCharacterState(CharacterState.Down);
                    }

                    Blow(attackData);
                    status.AddValue(StatusName.Invalid, 0.1f);
                }
            }

            //吹き飛び処理
            public void Blow(AttackData a)
            {
                Vector3 vec = a.GetBlowVector();
                if (vec.sqrMagnitude > 0.9f)
                {
                    if (isDown)
                    {
                        ragdollInstance.Blow(vec);
                    }
                    else
                    {
                        behaviorInstance.Blow(vec);
                    }
                }
            }

            public void Wake()
            {
                if (isDown)
                {
                    if (characterState == CharacterState.Down)
                    {
                        SetCharacterState(CharacterState.Sky);
                    }
                    else if (status.GetValue(StatusName.Life) > 0)
                    {
                        status.AddValue(StatusName.Life, -1);
                        status.ResetHP();
                        SetCharacterState(CharacterState.Sky);
                    }
                }
            }

            public void AddReward(float val)
            {
                rewardPoint += val;
            }

            public float GetReward()
            {
                return rewardPoint;
            }

            public Character GetOwner()
            {
                return this;
            }
        }
    }
}