using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] PickupType type;
    GameObject textToEnable; 
    [SerializeField] GameObject gameObjectToEnable;

    [Header("Destroy")]
    [SerializeField] bool isDestroy = true;
    [SerializeField] float timeToDestroy = 2;

    PlayerController playerMovement;
    bool isInteractable = true;
    enum PickupType {Null, Weapon, Interact};

    private void Start()
    {
        switch (type)
        {
            case PickupType.Null:
                break;

            case PickupType.Weapon:
                textToEnable = UI_Manager.instance.TakeText;
                break;

            case PickupType.Interact:
                textToEnable = UI_Manager.instance.InteractText;
                break;

            default:
                Debug.LogError("Should not be default statement.");
                break;
        }
    }

    void Update()
    {
        if (playerMovement != null && isInteractable)
        {
            if (Input.GetButtonDown("Interact") && playerMovement.PickingUp())
            {
                isInteractable = false;

                GetComponent<Animator>().SetTrigger("take");

                textToEnable.SetActive(false);
                if (gameObjectToEnable != null) gameObjectToEnable.SetActive(true);

                if (isDestroy) Destroy(gameObject, timeToDestroy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<PlayerController>();
            textToEnable.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = null;
            textToEnable.SetActive(false);
        }
    }
}
