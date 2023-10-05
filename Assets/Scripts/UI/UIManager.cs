using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Match3 match3;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI targetScoreText;
    [Header("Complete Popup")]
    [SerializeField] private Transform popupWinTransform;
    [SerializeField] private TextMeshProUGUI scoreWinText;
    [SerializeField] Image[] starsComplete;
    [Header("Fail Popup")]
    [SerializeField] private Transform popupLoseTransform;
    [SerializeField] private TextMeshProUGUI scoreLoseText;
    [SerializeField] private TextMeshProUGUI targetScoreLoseText;
    [SerializeField] Image[] starsFail;

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
        popupWinTransform.gameObject.SetActive(true); 
        scoreWinText.text = match3.GetScore().ToString();
        UpdateStars(starsComplete);
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e)
    {
            popupLoseTransform.gameObject.SetActive(true);
            scoreLoseText.text = match3.GetScore().ToString();
            UpdateStars(starsFail);
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
    public float NormalizeScore(int score)
    {
        float normalizedScore = (float)score / levelSO.targetScore;
        return normalizedScore;
    }
}
