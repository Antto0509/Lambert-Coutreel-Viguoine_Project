using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public int width = 40;
    public int height = 40;
    public int iterations = 5;
    public TileBase wallTile;
    public TileBase pathTile;
    public TileBase pointTile;
    public Tilemap tilemap;
    
    private int[,] grid;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        grid = new int[width, height];
        DrawBaseShape(); // Dessine la forme de base
        RandomFillGrid(); // Remplis le reste de la grille aléatoirement

        for (int i = 0; i < iterations; i++)
        {
            SmoothGrid(); // Applique l'automate cellulaire
        }

        RenderMap(); // Affiche la carte sur la Tilemap
    }

    void DrawBaseShape()
    {
        int baseWidth = 10;
        int baseHeight = 6;
        int startX = (width - baseWidth) / 2;
        int startY = (height - baseHeight) / 2;

        for (int x = 0; x < baseWidth; x++)
        {
            for (int y = 0; y < baseHeight; y++)
            {
                grid[startX + x, startY + y] = 1; // Dessine un rectangle
            }
        }
    }

    void RandomFillGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0) // Ne modifie pas la forme de base
                {
                    grid[x, y] = Random.Range(0, 2);
                }
            }
        }
    }

    void SmoothGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0) // Ne modifie pas la forme de base
                {
                    int neighborWallCount = GetSurroundingWallCount(x, y);

                    if (neighborWallCount > 4)
                        grid[x, y] = 0;
                    else if (neighborWallCount < 4)
                        grid[x, y] = 1;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += grid[neighborX, neighborY] == 0 ? 1 : 0;
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void RenderMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (grid[x, y] == 0)
                {
                    tilemap.SetTile(tilePosition, wallTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, pathTile);
                }
            }
        }
    }
}