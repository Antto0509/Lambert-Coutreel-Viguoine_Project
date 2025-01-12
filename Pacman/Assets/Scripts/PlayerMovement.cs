using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Tilemap roadTilemap;
    
    private Vector2 movement;
    private Vector2? target;
    private Vector3Int currentCell;

    private void Start()
    {
        target = null;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // movement = new Vector2(horizontal, vertical).normalized;
        
        SeeForward(horizontal, vertical);
        SetDirection();
        
        rb.linearVelocity = movement * moveSpeed;

        if (movement == Vector2.zero)
        {
            AlignToTile();
        }
    }

    // Méthode pour aligner le personnage au centre de la tuile
    private void AlignToTile()
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(transform.position);

        Vector3 cellCenterPos = roadTilemap.GetCellCenterWorld(cellPosition);

        rb.position = new Vector2(cellCenterPos.x, cellCenterPos.y);
    }

    private void CheckIfCellInDirection()
    {
        
    }

    private void SetDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Haut
        {
            target = (Vector2)transform.position + Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // Bas
        {
            target = (Vector2)transform.position + Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) // Gauche
        {
            target = (Vector2)transform.position + Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // Droite
        {
            target = (Vector2)transform.position + Vector2.right;
        }

    }

    private void SeeForward(float horizontal, float vertical)
    {
        // Vérifier la direction principale
        if (horizontal > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);     
        }
        else if (horizontal < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 180);        
        }
        else if (vertical > 0)
        {   
            transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (vertical < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, -90);        
        }
    }
}
