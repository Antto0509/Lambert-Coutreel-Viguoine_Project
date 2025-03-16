using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Editor
{
    public class GenerationItem_Test
    {
        private GenerationItemManager _generationItemManager;
        private GameObject _gridInstance;

        /// <summary>
        /// Initialise les objets nécessaires avant chaque test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Crée un GameObject pour le GenerationItemManager
            _generationItemManager = new GameObject("GenerationItemManager").AddComponent<GenerationItemManager>();

            // Charge la préfab Grid depuis le dossier Resources/map
            GameObject gridPrefab = Resources.Load<GameObject>("Prefabs/map/Grid");
            Assert.IsNotNull(gridPrefab, "La préfab Grid n'a pas été trouvée dans Resources/map");

            // Instancie la préfab Grid
            _gridInstance = Object.Instantiate(gridPrefab);

            // Récupère toutes les Tilemaps dans la préfab Grid
            Tilemap[] tilemaps = _gridInstance.GetComponentsInChildren<Tilemap>();

            // Cherche la Tilemap nommée "Road"
            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap.gameObject.name == "Road")
                {
                    _generationItemManager.roadTilemap = tilemap;
                    break;
                }
            }

            // Vérifie que la Tilemap "Road" a bien été trouvée
            Assert.IsNotNull(_generationItemManager.roadTilemap, "La Tilemap 'Road' n'a pas été trouvée dans la préfab Grid");

            // Charge les autres préfabs nécessaires
            _generationItemManager.pacGommePrefab = Resources.Load<GameObject>("Prefabs/PacGomme/PacGomme");
            _generationItemManager.SuperPacGommePrefab = Resources.Load<GameObject>("Prefabs/PacGomme/SuperPacGome");

            // Vérifie que les préfabs ont bien été chargées
            Assert.IsNotNull(_generationItemManager.pacGommePrefab, "pacGommePrefab n'a pas été chargée");
            Assert.IsNotNull(_generationItemManager.SuperPacGommePrefab, "SuperPacGommePrefab n'a pas été chargée");
        }

        /// <summary>
        /// Vérifie que le nombre de Super Pac-Gommes placées est correct.
        /// </summary>
        [Test]
        public void GenerationItemManagerTest()
        {
            // Génère les Pac-Gommes sur toutes les cellules de la route
            _generationItemManager.SetPacGommeOnAllRoadCell(true);

            // Vérifie que le nombre de Super Pac-Gommes est bien égal à 5
            Assert.AreEqual(5, _generationItemManager.SuperPacGommeCount);
        }

        /// <summary>
        /// Vérifie que le nombre de Pac-Gommes normales est correct par rapport aux cellules de la route.
        /// </summary>
        [Test]
        public void TestCountPacGomme()
        {
            _generationItemManager.SetPacGommeOnAllRoadCell(true);

            var countTile = 0;

            // Parcourt toutes les positions dans les limites de la Tilemap
            foreach (Vector3Int position in _generationItemManager.roadTilemap.cellBounds.allPositionsWithin)
            {
                // Vérifie si une tuile est présente à cette position
                if (_generationItemManager.roadTilemap.HasTile(position))
                {
                    countTile++;
                }
            }

            // Vérifie que le nombre de Pac-Gommes normales correspond au nombre total de tuiles - les Super Pac-Gommes
            Assert.AreEqual(countTile - _generationItemManager.SuperPacGommeCount, _generationItemManager.PacGommeCount);
        }

        /// <summary>
        /// Nettoie les objets créés après chaque test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Détruit les objets créés pendant le test
            if (_generationItemManager != null)
            {
                Object.DestroyImmediate(_generationItemManager.gameObject);
            }
            if (_gridInstance != null)
            {
                Object.DestroyImmediate(_gridInstance);
            }
        }
    }
}
