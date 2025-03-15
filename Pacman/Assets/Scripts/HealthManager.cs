using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heartPrefab;
   
    public Transform heartsParent;
   
    public int maxHearts = 20;
    public int currentHealth = 3;
    
    public GameObject[] heartObjects;

    /// <summary>
    /// Initialise les cœurs et met à jour leur affichage au démarrage du jeu.
    /// </summary>
    void Start()
    {
        InitializeHearts();
        UpdateHearts();
    }

    /// <summary>
    /// Initialise les cœurs en instanciant le nombre maximum de cœurs et en les stockant dans un tableau.
    /// </summary>
    public void InitializeHearts()
    {
        heartObjects = new GameObject[maxHearts];

        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsParent);
            heartObjects[i] = heart;
        }
    }

    /// <summary>
    /// Met à jour l'affichage des cœurs en fonction de la santé actuelle.
    /// </summary>
    private void UpdateHearts()
    {
        for (int i = 0; i < maxHearts; i++)
        {
            heartObjects[i].SetActive(i < currentHealth);
        }
    }

    /// <summary>
    /// Ajoute de la santé au joueur et met à jour l'affichage des cœurs.
    /// </summary>
    /// <param name="newHealth">La quantité de santé à ajouter.</param>
    public void AddHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth + newHealth, 0, maxHearts);
        UpdateHearts();
    }

    /// <summary>
    /// Réduit la santé du joueur et met à jour l'affichage des cœurs.
    /// </summary>
    /// <param name="newHealth">La quantité de santé à retirer.</param>
    public void DecreaseHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth - newHealth, 0, maxHearts);
        UpdateHearts();
    }
}