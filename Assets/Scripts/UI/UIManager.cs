using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Match3 match3;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI targetScoreText;
    [Header("Complete Popup")]
    [SerializeField] private GameObject popupWinGameObject;
    [SerializeField] private TextMeshProUGUI scoreWinText;
    [SerializeField] Image[] starsComplete;
    [Header("Fail Popup")]
    [SerializeField] private GameObject popupLoseGameObject;
    [SerializeField] private TextMeshProUGUI scoreLoseText;
    [SerializeField] private TextMeshProUGUI targetScoreLoseText;
    [SerializeField] Image[] starsFail;
    [Header("Pause Popup")]
    [SerializeField] private GameObject pauseMenuGameObject;
    [SerializeField] private GameObject SFX_Button;
    [SerializeField] private GameObject BG_Button;

    private LevelSO levelSO;

    private void Awake()
    {
        match3.OnLevelSet += Match3_OnLevelSet;
        match3.OnMoveUsed += Match3_OnMoveUsed;
        match3.OnGlassDestroyed += Match3_OnGlassDestroyed;
        match3.OnScoreChanged += Match3_OnScoreChanged;

        match3.OnOutOfMoves += Match3_OnOutOfMoves;
        match3.OnWin += Match3_OnWin;
    }
    private void Match3_OnWin(object sender, System.EventArgs e)
    {
        popupWinGameObject.SetActive(true);
        scoreWinText.text = match3.GetScore().ToString();
        UpdateStars(starsComplete);
        SFX_Manager.instance.PlaySFX(ClipType.LevelComplete);
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e)
    {
        popupLoseGameObject.SetActive(true);
        scoreLoseText.text = match3.GetScore().ToString();
        UpdateStars(starsFail);
        SFX_Manager.instance.PlaySFX(ClipType.LevelLose);
    }

    private void Match3_OnScoreChanged(object sender, System.EventArgs e)
    {
        UpdateText();
    }

    private void Match3_OnGlassDestroyed(object sender, System.EventArgs e)
    {
        UpdateText();
    }

    private void Match3_OnMoveUsed(object sender, System.EventArgs e)
    {
        UpdateText();
    }
    private void Match3_OnLevelSet(object sender, System.EventArgs e)
    {
        levelSO = match3.GetLevelSO();

        switch (levelSO.goalType)
        {
            default:
            case LevelSO.GoalType.Score:

                targetScoreText.gameObject.SetActive(true);

                targetScoreText.text = levelSO.targetScore.ToString() + " Score";
                targetScoreLoseText.text = levelSO.targetScore.ToString();
                break;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        movesText.text = match3.GetMoveCount().ToString();
        scoreText.text = match3.GetScore().ToString();
    }

    private void UpdateStars(Image[] starsArr)
    {
        int score = match3.GetScore();
        float normalizedScore = NormalizeScore(score);

        if (normalizedScore > 0.33f && normalizedScore < 0.66f)
        { starsArr[0].gameObject.SetActive(true); }
        else if (normalizedScore > 0.66f && normalizedScore < 0.99f)
        {
            starsArr[0].gameObject.SetActive(true);
            starsArr[1].gameObject.SetActive(true);
        }
        else if (normalizedScore >= 1.0f)
        {
            starsArr[0].gameObject.SetActive(true);
            starsArr[1].gameObject.SetActive(true);
            starsArr[2].gameObject.SetActive(true);
        }
    }
    private float NormalizeScore(int score)
    {
        float normalizedScore = (float)score / levelSO.targetScore;
        return normalizedScore;
    }

    public void ReloadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // There are no more scenes in the build order, so reload the first scene (index 0).
            SceneManager.LoadScene(0);
        }
    }
    public void TogglePausePanel()
    {
        pauseMenuGameObject.SetActive(!pauseMenuGameObject.activeSelf);
        BGMusic.instance.BGPauseToggle(pauseMenuGameObject.activeSelf);
    }

    public void Toggle_SFX(RectTransform rectTransform)
    {
        TogglePauseButtonVisual(rectTransform);
        SFX_Manager.instance.audioSource.volume = SFX_Manager.instance.audioSource.volume == 0.0f ? 1.0f : 0.0f;
    }
    public void Toggle_BG(RectTransform rectTransform)
    {
        TogglePauseButtonVisual(rectTransform);
        BGMusic.instance.audioSource.volume = BGMusic.instance.audioSource.volume == 0.0f ? BGMusic.instance.pauseVolume : 0.0f;
        BGMusic.instance.isMuted = (!BGMusic.instance.isMuted);
    }

    private void TogglePauseButtonVisual(RectTransform rectTransform)
    {
        bool isOn = rectTransform.GetComponent<Toggle>().isOn;
        rectTransform.anchoredPosition = isOn ? new Vector2(190.0f, 0.0f) : Vector2.zero;
    }
}
