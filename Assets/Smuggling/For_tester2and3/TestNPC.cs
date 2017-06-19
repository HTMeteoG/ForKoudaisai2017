using UnityEngine;
using System.Collections;

public class TestNPC : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    float speed;

    [SerializeField]
    Character2Behavior testTarget;
    [SerializeField]
    Vector3 goalPoint;

    float testWaitTime = 2f;
    float wait = 0f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        speed = agent.speed;
    }

    void Update()
    {
        if (!agent.hasPath)
        {
            agent.ResetPath();
            agent.SetDestination(CalcNextPosition());
            wait = testWaitTime;
            agent.Stop();
        }
        else
        {
            if (wait > 0)
            {
                wait -= Time.deltaTime;
                if (wait <= 0)
                {
                    if (agent.pathPending)
                    {
                        wait += Time.deltaTime * 2f;
                    }
                    else
                    {
                        agent.Resume();
                    }
                }
            }
        }
    }

    Vector3 CalcNextPosition()
    {
        if (testTarget != null)
        {
            return testTarget.transform.position - testTarget.transform.forward;
        }
        else
        {
            return goalPoint;
        }
    }
}
