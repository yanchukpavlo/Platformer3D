using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisappear : MonoBehaviour
{
    [SerializeField] float timeToDisappear = 3;
    [SerializeField] float timeBetweenDisappear = 3;
    [SerializeField] float animationTime = 0.5f;

    bool isDisappear;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isDisappear)
        {
            StartCoroutine(DisappearCoroutine());
        }
    }

    IEnumerator DisappearCoroutine()
    {
        isDisappear = true;

        yield return new WaitForSeconds(timeToDisappear);
        animator.SetTrigger("fadeIn");

        yield return new WaitForSeconds(timeBetweenDisappear + animationTime);
        animator.SetTrigger("fadeOut");

        isDisappear = false;
    }
}
