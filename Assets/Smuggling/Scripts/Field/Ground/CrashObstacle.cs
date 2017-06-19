using UnityEngine;
using System.Collections;

public class CrashObstacle : MonoBehaviour
{
    GameObject crashedGroundRoot;

    float respawnTime;
    float respawn;
    bool autoRespawn = true;

    public void init(GameObject cgRoot, float rTime)
    {
        crashedGroundRoot = cgRoot;
        respawnTime = rTime;
        if(respawnTime == 0)
        {
            autoRespawn = false;
        }
        else
        {
            autoRespawn = true;
        }

        if (gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>() == null)
        {
            UnityEngine.AI.NavMeshObstacle obstacle = gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            obstacle.carving = true;
            obstacle.size = Vector3.one;
        }
    }

    void Update()
    {
        if (crashedGroundRoot != null && autoRespawn)
        {
            respawn += Time.deltaTime;
            if (respawn > respawnTime)
            {
                respawn = 0;
                Respawn();
            }
        }
    }

    public void Respawn()
    {
        crashedGroundRoot.SetActive(true);
        gameObject.SetActive(false);
    }

    public Ground GetGround()
    {
        return crashedGroundRoot.GetComponent<Ground>();
    }
}
