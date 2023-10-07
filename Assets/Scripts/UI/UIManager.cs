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
    [SerializeField] private TextMeshProUGUI levelText;
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
    [Header("Levels Panel")]
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] ButtonStateHandler buttonStateHandler;

    private LevelSO levelSO;
    public bool onPause { get; private set; } = false;

    private void Awake()
    {
        match3.OnLevelSet += Match3_OnLevelSet;
        match3.OnMoveUsed += Match3_OnMoveUsed;
        match3.OnGlassDestroyed += Match3_OnGlassDestroyed;
        match3.OnScoreChanged += Match3_OnScoreChanged;

        match3.OnOutOfMoves += Match3_OnOutOfMoves;
        match3.OnWin += Match3_OnWin;
    }

    private void Start()
    {
        SetLevelText();
    }
    private void Match3_OnWin(object sender, System.EventArgs e)
    {
        popupWinGameObject.SetActive(true);
        scoreWinText.text = match3.GetScore().ToString();
        UpdateStars(starsComplete);
        SFX_Manager.instance.PlaySFX(ClipType.LevelComplete);
        GameManager.instance.CompleteLevel();
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
        {
            starsArr[0].gameObject.SetActive(true);
            starsArr[0].GetComponent<StarAnimation>().PlayStarAnimation(starsArr[0].gameObject);
        }
        else if (normalizedScore > 0.66f && normalizedScore < 0.99f)
        {
            starsArr[0].gameObject.SetActive(true);
            starsArr[1].gameObject.SetActive(true);
            starsArr[0].GetComponent<StarAnimation>().PlayStarAnimation(starsArr[0].gameObject);
            starsArr[1].GetComponent<StarAnimation>().PlayStarAnimation(starsArr[1].gameObject);
        }
        else if (normalizedScore >= 1.0f)
        {
            starsArr[0].gameObject.SetActive(true);
            starsArr[1].gameObject.SetActive(true);
            starsArr[2].gameObject.SetActive(true);
            starsArr[0].GetComponent<StarAnimation>().PlayStarAnimation(starsArr[0].gameObject);
            starsArr[1].GetComponent<StarAnimation>().PlayStarAnimation(starsArr[1].gameObject);
            starsArr[2].GetComponent<StarAnimation>().PlayStarAnimation(starsArr[2].gameObject);
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

    public void TogglePausePanel()
    {
        pauseMenuGameObject.SetActive(!pauseMenuGameObject.activeSelf);
        BGMusic.instance.BGPauseToggle(pauseMenuGameObject.activeSelf);
        onPause = pauseMenuGameObject.activeSelf;
    }

    public void Toggle_SFX(RectTransform rectTransform)
    {
        SoundToggleButtonsVisual(rectTransform);
        SFX_Manager.instance.audioSource.volume = SFX_Manager.instance.audioSource.volume == 0.0f ? 1.0f : 0.0f;
    }
    public void Toggle_BG(RectTransform rectTransform)
    {
        SoundToggleButtonsVisual(rectTransform);
        BGMusic.instance.audioSource.volume = BGMusic.instance.audioSource.volume == 0.0f ? BGMusic.instance.pauseVolume : 0.0f;
        BGMusic.instance.isMuted = (!BGMusic.instance.isMuted);
    }
    public void ToggleLevelsPanel()
    {
        levelsPanel.SetActive(!levelsPanel.activeSelf);
        buttonStateHandler.SetLevelButtons(PlayerPrefs.GetInt("UnlockedLevels", 1));
    }

    public void SelectLevel(Button button)
    {
        GameManager.instance.LoadLevel(button.name);
        Debug.Log("Clicked button name: " + button.name);
        StartCoroutine(ToggleUIDelay());
    }

    public void LoadNextLevelButton()
    { GameManager.instance.LoadNextlevel(); }

    public void Quit()
    { Application.Quit(); }

    private void SetLevelText()
    { levelText.text = "Level: " + SceneManager.GetActiveScene().buildIndex.ToString(); }

    private void SoundToggleButtonsVisual(RectTransform rectTransform)
    {
        bool isOn = rectTransform.GetComponent<Toggle>().isOn;
        rectTransform.anchoredPosition = isOn ? new Vector2(190.0f, 0.0f) : Vector2.zero;
    }
    private IEnumerator ToggleUIDelay()
    {
        yield return new WaitForSeconds(0.2f);
        ToggleLevelsPanel();
    }
}
