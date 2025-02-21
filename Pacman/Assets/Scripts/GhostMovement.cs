using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostMovement : MonoBehaviour
{
    public enum GhostType { Blinky, Pinky, Inky, Clyde }    // Types de fantômes
    public GhostType ghostType;                             // Type du fantôme défini dans l'Inspector

    public float moveSpeed = 2f;                            // Vitesse de déplacement du fantôme
    public float snapTolerance = 0.1f;                      // Marge de tolérance pour l'alignement sur une case
    public Tilemap roadTilemap;                             // Référence au Tilemap des routes
    public Rigidbody2D rb;                                  // Référence au Rigidbody2D du fantôme
    public Transform pacman;                                // Référence à Pac-Man
    public Vector2 scatterTarget;                           // Point de dispersion propre à chaque fantôme

    private Vector2 _targetDirection;                       // Direction de déplacement actuelle
    private Vector2 _lastDirection;                         // Dernière direction de déplacement
    private enum GhostState { Chase, Scatter, Frightened }  // États du fantôme
    private GhostState _currentState;                       // État actuel du fantôme

    private void Start()
    {
        AlignToTileCenter();
        ChooseNewDirection();
        _currentState = GhostState.Scatter; // Commence en mode dispersion
        AssignScatterTarget(); // Assigner le point de dispersion selon le fantôme
    }

    private void Update()
    {
        if (IsCenteredOnTile())
        {
            if (!CanMoveInDirection(_targetDirection))
            {
                ChooseNewDirection();
            }
        }
        rb.linearVelocity = _targetDirection * moveSpeed;
    }

    /// <summary>
    /// Aligne le fantôme sur le centre de la case actuelle
    /// </summary>
    private void AlignToTileCenter()
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(transform.position);
        transform.position = roadTilemap.GetCellCenterWorld(cellPosition);
    }

    /// <summary>
    /// Vérifie si le fantôme est centré sur une case
    /// </summary>
    /// <returns>Vrai si le fantôme est centré sur une case, faux sinon</returns>
    private bool IsCenteredOnTile()
    {
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = roadTilemap.WorldToCell(worldPosition);
        Vector3 tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);
        return Vector2.Distance(worldPosition, tileCenter) <= snapTolerance;
    }

    /// <summary>
    /// Vérifie si le fantôme peut se déplacer dans une direction donnée
    /// </summary>
    /// <param name="direction">Direction de déplacement</param>
    /// <returns>Vrai si le fantôme peut se déplacer dans la direction donnée, faux sinon</returns>
    private bool CanMoveInDirection(Vector2 direction)
    {
        Vector3Int targetPosition = roadTilemap.WorldToCell(transform.position + (Vector3)direction);
        return roadTilemap.HasTile(targetPosition);
    }

    /// <summary>
    /// Choisi une nouvelle direction de déplacement pour le fantôme
    /// </summary>
    private void ChooseNewDirection()
    {
        List<Vector2> availableDirections = new List<Vector2>();
        Vector2[] allDirections = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

        foreach (Vector2 dir in allDirections)
        {
            if (CanMoveInDirection(dir))
            {
                availableDirections.Add(dir);
            }
        }

        if (availableDirections.Count > 2)
        {
            availableDirections.Remove(-_lastDirection);
        }

        if (_currentState == GhostState.Chase)
        {
            _targetDirection = GetChaseDirection(availableDirections);
        }
        else if (_currentState == GhostState.Frightened)
        {
            _targetDirection = GetFarthestDirectionFromPacman(availableDirections);
        }
        else
        {
            _targetDirection = GetBestDirectionToPoint(scatterTarget, availableDirections);
        }

        _lastDirection = _targetDirection;
    }

    /// <summary>
    /// Détermine la direction de déplacement pour poursuivre Pac
    /// </summary>
    /// <param name="directions">Liste des directions possibles</param>
    /// <returns>Direction de déplacement</returns>
    private Vector2 GetChaseDirection(List<Vector2> directions)
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                return GetBestDirectionToPoint(pacman.position, directions); // Suit Pac-Man en ligne droite
            case GhostType.Pinky:
                return GetBestDirectionToPoint((Vector2)pacman.position + (Vector2)pacman.up * 4, directions); // Anticipe Pac-Man
            case GhostType.Inky:
                return Random.value > 0.5f ? GetBestDirectionToPoint(pacman.position, directions) : GetFarthestDirectionFromPacman(directions);
            case GhostType.Clyde:
                return Vector2.Distance(transform.position, pacman.position) > 8f ? GetBestDirectionToPoint(pacman.position, directions) : GetBestDirectionToPoint(scatterTarget, directions);
            default:
                return directions[Random.Range(0, directions.Count)];
        }
    }

    /// <summary>
    /// Détermine la meilleure direction pour atteindre un point donné
    /// </summary>
    /// <param name="target">Point cible</param>
    /// <param name="directions">Liste des directions possibles</param>
    /// <returns>Direction de déplacement</returns>
    private Vector2 GetBestDirectionToPoint(Vector2 target, List<Vector2> directions)
    {
        Vector2 bestDirection = directions[0];
        float shortestDistance = float.MaxValue;

        foreach (Vector2 dir in directions)
        {
            Vector2 nextPosition = (Vector2)transform.position + dir;
            float distance = Vector2.Distance(nextPosition, target);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                bestDirection = dir;
            }
        }
        return bestDirection;
    }

    /// <summary>
    /// Détermine la direction la plus éloignée de Pac-Man
    /// </summary>
    /// <param name="directions">Liste des directions possibles</param>
    /// <returns>Direction de déplacement</returns>
    private Vector2 GetFarthestDirectionFromPacman(List<Vector2> directions)
    {
        Vector2 bestDirection = directions[0];
        float longestDistance = 0f;

        foreach (Vector2 dir in directions)
        {
            Vector2 nextPosition = (Vector2)transform.position + dir;
            float distance = Vector2.Distance(nextPosition, pacman.position);
            if (distance > longestDistance)
            {
                longestDistance = distance;
                bestDirection = dir;
            }
        }
        return bestDirection;
    }

    /// <summary>
    /// Assigner le point de dispersion selon le fantôme
    /// </summary>
    private void AssignScatterTarget()
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                scatterTarget = new Vector2(20, 20); // Coin supérieur droit
                break;
            case GhostType.Pinky:
                scatterTarget = new Vector2(-20, 20); // Coin supérieur gauche
                break;
            case GhostType.Inky:
                scatterTarget = new Vector2(20, -20); // Coin inférieur droit
                break;
            case GhostType.Clyde:
                scatterTarget = new Vector2(-20, -20); // Coin inférieur gauche
                break;
        }
    }

    /// <summary>
    /// Définir l'état actuel du fantôme
    /// </summary>
    /// <param name="state">État du fantôme</param>
    public void SetGhostState(string state)
    {
        if (state == "chase") _currentState = GhostState.Chase;
        else if (state == "scatter") _currentState = GhostState.Scatter;
        else if (state == "frightened") _currentState = GhostState.Frightened;

        ChooseNewDirection();
    }
}
