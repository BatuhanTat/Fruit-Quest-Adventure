using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private int lastPlayedScene;
    private int unlockedLevels; // Initially, only the first level is unlocked


    float levelLoadDelay = 0.2f;
    int levelCount = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        // Minus booting scene.
        levelCount = SceneManager.sceneCountInBuildSettings - 1;
        unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1); // Load the unlockedLevels value from PlayerPrefs
    }

    public void CompleteLevel()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Active Level Index: " + levelIndex);
        if (levelIndex == unlockedLevels)
        {
            Debug.Log("Level Index " + levelIndex + "  unlockedLevels: " + unlockedLevels);
            if (levelIndex == 1 || IsPreviousLevelCompleted(levelIndex))
            {
                unlockedLevels++;
                PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels);

                PlayerPrefs.SetInt("Level_" + levelIndex, 1);
                PlayerPrefs.Save(); // Optional: Manually save PlayerPrefs
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            //Invoke(nameof(LoadDelayedScene), levelLoadDelay);
            StartCoroutine(LoadingDelay(sceneName));
        }
    }
    public void LoadNextlevel()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        if (levelIndex + 1 <= PlayerPrefs.GetInt("UnlockedLevels") && levelIndex + 1 <= levelCount)
        {
            StartCoroutine(LoadingDelay(levelIndex + 1));
        }
        //if(levelIndex > levelCount)
        else
        {
            // On completion of final level load level 1, buildindex 1.
            StartCoroutine(LoadingDelay(1));
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerPrefs.SetInt("LastPlayedLevel", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save(); // Optional: Manually save PlayerPrefs
    }


    private IEnumerator LoadingDelay(object arg)
    {
        yield return new WaitForSeconds(levelLoadDelay);

        if (arg is string)
        {
            SceneManager.LoadScene((string)arg);
        }
        else if (arg is int)
        {
            SceneManager.LoadScene((int)arg);
        }
    }
    private bool IsPreviousLevelCompleted(int levelIndex)
    {
        if (levelIndex == 0) // First level has no previous level
        {
            return true;
        }

        return PlayerPrefs.GetInt("Level_" + (levelIndex - 1), 0) == 1;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
