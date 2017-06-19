using UnityEngine;
using System.Collections;

public class CarryGrabPoint2 : MonoBehaviour
{
    TestCarry2 carry;
    Character2Behavior grabingCharacter;
    Joint joint;
    float limitDistance;
    float limitAngular = 20f;

    void Start()
    {
        carry = transform.root.GetComponent<TestCarry2>();
    }

    void Update()
    {
        if (grabingCharacter != null)
        {
            if (joint == null)
            {
                grabingCharacter.LoseCarry();
                grabingCharacter = null;
            }
            else
            {
                float distance = (grabingCharacter.transform.position - transform.position).magnitude;
                if (distance > limitDistance)
                {
                    joint.breakForce = 0f;
                }
            }
        }
    }

    public TestCarry2 GetCarry()
    {
        return carry;
    }

    public bool SetGrabbing(Character2Behavior character, float radius)
    {
        if (grabingCharacter == null && joint == null)
        {
            Debug.Log(Vector3.Angle(character.transform.forward, carry.transform.forward));
            if (Vector3.Angle(character.transform.forward, carry.transform.forward) < limitAngular)
            {
                grabingCharacter = character;
                Vector3 anchor = carry.transform.InverseTransformPoint(transform.position);
                limitDistance = radius * 4;
                joint = carry.AddJoint(character, anchor, radius);
                return true;
            }
        }
        return false;
    }
}
