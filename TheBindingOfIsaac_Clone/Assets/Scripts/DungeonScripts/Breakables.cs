using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakables : MonoBehaviour
{

    [SerializeField]private bool shouldDropItem;
    [SerializeField] private GameObject[] itemsToDrop;
    [SerializeField] private float itemDropPercent;

    private void Start()
    {
        shouldDropItem = Random.Range(0f, 1f) < 0.5f;
    }


    public void Smash()
    {
        Destroy(gameObject);

        //drop items
        if (shouldDropItem)
        {
            float dropChance = Random.Range(0f, 100f);

            if (dropChance < itemDropPercent)
            {
                int randomItem = Random.Range(0, itemsToDrop.Length);

                Instantiate(itemsToDrop[randomItem], transform.position, transform.rotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerProjectTile>() || other.GetComponentInParent<PlayerProjectTile>())
        {
            Smash();
        }
    }
}
