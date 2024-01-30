using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField]private GameObject player;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text errorMessageText;
    public static Restart instance;
    public bool isDeathMenuShow=false;
    public bool isPause=false;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!isDeathMenuShow && !isPause)
        {     
            if (Input.GetKey(KeyCode.R))
            {
                RestartLoop();
            } 
        }
        if (player == null)
        {
            Cursor.visible = true;
            UIController.instance.deathScreen.SetActive(true);
        }
        else
        {
            UIController.instance.deathScreen.SetActive(false);
        }
        if (UIController.instance.deathScreen.activeInHierarchy)
            isDeathMenuShow = true;
        else
            isDeathMenuShow = false;
        if(UIController.instance.pauseScreen.activeInHierarchy)
            isPause = true;
        else
            isPause = false;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void DiePlayer()
    {
       Destroy(player);
       Time.timeScale = 0;
    }

    public void RestartLoop()
    {
        Cursor.visible = false;
        Destroy(player);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        LevelManager.Instance.ResetLevel();
        UIController.instance.VisualBossHealth(false);
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void ReturnMenu()
    {
        string playerName = inputField.text.Trim();

        if (!string.IsNullOrEmpty(playerName))
        {
            // —оздаем объект PlayerData дл€ текущего игрока
            PlayerData playerData = new PlayerData();
            playerData.playerName = playerName;

            // ѕолучаем текущие данные об игроках
            string allPlayerNames = PlayerPrefs.GetString("AllPlayerNames", "");

            // ѕровер€ем, есть ли уже данные дл€ этого игрока
            if (allPlayerNames.Contains(playerName))
            {
                // ≈сли данные уже есть, загружаем их и обновл€ем пройденные уровни
                string json = PlayerPrefs.GetString(playerName);
                playerData = JsonUtility.FromJson<PlayerData>(json);

                // ѕример сохранени€ информации о пройденном уровне
                string completedLevel = $"Level: {LevelManager.Instance.GetLevel()}";
                playerData.completedLevels = completedLevel;
            }
            else
            {
                // ≈сли данных дл€ этого игрока еще нет, сохран€ем только новые пройденные уровни
                string completedLevel = $"Level: {LevelManager.Instance.GetLevel()}";
                playerData.completedLevels = completedLevel;

                // ƒобавл€ем им€ игрока в список всех игроков
                allPlayerNames += (allPlayerNames.Length > 0 ? ";" : "") + playerName;
                PlayerPrefs.SetString("AllPlayerNames", allPlayerNames);
            }

            // —охран€ем или обновл€ем данные текущего игрока
            string dataJson = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString(playerName, dataJson);

            RestartLoop();
            SceneManager.LoadScene(sceneToLoad);
            UIController.instance.VisualBossHealth(false);
        }
        else
        {
            errorMessageText.text = "Player name cannot be empty!";
        }
    }
}
