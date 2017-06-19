using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

namespace HTGames
{
    public class ResultManager : MonoBehaviour
    {
        [SerializeField]
        Text TotalAliveTime;
        [SerializeField]
        Text TotalCoin;
        [SerializeField]
        Text TotalScore;
        [SerializeField]
        Text Ranking;

        List<int> scoreList = new List<int>();

        void Start()
        {
            System.GameScore s = System.GameSystem.score;
            int yourScore = 0;
            if (s != null)
            {
                TotalAliveTime.text = "Total Survival time : " + (int)s.GetValue(System.ScoreName.AliveTime) + " (s)";
                TotalCoin.text = "Total Coin : " + (int)s.GetValue(System.ScoreName.Coin);
                yourScore = (int)s.CalculateScore();
                TotalScore.text = "Score : " + yourScore;
            }

            System.GameSystem.score.ResetValue();

            int rankCount = 0;
            int yourRank = 0;

            //データ読み込み(ランキング高い順にソート済みであることを前提とする)
            string filePath;
            filePath = Path.Combine(Application.streamingAssetsPath, "hiScore.txt");
            if (File.Exists(filePath))
            {
                string[] loadData = File.ReadAllText(filePath).Split(',');
                //記録手法上、ランキングデータの最後尾は必ず空なのでエラー回避
                rankCount = loadData.Length - 1;

                for (int i = 0; i < rankCount; i++)
                {
                    try
                    {
                        scoreList.Add(int.Parse(loadData[i]));
                    }
                    catch (FormatException e)
                    {

                    }
                }
            }

            //ランキング処理
            for (int i = 0; i < scoreList.Count; i++)
            {
                if (yourScore >= scoreList[i])
                {
                    scoreList.Insert(i, yourScore);
                    yourRank = i + 1;
                    break;
                }
            }
            if (yourRank == 0)
            {
                scoreList.Add(yourScore);
                yourRank = scoreList.Count;
            }

            rankCount = scoreList.Count;
            Ranking.text = "これまでの受験者" + rankCount + "人中、" + yourRank + "位の実力です。";

            //データ保存
            int[] saveDataInt = scoreList.ToArray();
            string savedata = "";
            for (int i = 0; i < rankCount; i++)
            {
                savedata += saveDataInt[i] + ",";
            }
            File.WriteAllText(filePath, savedata);
        }
    }
}
