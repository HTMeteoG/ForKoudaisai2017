using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace HTGames.System
{
    public class ParameterValue : MonoBehaviour, ParameterUI
    {
        [SerializeField]
        private Text displayText;

        [SerializeField]
        StatusName valueName;
        [SerializeField]
        string prefix;  //接頭語
        [SerializeField]
        string suffix;  //接尾語

        public void ReflectsData(CharacterStatus cs)
        {
            float val = cs.GetValue(valueName);
            displayText.text = prefix + val.ToString() + suffix;
        }

        public void ReflectsDeadData()
        {
            displayText.text = "";
        }

        void Start()
        {
            PlayerParameter p = transform.parent.GetComponent<PlayerParameter>();
            if (p != null)
            {
                p.AddStatusUI(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}