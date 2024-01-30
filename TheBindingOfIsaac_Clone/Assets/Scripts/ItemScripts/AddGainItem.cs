using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGainItem : Item
{
    protected override void CustomOnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.AddGain(35);
            Destroy(gameObject);
        }
    }
}
