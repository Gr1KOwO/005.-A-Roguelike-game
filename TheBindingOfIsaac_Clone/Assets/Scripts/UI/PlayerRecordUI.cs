using TMPro;
using UnityEngine;

public class PlayerRecordUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text completedLevelsText;

    public void SetPlayerRecord(string playerName, string completedLevels)
    {
        playerNameText.text = playerName;
        completedLevelsText.text = completedLevels;
    }
}
