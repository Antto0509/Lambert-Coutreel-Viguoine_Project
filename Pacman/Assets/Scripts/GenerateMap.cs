using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public float scale = 20f; 
    public int width = 10;
    public int height = 10;
    private int[,] _mapLayout;

    public GameObject MurG;
    public GameObject MurD;
    public GameObject MurH;
    public GameObject MurB;
    public GameObject CoinHG;
    public GameObject CoinBG;
    public GameObject CoinBD;
    public GameObject CoinHD;
    public GameObject pacGumPrefab;
    public GameObject playerPrefab;
    public GameObject GhostBlue;
    public GameObject GhostPink;
    public GameObject GhostOrange;
    public GameObject GhostRed;

    void Start()
    {
        GenerateMap();
        DrawMap();
    }
    
    void GenerateMap()
    {
        _mapLayout = new int[height, width]; // Initialiser le tableau

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Si on est sur un bord, on met un mur (1)
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    _mapLayout[y, x] = 1; // Mur
                }
                else
                {
                    // Génération procédurale avec Perlin Noise
                    float xCoord = (float)x / width * scale;
                    float yCoord = (float)y / height * scale;
                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);

                    // Définir les valeurs (exemple : 1 = mur, 2 = pacgum, 0 = vide)
                    if (noiseValue > 0.6f) _mapLayout[y, x] = 1; // Mur
                    else if (noiseValue > 0.3f) _mapLayout[y, x] = 2; // Pacgum
                    else _mapLayout[y, x] = 0; // Sol vide
                }
            }
        }
    }

    void DrawMap() {
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            Vector3 position = new Vector3(x, 0, y);

            if (_mapLayout[y, x] == 1) // Si c'est un mur
            {
                bool isLeftWall = (x == 0 && y > 0 && y < height - 1);
                bool isRightWall = (x == width - 1 && y > 0 && y < height - 1);
                bool isTopWall = (y == height - 1 && x > 0 && x < width - 1);
                bool isBottomWall = (y == 0 && x > 0 && x < width - 1);

                bool isTopLeftCorner = (x == 0 && y == height - 1);
                bool isTopRightCorner = (x == width - 1 && y == height - 1);
                bool isBottomLeftCorner = (x == 0 && y == 0);
                bool isBottomRightCorner = (x == width - 1 && y == 0);

                if (isTopLeftCorner)
                {
                    Instantiate(CoinHG, position, Quaternion.identity, transform);
                }
                else if (isTopRightCorner)
                {
                    Instantiate(CoinHD, position, Quaternion.identity, transform);
                }
                else if (isBottomLeftCorner)
                {
                    Instantiate(CoinBG, position, Quaternion.identity, transform);
                }
                else if (isBottomRightCorner)
                {
                    Instantiate(CoinBD, position, Quaternion.identity, transform);
                }
                else if (isLeftWall)
                {
                    Instantiate(MurG, position, Quaternion.identity, transform);
                }
                else if (isRightWall)
                {
                    Instantiate(MurD, position, Quaternion.identity, transform);
                }
                else if (isTopWall)
                {
                    Instantiate(MurH, position, Quaternion.identity, transform);
                }
                else if (isBottomWall)
                {
                    Instantiate(MurB, position, Quaternion.identity, transform);
                }else if (_mapLayout[y, x] == 4) // Pac-Man
                {
                    Instantiate(playerPrefab, position, Quaternion.identity, transform);
                }
                else if (_mapLayout[y, x] == 5) // Fantôme bleu
                {
                    Instantiate(GhostBlue, position, Quaternion.identity, transform);
                }
                else if (_mapLayout[y, x] == 6) // Fantôme rose
                {
                    Instantiate(GhostPink, position, Quaternion.identity, transform);
                }
                else if (_mapLayout[y, x] == 7) // Fantôme orange
                {
                    Instantiate(GhostOrange, position, Quaternion.identity, transform);
                }
                else if (_mapLayout[y, x] == 8) // Fantôme rouge
                {
                    Instantiate(GhostRed, position, Quaternion.identity, transform);
                }else if (_mapLayout[y, x] == 2) // Pac-Gomme
                {
                    Instantiate(pacGumPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
    
}