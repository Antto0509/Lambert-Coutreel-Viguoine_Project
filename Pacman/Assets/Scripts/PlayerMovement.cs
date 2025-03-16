using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    public float snapTolerance = 0.1f;
    
    public Rigidbody2D rb;
    
    public Tilemap roadTilemap;
    
    public HealthManager healthManager;
    
    public ScoreManager scoreManager;
    
    public Transform RespawnPoint;
    
    public Animator animator;
    
    public new CircleCollider2D collider;
    
    private Vector2 movement;
    
    private Vector2 target;
    
    private Vector3Int currentCell;

    public GameObject Sortie1;
    
    public GameObject Sortie2;
    
    public GameObject SortieFin;
    
    public bool hunter = false;

    public Vector3Int targetPosition;

    private void Start()
    {
        AlignToTileCenter();
    }

    private void Update()
    {
        SetDirection();
    }

    private void AlignToTileCenter()
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(transform.position);

        Vector3 tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);

        transform.position = tileCenter;
    }
    
    private bool CheckIfCellInDirection(Vector2 direction)
    {
        if (!IsCenteredOnTile())
        {
            return false;
        }
        
        // Calculer la position cible
        targetPosition = roadTilemap.WorldToCell(transform.position + (Vector3)direction);

        // Vérifie si la case correspondante dans la Tilemap a une tuile
        return roadTilemap.HasTile(targetPosition);
    }
    
    private bool IsCenteredOnTile()
    {
        Vector3 worldPosition = transform.position;

        Vector3Int cellPosition = roadTilemap.WorldToCell(worldPosition);

        Vector3 tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);

        return Vector2.Distance(worldPosition, tileCenter) <= snapTolerance;
    }

    private void SetDirection()
    {
        
        if (Input.GetKey(KeyCode.UpArrow) && CheckIfCellInDirection(Vector2.up))
        {
            target = Vector2.up;
            SeeForward();
        }
        else if (Input.GetKey(KeyCode.DownArrow) && CheckIfCellInDirection(Vector2.down))
        {
            target = Vector2.down;
            SeeForward();
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && CheckIfCellInDirection(Vector2.left))
        {
            target = Vector2.left;
            SeeForward();
        }
        else if (Input.GetKey(KeyCode.RightArrow) && CheckIfCellInDirection(Vector2.right))
        {
            target = Vector2.right;
            SeeForward();
        }
        
        rb.linearVelocity = target * moveSpeed;
    }

    private void SeeForward()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Vérifier la direction principale
        if (horizontal > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);     
        }
        else if (horizontal < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);        
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PacGomme"))
        {
            scoreManager.AddScore(10);
            Destroy(other.gameObject);
        }
        
        if (other.CompareTag("SuperPacGomme"))
        {
            scoreManager.AddScore(50);
            Destroy(other.gameObject);
            StartCoroutine(HuntingPhase());
        }
        
        if (other.CompareTag("Ghost"))
        {
            if (!hunter)
            {
                StartCoroutine(Death());
            }
            else
            {
                other.gameObject.GetComponent<GhostMovement>().dead = true;
            }
        }

        if (other.CompareTag("Exit"))
        {
            if (SortieFin != other.gameObject && other.transform.position == Sortie1.transform.position)
            {
                transform.position = Sortie2.transform.position;
                SortieFin = Sortie2;
            }
            else
            {
                if (SortieFin != other.gameObject && other.transform.position == Sortie2.transform.position)
                {
                    transform.position = Sortie1.transform.position;
                    SortieFin = Sortie1;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Exit") && SortieFin == other.gameObject)
        {
            SortieFin = null;
        }
    }

    private IEnumerator HuntingPhase()
    {
        hunter = true;
        moveSpeed = 6.0f;
        
        yield return new WaitForSeconds(10f);
        
        hunter = false;
        moveSpeed = 5.0f;
    }
    
    private IEnumerator Death()
    {
        target = Vector2.zero;
        collider.isTrigger = true;
        
        yield return PlayAnimationAndWait("Death");
        
        transform.position = RespawnPoint.position;
        collider.isTrigger = false;
        AlignToTileCenter();
        healthManager.DecreaseHealth(1);
        animator.Play("PacmanMove");
    }
    
    private IEnumerator PlayAnimationAndWait(string animationName)
    {
        // Lancer l'animation
        animator.Play(animationName);
        
        // Attendre que l'animation commence (utile pour les transitions)
        yield return new WaitForEndOfFrame();

        // Attendre que l'animation se termine
        while (IsAnimationPlaying(animationName))
        {
            yield return null; // Attendre le prochain frame
        }
    }
    
    private bool IsAnimationPlaying(string animationName)
    {
        var currentState = animator.GetCurrentAnimatorStateInfo(0);
        return currentState.IsName(animationName) && currentState.normalizedTime < 1.0f;
    }

    public void Restart()
    {
        transform.position = RespawnPoint.transform.position;
        AlignToTileCenter();
        target = Vector2.zero;
    }
}
