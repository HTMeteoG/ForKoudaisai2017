using UnityEngine;
using System.Collections.Generic;
using HTGames.BattleMain;

namespace HTGames
{
    namespace System
    {
        public class PlayerParameter : MonoBehaviour
        {
            TrackingCamera trackingCamera;
            bool activate;
            Character trackingCharacter;
            CharacterStatus status;
            List<ParameterUI> paraUI = new List<ParameterUI>();

            void Start()
            {
                trackingCamera = transform.parent.GetComponent<TrackingCamera>();
            }

            void Update()
            {
                if (activate)
                {
                    if (trackingCharacter != null)
                    {
                        foreach (ParameterUI p in paraUI)
                        {
                            p.ReflectsData(status);
                        }
                    }
                    else
                    {
                        status = null;
                        foreach (ParameterUI p in paraUI)
                        {
                            p.ReflectsDeadData();
                        }
                        activate = false;
                    }
                }
                else if (trackingCamera.GetTrackTarget() != null)
                {
                    Controlable controll = trackingCamera.GetTrackTarget().GetComponent<Controlable>();
                    if (controll != null)
                    {
                        trackingCharacter = controll.GetOwner();
                        status = trackingCharacter.GetCharacterStatus();
                        activate = true;
                    }
                }
            }

            public void AddStatusUI(ParameterUI p)
            {
                paraUI.Add(p);
            }
        }
    }
}

