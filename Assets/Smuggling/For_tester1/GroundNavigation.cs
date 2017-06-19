using UnityEngine;
using System.Collections;

//空オブジェクト、地面移動時の指標になる？
public class GroundNavigation : MonoBehaviour
{

    TestCharacter ownerCharacter;

    Ground groundObject;
    Vector3 lastWorldPosition;
    Vector3 lastLocalPosition;
    Vector3 lastForward;


    public void init(TestCharacter c)
    {
        ownerCharacter = c;
    }

    void Start()
    {

    }

    void Update()
    {
        if (groundObject != null)
        {
            Vector3 deltaLocalP = transform.TransformVector(transform.localPosition - lastLocalPosition);
            Vector3 deltaWorldP = transform.position - lastWorldPosition;
            Quaternion deltaRot = Quaternion.FromToRotation(lastForward, transform.forward);

            ownerCharacter.transform.position += (deltaWorldP - deltaLocalP);
            ownerCharacter.transform.Rotate(Vector3.Project(deltaRot.eulerAngles, Vector3.up));

            lastLocalPosition = transform.localPosition;
            lastWorldPosition = transform.position;
            lastForward = transform.forward;
        }
    }

    public void SetGround(Ground ground, Vector3 pos)
    {
        if (ground == null)
        {
            groundObject = null;
            transform.SetParent(null);
            transform.localScale = Vector3.one;
        }
        else if (groundObject != ground)
        {
            groundObject = ground;
            transform.SetParent(ground.transform);
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            transform.position = pos;
            lastLocalPosition = transform.localPosition;
            lastWorldPosition = transform.position;
            lastForward = transform.forward;
        }
        else
        {
            transform.position = pos;
        }
    }
}
