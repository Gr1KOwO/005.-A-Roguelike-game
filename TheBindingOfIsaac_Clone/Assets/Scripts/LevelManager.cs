using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int level;
    public static LevelManager Instance;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            level = 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIController.instance.LevelText.text = $"Level: {level}";
    }

    public void ResetLevel()
    {
        level = 1;
        UIController.instance.LevelText.text = $"Level: {level}";
    }

    public int GetLevel()
    {
        return level;
    }

    public void UpdateLevel()
    {
        level++;
        UIController.instance.LevelText.text = $"Level: {level}";
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
