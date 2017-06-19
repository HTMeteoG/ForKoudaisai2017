using UnityEngine;
using System.Collections;

public class CarryGrabPoint : MonoBehaviour
{
    TestCarry carry;
    TestCharacter grabingCharacter;
    Joint joint;
    float limitDistance;

    void Start()
    {
        carry = transform.root.GetComponent<TestCarry>();
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

    public TestCarry GetCarry()
    {
        return carry;
    }

    public bool SetGrabbing(TestCharacter character, float radius)
    {
        if (grabingCharacter == null && joint == null)
        {
            grabingCharacter = character;
            Vector3 anchor = carry.transform.InverseTransformPoint(transform.position);
            limitDistance = radius * 4;
            joint = carry.AddJoint(character, anchor, radius);
            return true;
        }
        return false;
    }
}
