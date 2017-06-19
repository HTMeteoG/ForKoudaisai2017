using UnityEngine;
using HTGames.BattleMain;

namespace HTGames
{
    namespace BallAvoiding
    {
        public class BlowingFun : MonoBehaviour
        {
            [SerializeField]
            Vector3 blowVector;
            [SerializeField]
            float maxDistance = 10;

            void OnTriggerStay(Collider target)
            {
                Blowable b = target.GetComponent<Blowable>();
                if (b != null)
                {
                    Vector3 vec = target.transform.position - transform.position;
                    float distance = Vector3.Project(vec, blowVector).magnitude;
                    float power = Mathf.Max(0, (maxDistance - distance) / maxDistance);

                    b.Blow(power * blowVector * Time.deltaTime);
                }
            }

        }
    }
}
