using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] List<Item> items = new List<Item>();
    private bool canOpen, isOpen;
    [SerializeField] private GameObject lid;
    [SerializeField] private Transform spawnItem;
    [SerializeField] private float throwForce;
    [System.Serializable]
    private class Item
    {
        public GameObject itemObject;
        public float dropChance; 
    }

    void Update()
    {
        if (canOpen && !isOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isOpen = true;
                OpenChest();
            }
        }
    }



    private void OpenChest()
    {
        lid.SetActive(!isOpen);
        float totalChance = 0f;
        foreach (Item item in items)
        {
            totalChance += item.dropChance;
        }

        float randomChance = Random.Range(0f, totalChance);
        Debug.Log("Chance:"+ randomChance);

        foreach (Item item in items)
        {
            randomChance -= item.dropChance;

            if (randomChance <= 0f)
            {
                GameObject itemSpawned = Instantiate(item.itemObject, spawnItem.position, Quaternion.identity);
                itemSpawned.GetComponent<Rigidbody>().AddForce(-spawnItem.transform.forward* throwForce,ForceMode.VelocityChange);
                break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            canOpen = true;
        }
        else
        {
            canOpen=false;
        }
    }
}
