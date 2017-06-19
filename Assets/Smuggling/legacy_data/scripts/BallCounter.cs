using UnityEngine;
using System.Collections;

namespace HTGames
{
    namespace BallAvoiding
    {
        public class BallCounter : MonoBehaviour
        {
            //必ずBallMakerの子オブジェクトにすること
            BallMaker maker;
            [SerializeField]
            float ballMakeRate = 0.1f;
            [SerializeField]
            float mobMakeRate = 0.01f;
            [SerializeField]
            float coinMakeRate = 0.1f;

            [SerializeField]
            float ballUpperRate = 0.0001f;
            [SerializeField]
            float mobUpperRate = 0.00001f;
            [SerializeField]
            float coinUpperRate = 0.0001f;

            void Start()
            {
                maker = transform.parent.GetComponent<BallMaker>();
            }

            void OnTriggerEnter(Collider c)
            {
                if (c.GetComponent<Bullet>() != null)
                {
                    maker.MakeBall(ballMakeRate);
                    maker.MakeMob(mobMakeRate);
                    maker.MakeCoin(coinMakeRate);
                    if (maker.isPlayerActivation())
                    {
                        ballMakeRate += ballUpperRate;
                        mobMakeRate += mobUpperRate;
                        coinMakeRate += coinUpperRate;
                    }
                }
            }
        }
    }
}