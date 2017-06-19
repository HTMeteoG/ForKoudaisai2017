using UnityEngine;
using System.Collections.Generic;

namespace HTGames
{
    namespace BattleMain
    {
        public enum BlowType
        {
            None,           //そもそも吹き飛ばない
            Relative,         //このオブジェクトの中心から離れる方向に飛ばす
            LocalStatic,    //ローカル座標単位で方向が固定
            WorldStatic,    //ワールド座標単位で方向が固定

        };

        public class AttackObject : MonoBehaviour
        {
            [SerializeField]
            protected BlowType blowType;
            //方向固定時のみ使用、なおNormalizedされる
            [SerializeField]
            protected Vector3 blowVector;
            [SerializeField]
            protected float blowMagnitude;

            [SerializeField]
            protected AttackData attackData;
            bool activeAttackData = false;

            protected virtual void Start()
            {
                blowVector.Normalize();
                //test
                Activate(true, 0);
                //test
            }

            public void Activate(bool b, int i)
            {
                activeAttackData = b;
                attackData.SetDamageMag(i);
            }

            //これに触れたオブジェクトがこれを呼び出す
            public virtual void Touch(Character c)
            {
                if (activeAttackData)
                {
                    SetBlow(c.GetTargetTransform());
                    c.Damage(attackData);
                }
            }

            void OnCollisionEnter(Collision c)
            {
                Controlable control = c.gameObject.GetComponent<Controlable>();
                if (control != null)
                {
                    Touch(control.GetOwner());
                }
            }

            //BlowTypeに合わせてSetBlowVectorをする
            void SetBlow(Transform t)
            {
                Vector3 vec = Vector3.zero;
                switch (blowType)
                {
                    case BlowType.Relative:
                        vec = (t.position - transform.position).normalized;
                        break;

                    case BlowType.LocalStatic:
                        //vec = transform.
                        break;

                    case BlowType.WorldStatic:
                        vec = blowVector;
                        break;
                }
                attackData.SetBlowVector(vec * blowMagnitude);
            }
        }
    }
}