using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Destroy(player);
            Restart.instance.DiePlayer();
            CameraFollow.instance.DiePlayer();
        }

        if (other.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            enemy.DestroyEnemy();
        }
    }
}
