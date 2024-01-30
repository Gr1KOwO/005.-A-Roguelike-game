using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealingItem : Item
{
    protected override void CustomOnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.MaxHealing();
            Destroy(gameObject);
        }
    }
}
