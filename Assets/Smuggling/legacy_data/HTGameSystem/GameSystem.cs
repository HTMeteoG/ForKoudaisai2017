using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HTGames
{
    namespace System
    {
        //ゲーム全体に関わるメソッドを扱う
        public class GameSystem : MonoBehaviour
        {
            private static GameSystem gameSystem;

            [SerializeField]
            private Image loadingImage;
            private float imageFillAmount;

            private AsyncOperation loadOperation;

            enum LoadingProgress
            {
                None,       //遷移処理なし
                Closing,    //画面の暗転
                Opening,    //画面の明転

            };
            private LoadingProgress loadingScene = LoadingProgress.None;
            private float closeSpeed = 2f;

            public static GameScore score;

            void Start()
            {
                LoadScene("BallAvoiding_Demo", LoadSceneMode.Additive);
                if (gameSystem == null)
                {
                    gameSystem = this;
                    DontDestroyOnLoad(gameObject);

                    loadingImage.fillAmount = 0;
                    Vector2 windowSize = new Vector2(Screen.width, Screen.height);
                    loadingImage.rectTransform.sizeDelta = windowSize;
                    loadingImage.gameObject.SetActive(false);
                    score = new GameScore();
                }
                else
                {
                    Destroy(gameObject);
                }
            }

            void Update()
            {
                //LoadScene処理 BEGIN
                if (loadingScene != LoadingProgress.None)
                {
                    loadingImage.gameObject.SetActive(true);
                    if (loadingScene == LoadingProgress.Closing)
                    {
                        imageFillAmount += Time.deltaTime * closeSpeed;
                        if (imageFillAmount > 1)
                        {
                            imageFillAmount = 1;
                            if (loadOperation.progress == 0.9f)
                            {
                                loadOperation.allowSceneActivation = true;
                                loadingScene = LoadingProgress.Opening;
                            }
                        }

                        loadingImage.fillAmount = imageFillAmount;
                    }
                    else if (loadingScene == LoadingProgress.Opening)
                    {
                        imageFillAmount -= Time.deltaTime * closeSpeed;
                        if (imageFillAmount < 0)
                        {
                            imageFillAmount = 0;
                            loadingScene = LoadingProgress.None;
                            loadingImage.gameObject.SetActive(false);
                        }

                        loadingImage.fillAmount = imageFillAmount;
                    }
                }
                //LoadScene処理 END
            }

            public void LoadScene(string sceneName, LoadSceneMode mode)
            {
                if (mode == LoadSceneMode.Single)
                {
                    loadOperation = SceneManager.LoadSceneAsync(sceneName, mode);
                    loadOperation.allowSceneActivation = false;
                    loadingScene = LoadingProgress.Closing;
                }
                else
                {
                    SceneManager.LoadScene(sceneName, mode);
                }
            }

            //このメソッドが返す値はnullになってはいけない
            public static GameSystem GetGameSystem()
            {
                return gameSystem;
            }

            public static bool LoadingComplete()
            {
                return gameSystem.loadingScene == LoadingProgress.None;
            }
        }

        public class GameScore
        {
            Dictionary<ScoreName, float> value = new Dictionary<ScoreName, float>();

            public GameScore()
            {
                value.Add(ScoreName.AliveTime, 0);
                value.Add(ScoreName.Coin, 0);
            }

            public void ResetValue()
            {
                value[ScoreName.AliveTime] = 0;
                value[ScoreName.Coin] = 0;
            }

            public void AddValue(ScoreName name, float val)
            {
                value[name] += val;
            }

            public float GetValue(ScoreName name)
            {
                return value[name];
            }

            public float CalculateScore()
            {
                float score = 0;
                score += GetValue(ScoreName.AliveTime) / 3f;
                score += GetValue(ScoreName.Coin);
                return score;
            }
        }

        public enum ScoreName
        {
            AliveTime,
            Coin,

        }
    }
}
