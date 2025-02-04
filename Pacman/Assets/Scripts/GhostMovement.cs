using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float snapTolerance = 0.1f;
    public Tilemap roadTilemap;
    public Rigidbody2D rb;
    public CircleCollider2D collider;
    public float radiusInSpawn = 0.2f;
    public float radiusOutSpawn = 0.12f;
    
    private Vector2 targetDirection;
    private Vector2 lastDirection;
    private Vector2 secondLastDirection;
    private float changeDirectionCooldown = 1.5f;
    private float lastChangeTime;
    
    private void Start()
    {
        AlignToTileCenter();
        ChooseInitialDirection();
    }

    private void Update()
    {
        if (IsCenteredOnTile())
        {
            if (IsObstacleAhead())
            {
                ChooseNewDirection(true); // Collision forcée
            }
            else if (Time.time - lastChangeTime > changeDirectionCooldown && UnityEngine.Random.value < 0.4f)
            {
                ChooseNewDirection(false); // Changement normal
            }
        }
        rb.linearVelocity = targetDirection * moveSpeed;
    }

    private void AlignToTileCenter()
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(transform.position);
        Vector3 tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);
        transform.position = tileCenter;
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
        if (!IsCenteredOnTile()) return false;
        Vector3Int targetPosition = roadTilemap.WorldToCell(transform.position + (Vector3)direction);
        return roadTilemap.HasTile(targetPosition);
    }

    private bool IsObstacleAhead()
    {
        return !CanMoveInDirection(targetDirection);
    }

    private void ChooseInitialDirection()
    {
        List<Vector2> possibleDirections = new List<Vector2> { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        possibleDirections.RemoveAll(dir => !CanMoveInDirection(dir));
        if (possibleDirections.Count > 0)
        {
            targetDirection = possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Count)];
        }
        lastDirection = targetDirection;
        secondLastDirection = lastDirection;
    }

    private void ChooseNewDirection(bool forced)
    {
        List<Vector2> possibleDirections = new List<Vector2>();
        List<Vector2> alternativeDirections = new List<Vector2>();
        Vector2 reverseDirection = -targetDirection;

        Vector2[] allDirections = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        foreach (Vector2 dir in allDirections)
        {
            if (CanMoveInDirection(dir))
            {
                if (dir != lastDirection && dir != secondLastDirection && dir != reverseDirection)
                    possibleDirections.Add(dir);
                else
                    alternativeDirections.Add(dir);
            }
        }

        if (forced && CanMoveInDirection(reverseDirection))
        {
            targetDirection = reverseDirection; // Demi-tour priorisé si collision
        }
        else if (possibleDirections.Count > 0)
        {
            secondLastDirection = lastDirection;
            lastDirection = targetDirection;
            targetDirection = possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Count)];
        }
        else if (alternativeDirections.Count > 0)
        {
            secondLastDirection = lastDirection;
            lastDirection = targetDirection;
            targetDirection = alternativeDirections[UnityEngine.Random.Range(0, alternativeDirections.Count)];
        }
        lastChangeTime = Time.time;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SpawnAreaGhost"))
        {
            collider.radius = radiusInSpawn;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SpawnAreaGhost"))
        {
            collider.radius = radiusOutSpawn;
        }
    }
}
