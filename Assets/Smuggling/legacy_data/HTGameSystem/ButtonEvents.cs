using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace HTGames
{
    namespace System
    {
        //ボタンを押したときの処理を行う
        public class ButtonEvents : MonoBehaviour
        {
            //単純なシーン遷移をする
            public void SceneMove(string sceneName)
            {
                GameSystem.GetGameSystem().LoadScene(sceneName, LoadSceneMode.Single);
            }

            //ゲーム終了
            public void Quit()
            {
                Application.Quit();
            }
        }
    }
}