using UnityEngine;
using System.Collections;

public class TestCarry2 : MonoBehaviour
{
    [SerializeField]
    float sumMass = 1f;

    Rigidbody rigidBody;
    Joint attachingJoint;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Fall test
        if (transform.position.y < StaticParameter.FALL)
        {
            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            rigidBody.velocity = Vector3.zero;
        }
        //Fall test
    }

    public ConfigurableJoint AddJoint(Character2Behavior character, Vector3 anchor, float distance)
    {
        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.anchor = anchor;
        joint.connectedBody = character.GetComponent<Rigidbody>();

        float distanceX = Vector3.Dot(character.transform.position - transform.position, transform.right);
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor += new Vector3(distanceX, 0.01f, 0.1f);

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        SoftJointLimit sjl = new SoftJointLimit();
        sjl.limit = -45;
        joint.lowAngularXLimit = sjl;
        sjl.limit = 45;
        joint.highAngularXLimit = sjl;

        SoftJointLimitSpring sjls = new SoftJointLimitSpring();
        sjls.spring = 100f;
        joint.linearLimitSpring = sjls;
        joint.angularYZLimitSpring = sjls;

        attachingJoint = joint;
        return joint;
    }

    public void LoseJoint()
    {
        if (attachingJoint != null)
        {
            attachingJoint.breakForce = 0;
        }
    }
}
