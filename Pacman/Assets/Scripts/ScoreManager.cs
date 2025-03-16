using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public TMP_Text scoreText;

    /// <summary>
    /// Initialise le score et met à jour l'affichage au démarrage.
    /// </summary>
    private void Start()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// Ajoute un certain nombre de points au score actuel et met à jour l'affichage.
    /// </summary>
    /// <param name="scoreToAdd">Le nombre de points à ajouter.</param>
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// Réinitialise le score à zéro et met à jour l'affichage.
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }
}