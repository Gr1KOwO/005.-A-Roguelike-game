using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AddBombItem : Item
{
    protected override void CustomOnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.AddBomb();
            Destroy(gameObject);
        }
    }
}