using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHealing : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.UpgradeHealth(90);
            Destroy(gameObject);
        }
    }
}
