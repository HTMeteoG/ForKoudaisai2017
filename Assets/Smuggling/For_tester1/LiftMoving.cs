using UnityEngine;
using System.Collections;

public class LiftMoving : MonoBehaviour
{

    [SerializeField]
    Vector3 startPosition;
    [SerializeField]
    Vector3 endPosition;
    [SerializeField]
    float timePeriod = 1f;

    Vector3 centorPosition;
    Vector3 moveVector;
    float rad;

    void Start()
    {
        centorPosition = (endPosition + startPosition) * 0.5f;
        moveVector = (endPosition - startPosition) * 0.5f;
    }

    void Update()
    {
        transform.position = centorPosition + moveVector * Mathf.Sin(rad);
        rad += Time.deltaTime * 2 * Mathf.PI / timePeriod;
    }
}
