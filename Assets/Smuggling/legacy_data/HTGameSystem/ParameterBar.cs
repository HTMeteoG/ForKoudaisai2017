using System;
using UnityEngine;
using UnityEngine.UI;

namespace HTGames
{
    namespace System
    {
        public class ParameterBar : MonoBehaviour, ParameterUI
        {
            //ゲージの枠
            [SerializeField]
            private Image blank;
            //ゲージ本体
            [SerializeField]
            private Image bar;
            //ゲージ上昇部分
            [SerializeField]
            private Image upper;
            //ゲージ下降部分
            [SerializeField]
            private Image downer;

            private float value;
            [SerializeField]
            private StatusName valueName;

            private float maxValue;
            [SerializeField]
            private StatusName maxValueName;

            private float ratio;//比
            private float oldRatio;
            private float startRatio;

            private float moveUp = 1;
            private float moveDown = 1;
            
            void Awake()
            {
                ratio = bar.fillAmount;
                oldRatio = ratio;
            }

            void Start()
            {
                PlayerParameter p = transform.parent.GetComponent<PlayerParameter>();
                if(p != null)
                {
                    p.AddStatusUI(this);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }

            public void ReflectsData(CharacterStatus cs)
            {
                if (bar != null)
                {
                    bool change = false;
                    if (maxValue != cs.GetValue(maxValueName))
                    {
                        change = true;
                        maxValue = cs.GetValue(maxValueName);
                    }

                    if (value != cs.GetValue(valueName))
                    {
                        change = true;
                        value = cs.GetValue(valueName);
                    }

                    if (change)
                    {
                        ratio = value / maxValue;
                        if (downer != null && ratio < oldRatio)
                        {
                            moveDown = -1;
                            moveUp = 1;
                            startRatio = oldRatio;
                            bar.fillAmount = ratio;
                            upper.fillAmount = ratio;
                        }

                        else if (upper != null && ratio > oldRatio)
                        {
                            moveUp = -1;
                            moveDown = 1;
                            startRatio = oldRatio;
                            upper.fillAmount = ratio;
                            downer.fillAmount = ratio;
                        }
                        else
                        {
                            bar.fillAmount = ratio;
                        }
                    }
                }
            }

            void Update()
            {
                if (moveDown < 1)
                {
                    moveDown += Time.deltaTime;
                    if (moveDown > 0)
                    {
                        oldRatio = startRatio + (ratio - startRatio) * Mathf.Pow(moveDown, 2);
                        downer.fillAmount = oldRatio;
                    }
                }

                if (moveUp < 1)
                {
                    moveUp += Time.deltaTime;
                    if (moveUp > 0)
                    {
                        oldRatio = startRatio + (ratio - startRatio) * Mathf.Pow(moveUp, 2);
                        bar.fillAmount = oldRatio;
                    }
                }
            }

            public void ReflectsDeadData()
            {
                if (bar != null)
                {
                    value = 0;
                    
                    ratio = value / maxValue;
                    if (downer != null && ratio < oldRatio)
                    {
                        moveDown = -1;
                        moveUp = 1;
                        startRatio = oldRatio;
                        bar.fillAmount = ratio;
                        upper.fillAmount = ratio;
                    }

                    else if (upper != null && ratio > oldRatio)
                    {
                        moveUp = -1;
                        moveDown = 1;
                        startRatio = oldRatio;
                        upper.fillAmount = ratio;
                        downer.fillAmount = ratio;
                    }
                }
            }
        }
    }
}
