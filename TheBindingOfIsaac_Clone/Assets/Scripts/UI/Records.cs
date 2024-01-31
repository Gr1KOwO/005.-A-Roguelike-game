using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Records : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Transform contentArea;
    [SerializeField] private GameObject playerRecordPrefab;

    private void Start()
    {
        LoadPlayerRecords();
    }

    private void LoadPlayerRecords()
    {
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        string allPlayerNames = PlayerPrefs.GetString("AllPlayerNames", "");

        string[] playerNames = allPlayerNames.Split(';');

        foreach (string playerName in playerNames)
        {
            string jsonData = PlayerPrefs.GetString(playerName);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            GameObject playerRecord = Instantiate(playerRecordPrefab, contentArea);
            PlayerRecordUI playerRecordUI = playerRecord.GetComponent<PlayerRecordUI>();
            playerRecordUI.SetPlayerRecord(playerData.playerName, playerData.completedLevels);
        }
    }

    public void ReturnToMenu()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
