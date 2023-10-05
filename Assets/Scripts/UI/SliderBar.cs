using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField] private Match3 match3;
    [Header("Slider Bar")]
    [SerializeField] Slider slider;
    [SerializeField] Image[] stars;
    [SerializeField] Color starInactiveColor;
    [SerializeField] Color starActiveColor;

    private LevelSO levelSO;

    int targetScore;

    private void Awake()
    {
        match3.OnScoreChanged += Match3_OnScoreChanged;

        levelSO = match3.GetLevelSO();
        targetScore = levelSO.targetScore;
    
        starActiveColor.a = 1.0f;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        int score = match3.GetScore();
        //Debug.Log("Normalized score : " + NormalizeScore(score) + " Score : " + score);
        slider.value = NormalizeScore(score);
        UpdateStars(stars);
    }

    private void UpdateStars(Image[] starsArray)
    {
        int score = match3.GetScore();
        float normalizedScore = NormalizeScore(score);

        if (normalizedScore > 0.33f && normalizedScore < 0.66f)
        { starsArray[0].color = starActiveColor; }
        else if (normalizedScore > 0.66f && normalizedScore < 0.99f)
        {
            starsArray[0].color = starActiveColor;
            starsArray[1].color = starActiveColor;
        }
        else if (slider.value >= 1.0f)
        {
            starsArray[0].color = starActiveColor;
            starsArray[1].color = starActiveColor;
            starsArray[2].color = starActiveColor;
        }
    }

    public float NormalizeScore(int score)
    {
        float normalizedScore = (float)score / targetScore;
        return normalizedScore;
    }

    private void Match3_OnScoreChanged(object sender, System.EventArgs e)
    {
        UpdateSlider();
    }
}
