using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.UpgradeDamage();
            Destroy(gameObject);
        }
    }
}
