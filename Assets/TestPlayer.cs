using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestPlayer : MonoBehaviour
{
    Camera c;
    NavMeshAgent a;
    float camHeight = 1.5f;
    float camForward = 10;
    Vector3 xAxis = Vector3.forward;
    Vector3 newXAxis;

    float rotateTime = 1;

    void Start()
    {
        c = FindObjectOfType<Camera>();
        a = GetComponent<NavMeshAgent>();

        c.transform.rotation = Quaternion.Euler(5, -Vector3.Angle(Vector3.right, xAxis), 0);
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        a.velocity = xAxis * moveX * a.speed;


        if (rotateTime < 1)
        {
            rotateTime += Time.deltaTime;
            xAxis = Vector3.RotateTowards(xAxis, newXAxis, rotateTime, 0);
            c.transform.rotation = Quaternion.LookRotation(Vector3.Cross(xAxis, Vector3.up));
                //+ Quaternion.Euler(5, 0, 0);
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") >= 0)
            {
                newXAxis = Vector3.Cross(xAxis, Vector3.up);
            }
            else
            {
                newXAxis = -Vector3.Cross(xAxis, Vector3.up);
            }
            rotateTime = 0;
        }


        if (c != null)
        {
            c.transform.position = transform.position
                + Vector3.up * camHeight - Vector3.Cross(xAxis, Vector3.up) * camForward;
        }
    }
}
