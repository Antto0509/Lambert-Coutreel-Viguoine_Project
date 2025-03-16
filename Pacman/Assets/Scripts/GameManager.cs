using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameWinUi;
    public GameObject gameOverUi;
    
    public HealthManager healthManager;
    public ScoreManager scoreManager;
    public GenerationItemManager generationItemManager;
    public PlayerMovement playerMovement;
    
    public GameObject ghostRed;
    public GameObject ghostPink;
    public GameObject ghostBlue;
    public GameObject ghostOrange;
    
    /// <summary>
    /// Affiche l'écran de défaite et arrête le temps.
    /// </summary>
    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverUi.SetActive(true);
    }

    /// <summary>
    /// Affiche l'écran de victoire et arrête le temps.
    /// </summary>
    public void GameWin()
    {
        Time.timeScale = 0;
        gameWinUi.SetActive(true);
    }

    /// <summary>
    /// Réinitialise le jeu à son état initial.
    /// </summary>
    public void RestartGame()
    {
        gameOverUi.SetActive(false);
        gameWinUi.SetActive(false);
        
        Time.timeScale = 1;
        
        generationItemManager.SetPacGommeOnAllRoadCell();
        playerMovement.Restart();
        healthManager.ResetHeart();
        scoreManager.ResetScore();
        
        RestartGhosts();
    }

    /// <summary>
    /// Réinitialise les fantômes à leur état initial.
    /// </summary>
    private void RestartGhosts()
    {
        var ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (var ghost in ghosts)
        {
            Destroy(ghost);
        }
        
        Instantiate(ghostRed).transform.position = new Vector3(0, 3.5f, 0);
        Instantiate(ghostBlue).transform.position = new Vector3(-2.5f, 1.2f, 0);
        Instantiate(ghostOrange).transform.position = new Vector3(4.5f, 1.2f, 0);
        Instantiate(ghostPink).transform.position = new Vector3(1, -0.2f, 0);
        
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (var ghost in ghosts)
        {
            if (ghost.name == ghostRed.name)
            {
                var ghostMovement = ghost.GetComponent<GhostMovement>();
                ghostMovement.isInHouse = false;
                ghostMovement.wantToExit = false;
                ghostMovement.dead = false;
                ghostMovement.wait = false;
                ghostMovement.sortieStatus = 3;
                ghostMovement.ChooseNewDirection();
            }
        }
    }
}
