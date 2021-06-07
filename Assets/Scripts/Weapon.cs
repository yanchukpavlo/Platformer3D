using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform shootPos;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 30f;

    [Header("Time")]
    public float timeBetweenShot = 1f;
    public float timeToBulletDestroy = 7f;

    float timer;
    
    public void Shot(Transform playerPos)
    {
        if (Time.time > timer)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);

            bullet.GetComponent<Rigidbody>().velocity = playerPos.forward * bulletSpeed;
            Destroy(bullet, timeToBulletDestroy);

            timer = Time.time + timeBetweenShot;
        }
    }
}
