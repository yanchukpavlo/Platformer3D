using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] Transform platform;
    [SerializeField] Transform startTransform;
    [SerializeField] Transform endTransform;
    [SerializeField] [Range(1f, 10f)] float timeBetweenSwapPoint = 2f;
    [SerializeField] float speed = 2f;
    [SerializeField] float stopDistance = 0.01f;

    [Header("Gizmos")]
    [SerializeField] float sphereRadius = 0.2f;
    [SerializeField] float boxRadius = 0.2f;

    Vector3 direction;
    public Vector3 Direction
    { 
        get { return direction; } 
    }

    float step;
    bool isMove;

    IEnumerator Start()
    {        
        step = speed * Time.deltaTime;
        yield return new WaitForSeconds(timeBetweenSwapPoint);

        CalculateDirection();

        isMove = !isMove;
    }

    void Update()
    {
        step = speed * Time.deltaTime;
        if (isMove)
        {
            platform.position = Vector3.MoveTowards(platform.position, endTransform.position, step);
        }

        if (Vector3.Distance(platform.position, endTransform.position) < stopDistance)
        {
            isMove = !isMove;
            StartCoroutine(WaitToChangePoint());
        }
    }

    IEnumerator WaitToChangePoint()
    {
        PointsSwap();

        yield return new WaitForSeconds(timeBetweenSwapPoint);
        CalculateDirection();

        isMove = true;
    }

    private void PointsSwap()
    {
        direction = Vector3.zero;

        Transform tempTransform = startTransform;
        startTransform = endTransform;
        endTransform = tempTransform;
    }

    private void CalculateDirection()
    {
        direction = endTransform.position - startTransform.position;
        Vector3 normalized = direction.normalized;
        direction = normalized * speed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startTransform.position, endTransform.position);

        Gizmos.DrawWireSphere(startTransform.position, sphereRadius);
        Gizmos.DrawWireCube(endTransform.position, new Vector3(boxRadius, boxRadius, boxRadius));
    }
#endif

}
