using UnityEngine;
using System.Collections;
using System;

public class Ground : MonoBehaviour, CollisionableToCharacter
{
    [SerializeField]
    GroundType firstType = GroundType.UnBroken;

    GameObject groundRoot;
    [SerializeField]
    Vector3 crashBlockNum = Vector3.one;

    float minHP = 100f;
    float maxHP = 1000f;
    float hp;

    [SerializeField]
    float respawnTime = 5f;

    Material gMaterial;
    static Material crackMaterial;

    GameObject crashObstacle;

    //For CrashEffect
    int cbn_x;
    int cbn_y;
    int cbn_z;

    Vector3 firstPosition;
    Vector3 crashBlockSize;

    void Awake()
    {
        groundRoot = transform.parent.gameObject;

        cbn_x = (int)crashBlockNum.x;
        cbn_y = (int)crashBlockNum.y;
        cbn_z = (int)crashBlockNum.z;
        crashBlockSize = new Vector3(groundRoot.transform.lossyScale.x / cbn_x,
                                     groundRoot.transform.lossyScale.y / cbn_y,
                                     groundRoot.transform.lossyScale.z / cbn_z);
    }

    public void init(GroundType type)
    {
        firstType = type;

        if (crashObstacle == null)
        {
            crashObstacle = new GameObject(groundRoot.name + "_Cobstacle");
            crashObstacle.transform.SetParent(groundRoot.transform.parent);
            crashObstacle.transform.localScale = transform.lossyScale + Vector3.up;
            crashObstacle.transform.position = transform.position + Vector3.up * transform.localScale.y;
            crashObstacle.transform.rotation = groundRoot.transform.rotation;
        }
        crashObstacle.SetActive(false);
        CrashObstacle cObst = crashObstacle.GetComponent<CrashObstacle>();
        if (cObst == null)
        {
            cObst = crashObstacle.AddComponent<CrashObstacle>();
        }

        if (crackMaterial == null)
        {
            crackMaterial = Resources.Load("BlockMaterial/Breakable_cracked") as Material;
        }

        switch (firstType)
        {
            case GroundType.Breakable:
                cObst.init(groundRoot, respawnTime);
                gMaterial = Resources.Load("BlockMaterial/BreakableBlock") as Material;
                groundRoot.GetComponent<MeshRenderer>().material = gMaterial;
                break;
            case GroundType.Danger:
                cObst.init(groundRoot, 0);
                gMaterial = Resources.Load("BlockMaterial/DangerBlock") as Material;
                crashObstacle.SetActive(true);
                break;
            case GroundType.Disappear:
                cObst.init(groundRoot, 0);
                gMaterial = Resources.Load("BlockMaterial/UnbrokenBlock") as Material;
                groundRoot.GetComponent<MeshRenderer>().material = gMaterial;
                GroundCrash(false);
                break;
            default:
                cObst.init(groundRoot, 0);
                gMaterial = Resources.Load("BlockMaterial/UnbrokenBlock") as Material;
                groundRoot.GetComponent<MeshRenderer>().material = gMaterial;
                break;
        }
        GetComponent<MeshRenderer>().material = gMaterial;

        ResetHP();
    }

    public void ResetHP()
    {
        hp = UnityEngine.Random.Range(minHP, maxHP);
        GetComponent<MeshRenderer>().material = gMaterial;
    }

    public void GroundCrash()
    {
        GroundCrash(true);
    }

    public void GroundCrash(bool addEffect)
    {
        Collider[] grounds = Physics.OverlapBox(transform.position + Vector3.up * transform.lossyScale.y, transform.lossyScale, transform.rotation);
        for (int i = 0; i < grounds.Length; i++)
        {
            Character2Behavior c = grounds[i].GetComponent<Character2Behavior>();
            if (c != null)
            {
                c.GetAttribution().ChangeState(CharacterState2.Down);
            }
        }

        crashObstacle.SetActive(true);
        groundRoot.SetActive(false);

        if (addEffect)
        {
            CrashEffect();
        }
    }

    //Make crash effect
    void CrashEffect()
    {
        firstPosition = groundRoot.transform.position
                      - groundRoot.transform.TransformDirection(groundRoot.transform.lossyScale) / 2f
                      + groundRoot.transform.TransformDirection(crashBlockSize) / 2f;

        GameObject crashBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crashBlock.transform.rotation = groundRoot.transform.rotation;
        crashBlock.transform.localScale = crashBlockSize;
        crashBlock.GetComponent<MeshRenderer>().material = gMaterial;
        crashBlock.AddComponent<CrashBlock>();
        crashBlock.AddComponent<Rigidbody>();
        for (int i = 0; i < cbn_x; i++)
        {
            for (int j = 0; j < cbn_y; j++)
            {
                for (int k = 0; k < cbn_z; k++)
                {
                    Vector3 blockPosition = firstPosition
                                  + groundRoot.transform.TransformDirection(
                                        new Vector3(crashBlockSize.x * i,
                                                    crashBlockSize.y * j,
                                                    crashBlockSize.z * k));
                    GameObject cb = (GameObject)Instantiate(crashBlock, blockPosition, groundRoot.transform.rotation);
                    cb.GetComponent<Rigidbody>().AddExplosionForce(10f, groundRoot.transform.position,
                        Vector3.Magnitude(groundRoot.transform.lossyScale), 2f, ForceMode.VelocityChange);
                    ClimbSystem.SetGameObject(cb.transform);
                }
            }
        }
        Destroy(crashBlock);
    }

    public GroundType GetGroundType()
    {
        return firstType;
    }

    public virtual bool isHangable()
    {
        if (firstType == GroundType.UnBroken || firstType == GroundType.Disappear)
        {
            return true;
        }
        return false;
    }

    public Vector3 getGroundNormal()
    {
        return transform.up;
    }

    public void CollisionAction(Character2 chara, Collision c)
    {
        CollisionAction(chara);
    }

    public void CollisionAction(Character2 chara)
    {
        if (firstType == GroundType.Danger)
        {
            Vector3 rand = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            chara.ForceMove((rand + getGroundNormal()).normalized * 10f);
            chara.Damage(StaticParameter.FALL_DAMAGE * 0.1f);
        }
        else
        {
            Pressure(100);
        }
    }

    public void Pressure(float damage)
    {
        if (firstType == GroundType.Breakable)
        {
            hp -= damage;
            if (hp <= 0)
            {
                ResetHP();

                GroundCrash(true);
            }
            else if (hp < 100)
            {
                GetComponent<MeshRenderer>().material = crackMaterial;
            }
        }
    }
}

public enum GroundType
{
    UnBroken,   //ダメージでは絶対に消えない足場
    Breakable,  //ダメージで消える足場(時間で自動復活する)
    Danger,     //ダメージ床
    Disappear,  //消滅状態

}