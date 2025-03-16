using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public TMP_Text scoreText;

    private void Start()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
    }
}
