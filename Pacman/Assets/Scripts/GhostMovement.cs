using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GhostMovement : MonoBehaviour
{
    public enum GhostType { Blinky, Pinky, Inky, Clyde }
    public GhostType ghostType;

    public float moveSpeed = 2f;
    public float snapTolerance = 0.001f;
    public Tilemap roadTilemap;
    public Rigidbody2D rb;
    public Transform pacman;
    public Transform respawnPointGhost;
    public bool isInHouse = true;
    private Vector2 _targetDirection;
    private Vector2 _lastDirection;

    public enum GhostState { Locked, Scatter, Chase, Frightened }
    public GhostState _currentState;
    
    public GameObject seeUp;
    public GameObject seeDown;
    public GameObject seeLeft;
    public GameObject seeRight;
    
    public GameObject pointSpawn;
    public GameObject point;

    /// <summary>
    /// Initialise le fantôme.
    /// </summary>
    private void Start()
    {
        AlignToTileCenter();
        ChooseNewDirection();

        if (ghostType == GhostType.Blinky)
        {
            _currentState = GhostState.Scatter;
            isInHouse = false;
        }
        else
        {
            _currentState = GhostState.Locked;
            StartCoroutine(ReleaseFromHouse());
        }
        UpdateSprite();
    }

    /// <summary>
    /// Met à jour la position du fantôme.
    /// </summary>
    private void Update()
    {
        if (isInHouse)
        {
            MoveInsideHouse();
            return;
        }

        if (IsCenteredOnTile())
        {
            // Si le fantôme est centré sur une tuile, il choisit une nouvelle direction
            if (!CanMoveInDirection(_targetDirection))
            {
                
            }
            ChooseNewDirection();
        }
        rb.linearVelocity = _targetDirection * moveSpeed;
        UpdateSprite();
    }

    /// <summary>
    /// Libère le fantôme de la maison après un certain délai.
    /// </summary>
    /// <returns> Coroutine. </returns>
    private IEnumerator ReleaseFromHouse()
    {
        List<Vector2> directions = new List<Vector2> { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        
        float delay = ghostType switch
        {
            GhostType.Pinky => 5f,
            GhostType.Inky  => 10f,
            GhostType.Clyde => 15f,
            _ => 0f
        };
        
        yield return new WaitForSeconds(delay);
        
        // Se déplace vers "pointSpawn" et ensuite vers "point"
        _targetDirection = GetBestDirectionToPoint(pointSpawn.transform.position, directions);
        yield return new WaitForSeconds(2f);
        _targetDirection = GetBestDirectionToPoint(point.transform.position, directions);
        yield return new WaitForSeconds(2f);
        
        _currentState = GhostState.Scatter;
        isInHouse = false;
        
        ChooseNewDirection();
    }

    /// <summary>
    /// Déplace le fantôme à l'intérieur de la maison.
    /// </summary>
    private void MoveInsideHouse()
    {
        float oscillationSpeed = 1f;
        transform.position += Vector3.up * (Mathf.Sin(Time.time * oscillationSpeed) * Time.deltaTime);
    }

    /// <summary>
    /// Centre le fantôme sur une tuile.
    /// </summary>
    private void AlignToTileCenter()
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(transform.position);
        transform.position = roadTilemap.GetCellCenterWorld(cellPosition);
    }

    /// <summary>
    /// Vérifie si le fantôme est centré sur une tuile.
    /// </summary>
    /// <returns> Vrai si le fantôme est centré sur une tuile, faux sinon. </returns>
    private bool IsCenteredOnTile()
    {
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = roadTilemap.WorldToCell(worldPosition);
        Vector3 tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);
        return Vector2.Distance(worldPosition, tileCenter) <= snapTolerance;
    }

    /// <summary>
    /// Vérifie si le fantôme peut se déplacer dans une direction donnée.
    /// </summary>
    /// <param name="direction"> Direction à vérifier. </param>
    /// <returns> Vrai si le fantôme peut se déplacer dans la direction donnée, faux sinon. </returns>
    private bool CanMoveInDirection(Vector2 direction)
    {
        Vector3Int targetPosition = roadTilemap.WorldToCell(transform.position + (Vector3)direction);
        return roadTilemap.HasTile(targetPosition);
    }

    /// <summary>
    /// Choisi une nouvelle direction pour le fantôme.
    /// </summary>
    private void ChooseNewDirection()
    {
        if (isInHouse) return;

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
            do
            {
                _targetDirection = availableDirections[Random.Range(0, availableDirections.Count)];
                Debug.Log("j'ai choisi une direction");
            }while (_targetDirection == _lastDirection);
        }

        _lastDirection = _targetDirection;
    }

    /// <summary>
    /// Retourne la direction à suivre pour chasser Pacman.
    /// </summary>
    /// <param name="directions"> Liste des directions possibles. </param>
    /// <returns> La direction à suivre pour chasser Pacman. </returns>
    private Vector2 GetChaseDirection(List<Vector2> directions)
    {
        switch (ghostType)
        {
            case GhostType.Blinky: // Chasse Pacman
                return GetBestDirectionToPoint(pacman.position, directions);
            case GhostType.Pinky: // Cible 4 cases devant Pacman
                return GetBestDirectionToPoint((Vector2)pacman.position + (Vector2)pacman.up * 4, directions);
            case GhostType.Inky: // Cible la position symétrique de Blinky par rapport à Pacman
                return Random.value > 0.5f ? GetBestDirectionToPoint(pacman.position, directions) : GetFarthestDirectionFromPacman(directions);
            case GhostType.Clyde: // Chasse Pacman si distance > 8, sinon retourne à la maison
                return Vector2.Distance(transform.position, pacman.position) > 8f ? GetBestDirectionToPoint(pacman.position, directions) : GetBestDirectionToPoint(respawnPointGhost.position, directions);
            default:
                return directions[Random.Range(0, directions.Count)];
        }
    }

    /// <summary>
    /// Retourne la direction la plus courte pour atteindre un point donné.
    /// </summary>
    /// <param name="target"> Position cible. </param>
    /// <param name="directions"> Liste des directions possibles. </param>
    /// <returns> La direction la plus courte pour atteindre le point donné. </returns>
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
    /// Retourne la direction la plus éloignée de Pacman.
    /// </summary>
    /// <param name="directions"> Liste des directions possibles. </param>
    /// <returns> La direction la plus éloignée de Pacman. </returns>
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
    /// Met à jour le sprite du fantôme en fonction de sa direction.
    /// </summary>
    private void UpdateSprite()
    {
        if (_targetDirection == Vector2.right)
        {
            seeUp.SetActive(false);
            seeDown.SetActive(false);
            seeLeft.SetActive(false);
            seeRight.SetActive(true);
        }
        else if (_targetDirection == Vector2.left)
        {
            seeUp.SetActive(false);
            seeDown.SetActive(false);
            seeLeft.SetActive(true);
            seeRight.SetActive(false);
        }
        else if (_targetDirection == Vector2.up)
        {
            seeUp.SetActive(true);
            seeDown.SetActive(false);
            seeLeft.SetActive(false);
            seeRight.SetActive(false);
        }
        else if (_targetDirection == Vector2.down)
        {
            seeUp.SetActive(false);
            seeDown.SetActive(true);
            seeLeft.SetActive(false);
            seeRight.SetActive(false);
        }
    }
}
