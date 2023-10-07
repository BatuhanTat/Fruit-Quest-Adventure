using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ASyncLoader : MonoBehaviour
{

    [Header("Slider")]
    [SerializeField] Slider loadingSlider;
    private void Start()
    {
        StartCoroutine(LoadLevelASync(PlayerPrefs.GetInt("LastPlayedLevel", 1)));
    }

    IEnumerator LoadLevelASync(int levelToLoad)
    {
        yield return new WaitForSeconds(0.02f);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}
