using UnityEngine;
using System.Collections;

public class CrashGround : Ground
{
    /*
    [SerializeField]
    GameObject groundRoot;
    [SerializeField]
    Vector3 crashBlockNum = Vector3.one;
    [SerializeField]
    bool autoRespawn = true;
    [SerializeField]
    float respawnTime = 5f;

    GameObject crashObstacle;

    //For CrashEffect
    int cbn_x;
    int cbn_y;
    int cbn_z;

    Vector3 firstPosition;
    Vector3 crashBlockSize;

    void Start()
    {
        crashObstacle = new GameObject("Cobstacle");
        crashObstacle.transform.localScale = transform.lossyScale + Vector3.up;
        crashObstacle.transform.position = transform.position + Vector3.up * transform.localScale.y;
        crashObstacle.transform.rotation = groundRoot.transform.rotation;

        CrashObstacle cObst = crashObstacle.AddComponent<CrashObstacle>();
        cObst.init(groundRoot, respawnTime);
        crashObstacle.SetActive(false);

        cbn_x = (int)crashBlockNum.x;
        cbn_y = (int)crashBlockNum.y;
        cbn_z = (int)crashBlockNum.z;
        crashBlockSize = new Vector3(groundRoot.transform.lossyScale.x / cbn_x,
                                     groundRoot.transform.lossyScale.y / cbn_y,
                                     groundRoot.transform.lossyScale.z / cbn_z);
        firstPosition = groundRoot.transform.position
                      - groundRoot.transform.lossyScale / 2f
                      + crashBlockSize / 2f;
    }

    public void GroundCrash()
    {
        Collider[] grounds = Physics.OverlapBox(transform.position + Vector3.up * transform.lossyScale.y, transform.lossyScale, transform.rotation);
        for (int i = 0; i < grounds.Length; i++)
        {
            Character2Behavior c = grounds[i].GetComponent<Character2Behavior>();
            if (c != null)
            {
                c.GetAttribution().ChangeState(CharacterState2.Sky);
            }
        }
        
        crashObstacle.SetActive(true);
        groundRoot.SetActive(false);

        CrashEffect();
    }

    //Make crash effect
    void CrashEffect()
    {
        GameObject crashBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crashBlock.transform.rotation = groundRoot.transform.rotation;
        crashBlock.transform.localScale = crashBlockSize;
        crashBlock.AddComponent<CrashBlock>();
        crashBlock.AddComponent<Rigidbody>();
        for (int i = 0; i < cbn_x; i++)
        {
            for (int j = 0; j < cbn_y; j++)
            {
                for (int k = 0; k < cbn_z; k++)
                {
                    Vector3 blockPosition = firstPosition
                                  + new Vector3(crashBlockSize.x * i,
                                                crashBlockSize.y * j,
                                                crashBlockSize.z * k);
                    GameObject cb = (GameObject)Instantiate(crashBlock, blockPosition, groundRoot.transform.rotation);
                    cb.GetComponent<Rigidbody>().AddExplosionForce(10f, groundRoot.transform.position,
                        Vector3.Magnitude(groundRoot.transform.lossyScale), 2f, ForceMode.VelocityChange);
                }
            }
        }
        Destroy(crashBlock);
    }

    public override bool isHangable()
    {
        return false;
    }
    */
}
