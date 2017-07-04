using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolutionaryProgramming
{
    public class EPManager : MonoBehaviour
    {

        /* 進化的プログラミングの流れ(参考文献：Wikipediaの記事[進化的プログラミング])
         * ランダムな初期値を持った N 個の個体を用意する
         * N 個の個体のコピーを作る
         * コピーによって生まれた N 個の個体に†正規乱数†を加える
         * コピー元のN個と乱数を加えたN個を混ぜた 2N 個の個体に対して評価値を求める
         * 　具体的には、自分以外をq個選んで、自分より成績が悪い個体の数を求める
         * 評価値が低い順にN個削除する
         * 「コピーを作る」に戻る、以下繰り返し
         */

        int N = 10; //個体の個数(コピーで増えた分を除く)
        int q = 10; //個体評価の際に用いるデータの個数

        //個体の格納場所
        List<CommandList> commands = new List<CommandList>();

        bool autoProcess = false; //自動制御用

        void Start()
        {

        }

        void Update()
        {

        }
    }
}