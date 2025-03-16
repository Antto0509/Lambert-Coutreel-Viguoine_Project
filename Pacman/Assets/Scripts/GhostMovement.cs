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
    public float snapTolerance = 0.001f;
    public Tilemap roadTilemap;
    public Rigidbody2D rb;
    public Transform respawnPointGhost;
    public bool isInHouse = true;
    public Vector2 _targetDirection;
    public Vector2 _lastDirection;
    
    public GameObject seeUp;
    public GameObject seeDown;
    public GameObject seeLeft;
    public GameObject seeRight;
    
    public GameObject beChasingSeeUp;
    public GameObject beChasingSeeDown;
    public GameObject beChasingSeeLeft;
    public GameObject beChasingSeeRight;
    
    public GameObject pointSpawn;
    public GameObject pointSortie;
    
    public PlayerMovement playerMovement;

    public int sortieStatus = 0;

    public GameObject Sortie1;
    
    public GameObject Sortie2;

    public GameObject SortieFin;

    public CircleCollider2D circleCollider;
    
    public bool wantToExit = false;
    public bool wait = false;
    public bool dead = false;
    
    /// <summary>
    /// Initialise le fantôme.
    /// </summary>
    private void Start()
    {
        seeUp.SetActive(false);
        seeDown.SetActive(true);
        seeLeft.SetActive(false);
        seeRight.SetActive(false);
        beChasingSeeUp.SetActive(false);
        beChasingSeeDown.SetActive(false);
        beChasingSeeLeft.SetActive(false);
        beChasingSeeRight.SetActive(false);
        
        AlignToTileCenter();
        ChooseNewDirection();

        if (ghostType == GhostType.Blinky)
        {
            isInHouse = false;
        }
        else
        {
            StartCoroutine(ReleaseFromHouse());
        }
    }

    /// <summary>
    /// Met à jour la position du fantôme.
    /// </summary>
    private void Update()
    {
        if (isInHouse && !wantToExit)
        {
            MoveInsideHouse();
            return;
        }

        if (dead)
        {
            Death();
        }
        
        // Si le fantôme est centré sur une tuile, il choisit une nouvelle direction
        if (IsCenteredOnTile() && !wait && !dead && !isInHouse)
        {
            ChooseNewDirection();
            StartCoroutine(WaitOneSecond()); // on attend 0.1s pour eviter que le fantomme change plusieur fois de choix sur le meme croisement
        }
        rb.linearVelocity = _targetDirection * moveSpeed;
        UpdateSprite();
    }

    private void Death()
    {
        seeUp.SetActive(false);
        seeDown.SetActive(true);
        seeLeft.SetActive(false);
        seeRight.SetActive(false);
        beChasingSeeUp.SetActive(false);
        beChasingSeeDown.SetActive(false);
        beChasingSeeLeft.SetActive(false);
        beChasingSeeRight.SetActive(false);
            
        circleCollider.enabled = false;
        transform.position = respawnPointGhost.position;
        StartCoroutine(ReleaseFromHouse());
        _targetDirection = Vector2.zero;
        _lastDirection = Vector2.zero;
        isInHouse = true;
        dead = false;
        circleCollider.enabled = true;
    }


    /// <summary>
    /// Methode qui tourne en paralelle du code, ici elle attend 0.1s pour eviter que le fantomme change de choix plusieur fois au meme croisement
    /// </summary>
    /// <returns>Coroutine. </returns>
    private IEnumerator WaitOneSecond()
    {
        wait = true;
        yield return new WaitForSecondsRealtime(0.1f);
        wait = false;
    }

    /// <summary>
    /// Libère le fantôme de la maison après un certain délai.
    /// </summary>
    /// <returns> Coroutine. </returns>
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
        wantToExit = true;
        
        circleCollider.isTrigger = true;
        
        while (isInHouse)
        {
            yield return new WaitForSeconds(1);
            
            switch (sortieStatus)
            {
                case 0:
                    _targetDirection = pointSpawn.transform.position - transform.position;
                    moveSpeed = 0.5f;
                    if (Vector2.Distance(transform.position, pointSpawn.transform.position) <= snapTolerance)
                    {
                        sortieStatus += 1;
                    }
                    break;
                case 1:
                    _targetDirection = pointSortie.transform.position - transform.position;
                    moveSpeed = 0.5f;
                    if (Vector2.Distance(transform.position, pointSortie.transform.position) <= snapTolerance)
                    {
                        sortieStatus += 1;
                    }
                    break;
                default:
                    moveSpeed = 5f;
                    isInHouse = false;
                    ChooseNewDirection();
                    break;
            }
        }
    }

    /// <summary>
    /// Déplace le fantôme à l'intérieur de la maison.
    /// </summary>
    private void MoveInsideHouse()
    {
        wantToExit = false;
        circleCollider.isTrigger = false;
        const float oscillationSpeed = 1f;
        transform.position += Vector3.up * (Mathf.Sin(Time.time * oscillationSpeed) * Time.deltaTime);
    }

    /// <summary>
    /// Centre le fantôme sur une tuile.
    /// </summary>
    private void AlignToTileCenter()
    {
        var cellPosition = roadTilemap.WorldToCell(transform.position);
        transform.position = roadTilemap.GetCellCenterWorld(cellPosition);
    }

    /// <summary>
    /// Vérifie si le fantôme est centré sur une tuile.
    /// </summary>
    /// <returns> Vrai si le fantôme est centré sur une tuile, faux sinon. </returns>
    private bool IsCenteredOnTile()
    {
        var worldPosition = transform.position;
        var cellPosition = roadTilemap.WorldToCell(worldPosition);
        var tileCenter = roadTilemap.GetCellCenterWorld(cellPosition);
        return Vector2.Distance(worldPosition, tileCenter) <= snapTolerance;
    }

    /// <summary>
    /// Vérifie si le fantôme peut se déplacer dans une direction donnée.
    /// </summary>
    /// <param name="direction"> Direction à vérifier. </param>
    /// <returns> Vrai si le fantôme peut se déplacer dans la direction donnée, faux sinon. </returns>
    private bool CanChangeDirection(Vector2 direction)
    {
        var targetPosition = roadTilemap.WorldToCell(transform.position + (Vector3)direction);
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
            if (CanChangeDirection(dir))
            {
                if (dir == -_targetDirection)
                {
                    continue;
                }
                availableDirections.Add(dir);
            }
        }

        if (availableDirections.Count > 2)
        {
            availableDirections = MajAvalableDirections(availableDirections);
            _targetDirection = availableDirections[Random.Range(0, availableDirections.Count)];
        }
        else
        {
            _targetDirection = availableDirections[Random.Range(0, availableDirections.Count)];
        }

        _lastDirection = _targetDirection;
    }

    
    /// <summary>
    /// Met a jour ma liste des direction disponible pour que le fantome ne retourne pas en arière ou qu'il reste dans la meme direction, on veux qu'il change a chaque croisement pour utiliser toute
    /// la place qu'il y a sur le plateau
    /// </summary>
    /// <param name="directions">Liste de direction possible</param>
    /// <returns>directions</returns>
    public List<Vector2> MajAvalableDirections(List<Vector2> directions)
    {
        if (_lastDirection == Vector2.up || _lastDirection == Vector2.down)
        {
            directions.Remove(Vector2.up);
            directions.Remove(Vector2.down);
        } 
        else
        {
            directions.Remove(Vector2.right);
            directions.Remove(Vector2.left);
        }
        
        return directions;
    }

    /// <summary>
    /// Met à jour le sprite du fantôme en fonction de sa direction.
    /// </summary>
    private void UpdateSprite()
    {
        if (_targetDirection == Vector2.right)
        {
            if (playerMovement.hunter)
            {
                seeUp.SetActive(false);
                seeDown.SetActive(false);
                seeLeft.SetActive(false);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(true);
            }
            else
            {
                seeUp.SetActive(false);
                seeDown.SetActive(false);
                seeLeft.SetActive(false);
                seeRight.SetActive(true);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(false);
            }
        }
        else if (_targetDirection == Vector2.left)
        {
            if (playerMovement.hunter)
            {
                seeUp.SetActive(false);
                seeDown.SetActive(false);
                seeLeft.SetActive(false);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(true);
                beChasingSeeRight.SetActive(false);
            }
            else
            {
                seeUp.SetActive(false);
                seeDown.SetActive(false);
                seeLeft.SetActive(true);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(false);
            }
        }
        else if (_targetDirection == Vector2.up)
        {
            if (playerMovement.hunter)
            {
                seeUp.SetActive(false);
                seeDown.SetActive(false);
                seeLeft.SetActive(false);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(true);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(false);
            }
            else
            {
                seeUp.SetActive(true);
                seeDown.SetActive(false);
                seeLeft.SetActive(false);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(false);
            }
        }
        else if (_targetDirection == Vector2.down)
        {
            if (playerMovement.hunter)
            {
                seeUp.SetActive(false);
                seeDown.SetActive(false);
                seeLeft.SetActive(false);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(true);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(false);
            }
            else
            {
                seeUp.SetActive(false);
                seeDown.SetActive(true);
                seeLeft.SetActive(false);
                seeRight.SetActive(false);
                beChasingSeeUp.SetActive(false);
                beChasingSeeDown.SetActive(false);
                beChasingSeeLeft.SetActive(false);
                beChasingSeeRight.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Methode propre a unity qui detecte l'entrée dans un trigger
    /// </summary>
    /// <param name="other">Le Collider du triger dans le quelle on entre</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            if (SortieFin != other.gameObject && other.transform.position == Sortie1.transform.position)
            {
                transform.position = Sortie2.transform.position;
                SortieFin = Sortie2;
                AlignToTileCenter();
            }
            else
            {
                if (SortieFin != other.gameObject && other.transform.position == Sortie2.transform.position)
                {
                    transform.position = Sortie1.transform.position;
                    SortieFin = Sortie1;
                    AlignToTileCenter();
                }
            }
        }
    }
    
    /// <summary>
    /// Methode propre a unity qui detecte la sortie d'un trigger
    /// </summary>
    /// <param name="other">Le Collider du triger du quelle on est sorti</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Exit") && SortieFin == other.gameObject)
        {
            SortieFin = null;
        }
    }
}
