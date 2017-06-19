using UnityEngine;
using System.Collections.Generic;
using System;

namespace HTGames
{
    namespace System
    {
        public enum StatusName
        {
            Life,
            HP,
            MaxHP,
            JumpTimes,
            MaxJump,
            Invalid,    //無敵時間
            Points,

        };

        [Serializable]
        public class CharacterStatus
        {
            /* SerializeFieldな情報は全て初期値であり、
             * ゲーム中で変化する値(全てfloat型)はDictionaryで保持する
             */
            [SerializeField]
            string name;
            [SerializeField]
            float life;
            [SerializeField]
            List<float> hp = new List<float>();
            [SerializeField]
            int maxJumpTimes;

            Dictionary<StatusName, float> parameter = new Dictionary<StatusName, float>();

            public CharacterStatus()
            {
                //コンストラクタはInspectorの数値が適用されないため、要素を足すだけ
                parameter.Add(StatusName.Life, 0);
                parameter.Add(StatusName.HP, 0);
                parameter.Add(StatusName.MaxHP, 0);
                parameter.Add(StatusName.JumpTimes, 0);
                parameter.Add(StatusName.MaxJump, 0);
                parameter.Add(StatusName.Invalid, 0);
                parameter.Add(StatusName.Points, 0);
            }

            public void ResetAll()
            {
                parameter[StatusName.Life] = life;
                ResetHP();
                parameter[StatusName.MaxJump] = maxJumpTimes;
                parameter[StatusName.JumpTimes] = maxJumpTimes;
            }

            public void ResetHP()
            {
                int hpIndex = (int)parameter[StatusName.Life];
                if (hp.Count <= hpIndex)
                {
                    hpIndex = hp.Count - 1;
                }
                parameter[StatusName.MaxHP] = hp[hpIndex];
                parameter[StatusName.HP] = parameter[StatusName.MaxHP];
            }

            public float GetValue(StatusName name)
            {
                if (parameter.ContainsKey(name))
                {
                    return parameter[name];
                }
                return 0;
            }

            public void SetValue(StatusName name, float val)
            {
                if (parameter.ContainsKey(name))
                {
                    parameter[name] = val;
                }
            }

            public void AddValue(StatusName name, float val)
            {
                SetValue(name, GetValue(name) + val);
            }
        }

        public interface ParameterUI
        {
            void ReflectsData(CharacterStatus cs);
            void ReflectsDeadData();
        }
    }

    namespace BattleMain
    {
        //キャラクターを操作するためのクラス
        public class Controller : MonoBehaviour
        {
            protected Character character;

            protected void SetCharacter(Character c)
            {
                character = c;
            }
        }

        //Controllerで操作できるオブジェクト
        //すなわちCharacterとリンクしているオブジェクト
        public interface Controlable
        {
            Character GetOwner();
        }

        //メソッドBlow(Vector3)で吹き飛ばせるオブジェクト
        public interface Blowable
        {
            void Blow(Vector3 vec);
        }

        public enum AttackType
        {
            Normal,

        };

        //攻撃行為に付与される最低限のデータ
        //これを引数としてダメージを受ける側に渡す
        [Serializable]
        public class AttackData
        {
            //ダメージの基本値
            [SerializeField]
            float damageValue;
            //ダメージ倍率、要素数が0のときは常に1として扱う
            [SerializeField]
            List<float> damageMagList = new List<float>();
            float damageMagnification = 1;

            //攻撃処理スクリプトが設定する値
            Vector3 blowVector = Vector3.zero;

            public virtual AttackType GetAttackType()
            {
                return AttackType.Normal;
            }

            public void SetDamageMag(int i)
            {
                if (i >= 0 && i < damageMagList.Count)
                {
                    damageMagnification = damageMagList[i];
                }
            }

            public void SetBlowVector(Vector3 vec)
            {
                blowVector = vec;
            }

            public float GetDamage()
            {
                return damageValue * damageMagnification;
            }

            public Vector3 GetBlowVector()
            {
                return blowVector;
            }
        }
    }
}