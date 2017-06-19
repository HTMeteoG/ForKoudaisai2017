using UnityEngine;
using System.Collections;

namespace HTGames
{
    namespace BattleMain
    {
        enum TrackMode
        {
            Free,                   //回転は自動だが手動で調整も可能
            FixedAngle,             //回転は手動で調整する(ターゲットがいる場合はそれも視界に入れる)

        };

        //対象を追従するカメラ
        public class TrackingCamera : MonoBehaviour
        {
            [SerializeField]
            Transform trackTarget;
            [SerializeField]
            Transform trackAnotherTarget;

            [SerializeField]
            TrackMode trackMode = TrackMode.Free;
            Transform cam;

            [SerializeField]
            float distance = 5;

            float camPositionY = 1;
            [SerializeField]
            float camPositionY_Manual = 1;

            const float yAngleMax = 45 * Mathf.Deg2Rad;
            const float yAngleMin = -10 * Mathf.Deg2Rad;
            const float rotateSpeedAuto = 30;

            void Start()
            {
                cam = transform.GetChild(0);
                cam.localPosition = Vector3.zero;
            }

            public Transform GetTrackTarget()
            {
                return trackTarget;
            }

            public void SetTrackTarget(Transform t)
            {
                trackTarget = t;
            }

            public void SetTrackAnotherTarget(Transform t)
            {
                trackAnotherTarget = t;
            }


            void Update()
            {
                if (trackTarget != null)
                {
                    if (trackMode == TrackMode.Free)
                    {
                        FreeTrack();
                        SetCamPositionY(camPositionY_Manual);
                    }

                    if (trackMode == TrackMode.FixedAngle)
                    {
                        FixedAngleTrack();
                        if (trackAnotherTarget != null)
                        {
                            TargetingAnotherTrack();
                        }
                        else
                        {
                            SetCamPositionY(camPositionY_Manual);
                        }
                    }

                    /* 能動的にカメラ操作をしたいとき(Free時は非推奨、バグる)
                    CamRotateHorizon(Input.GetAxis("MoveX") * 1f);
                    SetCamPositionY(camPositionY + Input.GetAxis("MoveY") * Time.deltaTime * 3);
                    */
                }
            }

            //TrackMode.Free
            void FreeTrack()
            {
                PositionTrack(distance);
                CamPositionTrack(0, camPositionY);

                Vector3 horizonDistance = Vector3.ProjectOnPlane((trackTarget.position - transform.position), Vector3.up);
                float dist = horizonDistance.magnitude;
                if (dist > 0.1)
                {
                    Quaternion rotateTarget = Quaternion.LookRotation(trackTarget.position - cam.position);
                    cam.rotation = Quaternion.RotateTowards(cam.rotation, rotateTarget, rotateSpeedAuto);
                }
            }

            //TrackMode.FixedAngle
            void FixedAngleTrack()
            {
                PositionTrack(0);
                CamPositionTrack(distance, camPositionY);

                Vector3 camDistance = transform.position - cam.position;
                float dist = camDistance.magnitude;
                if (dist > 0.1)
                {
                    Quaternion rotateTarget = Quaternion.LookRotation(transform.position - cam.position);
                    cam.rotation = Quaternion.RotateTowards(cam.rotation, rotateTarget, rotateSpeedAuto);
                }
            }

            //ターゲットを視界に入れるためのカメラ回転処理
            void TargetingAnotherTrack()
            {
                Vector3 cameraDirection = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
                Vector3 targetDirection = Vector3.ProjectOnPlane(trackAnotherTarget.position - trackTarget.position, Vector3.up);
                float targetDistance = targetDirection.magnitude;
                targetDirection.Normalize();

                float targetY = trackAnotherTarget.position.y - trackTarget.position.y;
                float angleH = Vector3.Angle(cameraDirection, targetDirection);
                float angleY = Mathf.Atan2(targetY, targetDistance);

                if (angleH > 20)//角度20度以上で回転処理
                {
                    Vector3 cameraDirectionCross = Vector3.Cross(Vector3.up, cameraDirection);
                    float dot = Vector3.Dot(cameraDirectionCross, targetDirection);
                    if (dot > 0)
                    {
                        CamRotateHorizon((angleH - 20) * 0.1f);
                    }
                    else if (dot < 0)
                    {
                        CamRotateHorizon(-(angleH - 20) * 0.1f);
                    }
                }

                SetCamPositionY(targetDistance * Mathf.Tan(-angleY));
            }

            //座標のトラッキング、引数はtransformと目標との距離
            void PositionTrack(float centerDistance)
            {
                Vector3 horizonDistance = Vector3.ProjectOnPlane((trackTarget.position - transform.position), Vector3.up);
                float dampVelocityH = 0;
                float dampVelocityY = 0;
                //zx平面上の移動
                Mathf.SmoothDamp(horizonDistance.magnitude, centerDistance, ref dampVelocityH, 1);
                transform.Translate(horizonDistance.normalized * -dampVelocityH);
                //y軸方向の移動
                Mathf.SmoothDamp(transform.position.y, trackTarget.position.y, ref dampVelocityY, 1);
                transform.Translate(Vector3.up * dampVelocityY);

                if (horizonDistance.magnitude < 0.01f)
                {
                    transform.Translate(transform.forward * -0.01f);
                }
            }

            //カメラオブジェクトのトラッキング、引数はzx平面上の距離とY軸距離
            void CamPositionTrack(float distH, float distY)
            {
                Vector3 horizonDistance = Vector3.ProjectOnPlane((transform.position - cam.position), Vector3.up);
                float dampVelocityH = 0;
                float dampVelocityY = 0;
                //zx平面上の移動
                Mathf.SmoothDamp(horizonDistance.magnitude, distH, ref dampVelocityH, 1);
                cam.Translate(horizonDistance.normalized * -dampVelocityH, Space.World);

                //y軸方向の移動
                Mathf.SmoothDamp(cam.position.y - transform.position.y, distY, ref dampVelocityY, 1);
                cam.Translate(Vector3.up * dampVelocityY, Space.World);

                if (horizonDistance.magnitude < 0.01f)
                {
                    cam.Translate(cam.forward * -0.01f, Space.World);
                }
            }

            //カメラオブジェクトの回転(水平方向、垂直方向はcamPositionYで調整する)
            public void CamRotateHorizon(float angle)
            {
                if (trackMode == TrackMode.FixedAngle)
                {
                    cam.RotateAround(transform.position, Vector3.up, angle);
                }
            }

            //camPositionYの調整
            public void SetCamPositionY(float newY)
            {
                float newAngle = Mathf.Atan2(newY, distance);

                if (newAngle > yAngleMax)
                {
                    newY = distance * Mathf.Tan(yAngleMax);
                }
                if (newAngle < yAngleMin)
                {
                    newY = distance * Mathf.Tan(yAngleMin);
                }

                camPositionY = newY;
            }

            public Vector3 ReviseHorizonnVector(Vector2 vec)
            {
                Vector3 forward;
                if (trackAnotherTarget != null && trackMode == TrackMode.FixedAngle)
                {
                    forward = trackAnotherTarget.position - trackTarget.position;
                }
                else
                {
                    forward = cam.forward;
                }

                Vector3 axisZ = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
                Vector3 axisX = Vector3.Cross(Vector3.up, axisZ).normalized;

                return axisX * vec.x + axisZ * vec.y;
            }
        }
    }
}
