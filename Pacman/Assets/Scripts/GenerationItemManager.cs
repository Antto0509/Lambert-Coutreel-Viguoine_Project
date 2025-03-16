using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationItemManager : MonoBehaviour
{
    // Tilemap contenant les cellules de la route où les PacGommes seront placées
    public Tilemap roadTilemap;

    // Préfabriqué de la PacGomme normale
    public GameObject pacGommePrefab;
    // Préfabriqué de la Super PacGomme
    public GameObject SuperPacGommePrefab;

    // Liste des objets PacGomme présents dans la scène
    public List<GameObject> pacGommeLst;

    // Compteur pour limiter le nombre de Super PacGommes générées
    public int SuperPacGommeCount = 0;
    
    // Compteur de PacGommes générées
    public int PacGommeCount = 0;

    /// <summary>
    /// Initialise la génération des PacGommes sur toutes les cellules de la route au démarrage du jeu.
    /// </summary>
    void Start()
    {
        SetPacGommeOnAllRoadCell();
    }

    /// <summary>
    /// Vérifie en permanence le nombre de PacGommes restantes et les régénère si nécessaire.
    /// </summary>
    void Update()
    {
        CheckPacGommeCount();
    }

    /// <summary>
    /// Vérifie le nombre de PacGommes & SuperPacGomme restantes dans la scène.
    /// Si aucune PacGomme n'est trouvée, régénère les PacGommes sur toutes les cellules de la route.
    /// </summary>
    private void CheckPacGommeCount()
    {
        pacGommeLst.Clear();
        pacGommeLst.AddRange(GameObject.FindGameObjectsWithTag("PacGomme"));
        pacGommeLst.AddRange(GameObject.FindGameObjectsWithTag("SuperPacGomme"));
        if (pacGommeLst.Count == 0)
        {
            // Debug.Log("Fin de plateau !");
            SetPacGommeOnAllRoadCell();
        }
    }

    /// <summary>
    /// Place des PacGommes et des Super PacGommes sur toutes les cellules de la route.
    /// </summary>
    public void SetPacGommeOnAllRoadCell(bool test = false)
    {
        // Parcourt toutes les positions des cellules dans les limites de la Tilemap
        foreach (Vector3Int position in roadTilemap.cellBounds.allPositionsWithin)
        {
            // Vérifie si une tuile est présente à cette position
            if (roadTilemap.HasTile(position))
            {
                // Convertit la position de la cellule en position mondiale
                Vector3 worldPosition = roadTilemap.GetCellCenterWorld(position);
                worldPosition.z = 0; // Assure que la position Z est à 0

                // Génère une Super PacGomme avec une chance donnée, si le nombre maximum n'est pas atteint
                if (GetRandomBoolWithChance(1) && SuperPacGommeCount < 5)
                {
                    Instantiate(SuperPacGommePrefab, worldPosition, Quaternion.identity);
                    SuperPacGommeCount += 1; // Incrémente le compteur de Super PacGommes
                }
                else
                {
                    // Génère une PacGomme normale
                    Instantiate(pacGommePrefab, worldPosition, Quaternion.identity);
                    PacGommeCount += 1;
                }
            }
        }

        // si apres une première génération, on a pas les 5 SuperPacGomme, on refait un tour jusqu'à avoir toute les SuperPacGomme
        while (SuperPacGommeCount < 5)
        {
            foreach (Vector3Int position in roadTilemap.cellBounds.allPositionsWithin)
            {
                // Vérifie si une tuile est présente à cette position
                if (roadTilemap.HasTile(position))
                {
                    // Convertit la position de la cellule en position mondiale
                    Vector3 worldPosition = roadTilemap.GetCellCenterWorld(position);
                    worldPosition.z = 0; // Assure que la position Z est à 0
        
                    
                    // Si on peut placer une Super PacGomme et que le nombre maximum n'est pas atteint
                    if (GetRandomBoolWithChance(1) && SuperPacGommeCount < 5)
                    {
                        // Vérifie s'il y a déjà une Super PacGomme ou une PacGomme à cette position
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 0.1f); // Rayon petit pour éviter les collisions indésirables
        
                        foreach (Collider2D collider in colliders)
                        {
                            if (collider.CompareTag("SuperPacGomme"))
                            {
                                // Il y a déjà une Super PacGomme ici, on ne peut pas en placer une nouvelle
                                break;
                            }
        
                            if (collider.CompareTag("PacGomme"))
                            {
                                // Il y a une PacGomme ici, on la détruit avant de placer une Super PacGomme
                                if (test)
                                {
                                    DestroyImmediate(collider.gameObject);
                                }
                                else
                                {
                                    Destroy(collider.gameObject);
                                }
                                Instantiate(SuperPacGommePrefab, worldPosition, Quaternion.identity);
                                SuperPacGommeCount += 1; // Incrémente le compteur de Super PacGommes
                                PacGommeCount -= 1;
                            }
                        }
                        
                        
                    }
                }
            }
        }
    }

    /// <summary>
    /// Génère un booléen aléatoire avec une chance donnée.
    /// </summary>
    /// <param name="chance">La chance en pourcentage (entre 0 et 100) de retourner true.</param>
    /// <returns>True si un nombre aléatoire est inférieur à la chance donnée, sinon false.</returns>
    bool GetRandomBoolWithChance(int chance)
    {
        // Génère un nombre aléatoire entre 0 et 99
        int randomValue = Random.Range(0, 100);

        // Retourne true si le nombre aléatoire est inférieur à la chance donnée
        return randomValue < chance;
    }
}