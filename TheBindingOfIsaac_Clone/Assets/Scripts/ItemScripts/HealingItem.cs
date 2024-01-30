using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : Item
{
    protected override void CustomOnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.Healing(50);
            Destroy(gameObject);
        }
    }
}
