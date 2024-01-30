using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Slider healthSlider;
    public TMP_Text healthText;

    public Slider gainSlider;
    public TMP_Text gainText;

    public TMP_Text attackText;
    public TMP_Text bombCountText;

    [SerializeField] private GameObject gain;

    [SerializeField] private GameObject bossHealth;
    public Slider bossHealthSlider;
    public TMP_Text bossNameText;

    public TMP_Text LevelText;

    public GameObject deathScreen;
    public GameObject pauseScreen;

    [SerializeField] private string mainMenu;

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

    public void VisualGain(bool visual)
    {
        gain.SetActive(visual);
    }

    public void VisualBossHealth(bool visual)
    {
        bossHealth.SetActive(visual);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        pauseScreen.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        pauseScreen.SetActive(true);
    }

    public void ReturnMainMenu()
    {
        Restart.instance.RestartLoop();
        pauseScreen.SetActive(false);
        SceneManager.LoadScene(mainMenu);
    }
}
