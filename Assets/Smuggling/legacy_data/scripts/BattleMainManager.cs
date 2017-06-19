using UnityEngine;
using System.Collections.Generic;

namespace HTGames
{
    namespace BattleMain
    {
        public class BattleMainManager : MonoBehaviour
        {
            static BattleMainManager battleMainManager;

            //キャラクターリスト
            [SerializeField]
            List<GameObject> appearCharacter = new List<GameObject>();
            //キャラクターのインスタンスリスト
            List<Character> characters = new List<Character>();

            [SerializeField]
            GameObject coinEmitter;

            void Awake()
            {
                if (battleMainManager != null)
                {
                    Destroy(battleMainManager);
                }
                battleMainManager = this;
            }

            //キャラクタの生成(Appearはしない)
            public void AddCharacter(int num)
            {
                if (num < appearCharacter.Count)
                {
                    GameObject newCharacter = Instantiate(appearCharacter[num]);
                    characters.Add(newCharacter.GetComponent<Character>());
                }
            }
            //ランダムなキャラクタの生成
            public void AddCharacter()
            {
                int select = Random.Range(0, appearCharacter.Count);
                AddCharacter(select);
            }

            public void RemoveCharacter(Character c)
            {
                characters.Remove(c);
            }

            //生成済みキャラクタでスポーンしていないものを選択(他スクリプトがAppearする)
            public Character GetNotAppearCharacter()
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].GetCharacterState() == CharacterState.NotAppear)
                    {
                        return characters[i];
                    }
                }
                return null;
            }

            public Character GetAppearCharacter(int num)
            {
                if (CharacterCount() > 0)
                {
                    num = num % CharacterCount();
                    if (characters[num].GetCharacterState() != CharacterState.NotAppear)
                    {
                        return characters[num];
                    }
                }
                return null;
            }

            public Character GetRandomCharacter()
            {
                return GetAppearCharacter(Random.Range(0, CharacterCount() - 1));
            }

            public int CharacterCount()
            {
                characters.RemoveAll(x => x == null);
                return characters.Count;
            }

            public void AddCoinEmitter(Vector3 position, int point)
            {
                CoinEmitter newEmitter = Instantiate(coinEmitter).GetComponent<CoinEmitter>();
                newEmitter.Setting(position, point);
            }

            public static BattleMainManager Get()
            {
                return battleMainManager;
            }
        }

        public class TagName
        {
            public const string Field = "Field";
        }
    }
}