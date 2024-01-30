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
        // ќчищаем содержимое ScrollView перед загрузкой новых записей
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // ѕолучаем строку со всеми именами игроков
        string allPlayerNames = PlayerPrefs.GetString("AllPlayerNames", "");

        // –азбиваем строку на массив имен игроков
        string[] playerNames = allPlayerNames.Split(';');

        // ѕроходимс€ по каждому имени игрока и создаем запись дл€ него
        foreach (string playerName in playerNames)
        {
            // ѕолучаем данные игрока по его имени
            string jsonData = PlayerPrefs.GetString(playerName);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            // —оздаем запись дл€ каждого игрока и добавл€ем ее в ScrollView
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
