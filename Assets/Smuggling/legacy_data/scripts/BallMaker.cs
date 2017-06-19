using UnityEngine;
using System.Collections;
using HTGames.BattleMain;

[System.Serializable]
public class Vector3MinMax
{
    public Vector3 min;
    public Vector3 max;

    public Vector3 RandomVector()
    {
        Vector3 vec = Vector3.zero;
        vec += Vector3.right * Random.Range(min.x, max.x);
        vec += Vector3.up * Random.Range(min.y, max.y);
        vec += Vector3.forward * Random.Range(min.z, max.z);
        return vec;
    }
}

namespace HTGames
{
    namespace BallAvoiding
    {
        public class BallMaker : MonoBehaviour
        {
            [SerializeField]
            GameObject ball;
            [SerializeField]
            Vector3MinMax ballPosition;
            [SerializeField]
            Vector3MinMax ballVelocity;
            [SerializeField]
            float firstBallNum = 10f;
            float ballStock = 0;

            [SerializeField]
            int mobIndex;
            [SerializeField]
            Vector3MinMax mobPosition;
            [SerializeField]
            float firstMobNum = 10f;
            float mobStock = 0;

            [SerializeField]
            GameObject coin;
            [SerializeField]
            Vector3MinMax coinPosition;
            [SerializeField]
            Vector3MinMax coinVelocity;
            float coinStock = 0;

            [SerializeField]
            float MakeTime = 1f;
            float waitTime = 0;

            UserController userController;
            bool playerActivation = false;

            // Use this for initialization
            void Start()
            {
                MakeBall(firstBallNum);
                MakeMob(firstMobNum);

                userController = GameObject.Find("TrackingCamera").GetComponent<UserController>();
            }

            // Update is called once per frame
            void Update()
            {
                if (userController != null)
                {
                    playerActivation = userController.isActivate();
                }

                if (Mathf.Max(ballStock, mobStock, coinStock) > 1f)
                {
                    waitTime += Time.deltaTime * Mathf.Max(1, ballStock / 25f);
                    if (waitTime > MakeTime)
                    {
                        waitTime -= MakeTime;
                        if (ballStock > 1f)
                        {
                            MakeBall();
                        }
                        if (mobStock > 1f)
                        {
                            MakeMob();
                        }
                        if (coinStock > 1f)
                        {
                            MakeCoin();
                        }
                    }
                }
            }

            public void MakeBall(float num)
            {
                ballStock += num;
            }

            void MakeBall()
            {
                GameObject newBall = Instantiate(ball);
                newBall.transform.position = transform.position + ballPosition.RandomVector();
                Rigidbody newBallR = newBall.GetComponent<Rigidbody>();
                if (newBallR != null)
                {
                    newBallR.velocity = ballVelocity.RandomVector();
                }
                ballStock -= 1;
            }

            public void MakeMob(float num)
            {
                mobStock += num;
            }

            void MakeMob()
            {
                BattleMainManager b = BattleMainManager.Get();
                b.AddCharacter(mobIndex);
                Character newMob = b.GetNotAppearCharacter();
                newMob.gameObject.AddComponent<MobController>();
                newMob.Appear(transform.position + mobPosition.RandomVector(), Quaternion.identity);
                int point = Random.Range(0, 100);
                newMob.GetCharacterStatus().AddValue(System.StatusName.Points, point);
                mobStock -= 1;
            }

            public void MakeCoin(float num)
            {
                coinStock += num;
            }

            void MakeCoin()
            {
                GameObject newCoin = Instantiate(coin);
                newCoin.transform.position = transform.position + coinPosition.RandomVector();
                Rigidbody newCoinR = newCoin.GetComponent<Rigidbody>();
                if (newCoinR != null)
                {
                    newCoinR.velocity = coinVelocity.RandomVector();
                }
                coinStock -= 1;
            }

            public bool isPlayerActivation()
            {
                return playerActivation;
            }
        }
    }
}