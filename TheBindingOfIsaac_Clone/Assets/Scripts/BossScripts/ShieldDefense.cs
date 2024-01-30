using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldDefense : MonoBehaviour
{
    [SerializeField] private int damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerProjectTile>())
        {
            Destroy(other.gameObject);
        }

        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.GetDamage(damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.GetDamage(damage);
        }
    }
}
