using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Zone : MonoBehaviour
{
    [SerializeField] ZoneType type;
    [SerializeField] Transform teleportPos;

    public enum ZoneType { Teleport, Win };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case ZoneType.Teleport:
                    other.transform.position = teleportPos.position;
                    break;

                case ZoneType.Win:
                    other.GetComponent<PlayerController>().Win();
                    EventsManager.instance.ChangeStateTrigger(EventsManager.GameState.Win);
                    break;

                default:
                    break;
            }
        }
    }
}
