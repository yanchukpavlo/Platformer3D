using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera cinemachine;
    GameObject player;

    private void Start()
    {
        SetPlayer();
    }

    public void SetPlayer()
    {
        cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
        player = FindObjectOfType<PlayerController>().gameObject;

        cinemachine.Follow = player.transform;
        cinemachine.LookAt = player.transform;
    }
}
