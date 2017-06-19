using UnityEngine;
using System.Collections;

public class Character2Down : MonoBehaviour
{
    Character2 attributionCharacter;
    Rigidbody rigid;

    public void SetAttribution(Character2 c)
    {
        attributionCharacter = c;
        rigid = gameObject.GetComponent<Rigidbody>();
        ClimbSystem.SetGameObject(transform);
    }

    public Vector3 GetVelocity()
    {
        if (rigid == null)
        {
            return Vector3.zero;
        }
        return rigid.velocity;
    }

    public void SetVelocity(Vector3 vec)
    {
        rigid.velocity = vec;
    }

    void Start()
    {

    }

    void Update()
    {
        if (transform.position.y < StaticParameter.FALL)
        {
            attributionCharacter.ChangeState(CharacterState2.Fall);
        }
    }

    void OnCollisionEnter(Collision c)
    {
        CollisionableToCharacter col = c.gameObject.GetComponent<CollisionableToCharacter>();
        if (col != null)
        {
            col.CollisionAction(attributionCharacter, c);
        }
    }
}
