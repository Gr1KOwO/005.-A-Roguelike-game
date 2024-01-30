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
        public float dropChance; // Шанс выпадения предмета (от 0 до 1)
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
        // Суммируем все шансы
        float totalChance = 0f;
        foreach (Item item in items)
        {
            totalChance += item.dropChance;
        }
        // Генерируем случайное число от 0 до суммарного шанса
        float randomChance = Random.Range(0f, totalChance);
        Debug.Log("Chance:"+ randomChance);

        // Проходимся по всем предметам и выбираем предмет
        foreach (Item item in items)
        {
            // Вычитаем шанс предмета из случайного числа
            randomChance -= item.dropChance;

            // Если случайное число стало отрицательным, выбираем этот предмет
            if (randomChance <= 0f)
            {
                // Создаем предмет и располагаем его в сундуке
                GameObject itemSpawned = Instantiate(item.itemObject, spawnItem.position, Quaternion.identity);
                itemSpawned.GetComponent<Rigidbody>().AddForce(-spawnItem.transform.forward* throwForce,ForceMode.VelocityChange);
                break; // Прерываем цикл, так как предмет уже выбран
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
