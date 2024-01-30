using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Transform spawnPointPlayer;
    [SerializeField]private GameObject spawnedPlayer;

    private void Awake()
    {
        if (spawnedPlayer != null)
        {
            return;
        }

        if (Restart.instance.GetPlayer() != null)
        {
            spawnedPlayer = Restart.instance.GetPlayer();
            spawnedPlayer.transform.position = spawnPointPlayer.position;
            return;
        }

        if (player != null && spawnPointPlayer != null)
        {
            spawnedPlayer = Instantiate(player, spawnPointPlayer.position, Quaternion.identity);
            Restart.instance.SetPlayer(spawnedPlayer);
        }
        Camera.main.GetComponent<CameraFollow>().SetTarget(spawnedPlayer);
    }
}
