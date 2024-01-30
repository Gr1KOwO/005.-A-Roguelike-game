using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.GetGain();
            Destroy(gameObject);
        }
    }
}
