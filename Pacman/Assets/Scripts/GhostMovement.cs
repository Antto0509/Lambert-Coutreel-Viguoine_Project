using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GhostMovement : MonoBehaviour
{
    public enum GhostType { Blinky, Pinky, Inky, Clyde }
    public GhostType ghostType;

    public float moveSpeed = 2f;
    public float snapTolerance = 0.1f;
    public Tilemap roadTilemap;
    public Rigidbody2D rb;
    public Transform pacman;
    public Transform ghostHouseExit; 
    public bool isInHouse = true;
    private Vector2 _targetDirection;
    private Vector2 _lastDirection;
    
    private enum GhostState { Locked, Scatter, Chase, Frightened }
    private GhostState _currentState;
    
    public GameObject seeUp;
    public GameObject seeDown;
    public GameObject seeLeft;
    public GameObject seeRight;

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

    private void Update()
    {
        if (isInHouse)
        {
            MoveInsideHouse();
            return;
        }

        if (IsCenteredOnTile())
        {
            if (!CanMoveInDirection(_targetDirection))
            {
                ChooseNewDirection();
            }
        }
        rb.linearVelocity = _targetDirection * moveSpeed;
        UpdateSprite();
    }

    private IEnumerator ReleaseFromHouse()
    {
        float delay = ghostType switch
        {
            GhostType.Pinky => 5f,
            GhostType.Inky  => 10f,
            GhostType.Clyde => 15f,
            _ => 0f
        };

        yield return new WaitForSeconds(delay);
        _currentState = GhostState.Scatter;
        isInHouse = false;

        while (Vector2.Distance(transform.position, ghostHouseExit.position) > snapTolerance)
        {
            transform.position = Vector2.MoveTowards(transform.position, ghostHouseExit.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        ChooseNewDirection();
    }

    private void MoveInsideHouse()
    {
        float oscillationSpeed = 1f;
        transform.position += Vector3.up * (Mathf.Sin(Time.time * oscillationSpeed) * Time.deltaTime);
    }

    private void AlignToTileCenter()
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(transform.position);
        transform.position = roadTilemap.GetCellCenterWorld(cellPosition);
    }

    private bool IsCenteredOnTile()
    {
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = roadTilemap.WorldToCell(worldPosition);
        Vector3 tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);
        return Vector2.Distance(worldPosition, tileCenter) <= snapTolerance;
    }

    private bool CanMoveInDirection(Vector2 direction)
    {
        Vector3Int targetPosition = roadTilemap.WorldToCell(transform.position + (Vector3)direction);
        return roadTilemap.HasTile(targetPosition);
    }

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
            _targetDirection = availableDirections[Random.Range(0, availableDirections.Count)];
        }

        _lastDirection = _targetDirection;
    }

    private Vector2 GetChaseDirection(List<Vector2> directions)
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                return GetBestDirectionToPoint(pacman.position, directions);
            case GhostType.Pinky:
                return GetBestDirectionToPoint((Vector2)pacman.position + (Vector2)pacman.up * 4, directions);
            case GhostType.Inky:
                return Random.value > 0.5f ? GetBestDirectionToPoint(pacman.position, directions) : GetFarthestDirectionFromPacman(directions);
            case GhostType.Clyde:
                return Vector2.Distance(transform.position, pacman.position) > 8f ? GetBestDirectionToPoint(pacman.position, directions) : GetBestDirectionToPoint(ghostHouseExit.position, directions);
            default:
                return directions[Random.Range(0, directions.Count)];
        }
    }

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
