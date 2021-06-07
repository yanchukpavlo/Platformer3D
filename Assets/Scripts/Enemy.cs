using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);

            Destroy(GetComponent<Collider>());
            GetComponent<Animator>().SetTrigger("die");
            EventsManager.instance.EnemyDestroyTrigger();

            Destroy(gameObject, 2f);
        }
    }
}
