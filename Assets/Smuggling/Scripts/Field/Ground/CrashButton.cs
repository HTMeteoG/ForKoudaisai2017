using UnityEngine;
using System.Collections.Generic;

public class CrashButton : MonoBehaviour
{
    [SerializeField]
    List<Transform> crashObjects = new List<Transform>();
    
    List<CrashObstacle> crashObstacles = new List<CrashObstacle>();

    List<GameObject> pushingObject = new List<GameObject>();
    Vector3 startScale;
    BoxCollider buttonCollider;
    Vector3 buttonStartScale;

    void Start()
    {
        foreach (Transform cobje in crashObjects)
        {
            for (int i = 0; i < cobje.childCount; i++)
            {
                CrashObstacle cground = cobje.GetChild(i).GetComponent<CrashObstacle>();
                if (cground != null)
                {
                    crashObstacles.Add(cground);
                    break;
                }
            }
        }

        startScale = transform.localScale;
        buttonCollider = GetComponent<BoxCollider>();
        buttonStartScale = buttonCollider.size;
    }

    void Update()
    {
        foreach (GameObject o in pushingObject)
        {
            if (!o.activeInHierarchy)
            {
                pushingObject.Remove(o);
                break;
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (pushingObject.Count == 0)
        {
            transform.localScale = startScale - Vector3.up * startScale.y * 0.75f;
            buttonCollider.size = buttonCollider.size + Vector3.up * buttonCollider.size.y * 3f;
            foreach (CrashObstacle cground in crashObstacles)
            {
                if (cground.isActiveAndEnabled)
                {
                    cground.Respawn();
                }
                else
                {
                    cground.GetGround().GroundCrash();
                }
            }
        }

        if (!pushingObject.Contains(c.gameObject))
        {
            pushingObject.Add(c.gameObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        pushingObject.Remove(c.gameObject);

        if (pushingObject.Count == 0)
        {
            transform.localScale = startScale;
            buttonCollider.size = buttonStartScale;
        }
    }
}
