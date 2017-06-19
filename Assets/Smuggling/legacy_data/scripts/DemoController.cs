using UnityEngine;
using System.Collections;
namespace HTGames
{
    namespace BattleMain
    {
        //TrackingCameraと同じGameObjectのコンポーネントに追加して使用する
        public class DemoController : Controller
        {
            TrackingCamera trackingCamera;
            Controlable controlTarget;

            float waitTime = 1f;
            float wait = 0;

            void Start()
            {
                trackingCamera = GetComponent<TrackingCamera>();
                if (trackingCamera == null)
                {
                    Debug.LogError("TrackingCamera was not found!!");
                    Destroy(this);
                }
            }

            void Update()
            {
                if (character != null)
                {
                    trackingCamera.SetTrackTarget(character.GetTargetTransform());
                    if (character.GetTargetCharacter() != null)
                    {
                        trackingCamera.SetTrackAnotherTarget
                            (character.GetTargetCharacter().GetTargetTransform());
                    }
                    else
                    {
                        trackingCamera.SetTrackAnotherTarget(null);
                    }
                }
                else
                {
                    wait += Time.deltaTime;
                    if (wait > waitTime)
                    {
                        wait -= waitTime;

                        BattleMainManager b = BattleMainManager.Get();
                        Character c = b.GetRandomCharacter();
                        if (c != null)
                        {
                            SetCharacter(c);
                        }
                    }
                }
            }
        }
    }
}
