using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationItemManager : MonoBehaviour
{
    public Tilemap roadTilemap;
    
    public GameObject pacGommePrefab;

    public GameObject[] pacGommeLst;
    
    void Start()
    {
        SetPacGommeOnAllRoadCell();
    }

    void Update()
    {
        CheckPacGommeCount();
    }

    public void CheckPacGommeCount()
    {
        pacGommeLst = GameObject.FindGameObjectsWithTag("PacGomme");
        if (pacGommeLst.Length == 0)
        {
            SetPacGommeOnAllRoadCell();
        }
    }

    public void SetPacGommeOnAllRoadCell()
    {
        foreach (Vector3Int position in roadTilemap.cellBounds.allPositionsWithin)
        {
            if (roadTilemap.HasTile(position))
            {
                Vector3 worldPosition = roadTilemap.GetCellCenterWorld(position);

                worldPosition.z = 0;

                Instantiate(pacGommePrefab, worldPosition, Quaternion.identity);
            }
        }
    }
}
