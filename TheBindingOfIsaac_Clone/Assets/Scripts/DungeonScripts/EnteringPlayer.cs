using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnteringPlayer : MonoBehaviour
{
    [SerializeField] GameObject Light;
    [SerializeField] List<Transform> SpawnersEnemy;
    [SerializeField] List<GameObject> EnemyList;
    [SerializeField] List<GameObject> doors;
    [SerializeField] Transform[] patrolPoints;
    List<GameObject> enemies;
    HashSet<Transform> spawners;
    List<Transform> uniqueSpawners;
    private bool FirstEntering = true;
    private bool chooseSpawn = false;
    public static EnteringPlayer instance;
    private int countEnemy;
    [SerializeField] private bool specialRoom;
    [SerializeField] private bool bossRoom;
    [SerializeField] private GameObject FinalDoor;

    private void Awake()
    {
        spawners = new HashSet<Transform>();
        enemies = new List<GameObject>();
        instance = this;
    }

    private void Start()
    {
       if(specialRoom) return;
       countEnemy = Random.Range(1, SpawnersEnemy.Count);

        for (int i =0;i<countEnemy;i++)
        {
            chooseSpawn = false;
            while(!chooseSpawn)
            {
                var spawn = SpawnersEnemy[Random.Range(0, SpawnersEnemy.Count)];
                if (!spawners.Contains(spawn))
                {
                    chooseSpawn = true;
                    spawners.Add(spawn);
                }
            }
        }

        uniqueSpawners = spawners.ToList<Transform>();
        spawners.Clear();
    }

    private void Update()
    {
        if (enemies.Count > 0 && !FirstEntering)
        {
            foreach(var enemy in enemies)
            {
                if(enemy==null)
                {
                    enemies.Remove(enemy);
                }
            }
        }

        if (enemies.Count == 0 && !FirstEntering)
        {
            foreach (GameObject obj in doors)
            {
                if (obj.activeSelf)
                {
                    OpenGate(obj);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && FirstEntering)
        {
            foreach (GameObject obj in doors)
            {
                if (obj.activeSelf)
                {
                    CloseGate(obj);
                }
            }

            Light.SetActive(true);
            Debug.Log("Player in Room");
            FirstEntering = !FirstEntering;

            for (int i = 0; i < countEnemy; i++)
            {
                GameObject enemy = Instantiate(EnemyList[Random.Range(0, EnemyList.Count )],
                    new Vector3(uniqueSpawners[i].position.x, 0, uniqueSpawners[i].position.z),
                    Quaternion.identity);
                enemies.Add(enemy);
            }
        }
    }

    private void CloseGate(GameObject parent)
    {
        CloseGateRecursively(parent.transform);
    }

    private void CloseGateRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "gate")
            {
                GameObject gateObject = child.gameObject;
                gateObject.SetActive(true);
                return;
            }

            CloseGateRecursively(child);
        }
    }

    private void OpenGate(GameObject parent)
    {
        OpenGateRecursively(parent.transform);
    }

    private void OpenGateRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "gate")
            {
                GameObject gateObject = child.gameObject;
                gateObject.SetActive(false);
                return;
            }

            OpenGateRecursively(child);
        }

        if(bossRoom)
            FinalDoor.SetActive(false);
    }

    public Transform[] GetPatrolPoint()
    {
        return patrolPoints;
    }
}
