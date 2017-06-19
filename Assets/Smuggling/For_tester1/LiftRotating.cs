using UnityEngine;
using System.Collections;

public class LiftRotating : MonoBehaviour
{

    [SerializeField]
    Vector3 startRotation;
    [SerializeField]
    Vector3 endRotation;
    [SerializeField]
    float timePeriod = 1f;

    Quaternion centorRotation;
    Vector3 moveRotate;
    float rad;

    void Start()
    {
        centorRotation = Quaternion.Euler((endRotation + startRotation) * 0.5f);
        moveRotate = (endRotation - startRotation) * 0.5f;
        transform.rotation = centorRotation;
    }

    void Update()
    {
        float deltaRad = rad + Time.deltaTime * 2 * Mathf.PI / timePeriod;
        float delta = Mathf.Sin(deltaRad) - Mathf.Sin(rad);

        transform.Rotate(moveRotate * delta);
        rad = deltaRad;
    }
}
