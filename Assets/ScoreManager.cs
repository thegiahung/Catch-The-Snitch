using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Make sure this namespace is included

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score;
    void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
