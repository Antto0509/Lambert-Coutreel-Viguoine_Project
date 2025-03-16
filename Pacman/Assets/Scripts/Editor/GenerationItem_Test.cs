using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Editor
{
    public class GenerationItem_Test
    {
        private GenerationItemManager _generationItemManager;
        private GameObject _gridInstance;

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

            // Charge les autres préfabs
            _generationItemManager.pacGommePrefab = Resources.Load<GameObject>("Prefabs/PacGomme/PacGomme");
            _generationItemManager.SuperPacGommePrefab = Resources.Load<GameObject>("Prefabs/PacGomme/SuperPacGome");

            // Vérifie que les préfabs ont bien été chargées
            Assert.IsNotNull(_generationItemManager.pacGommePrefab, "pacGommePrefab n'a pas été chargée");
            Assert.IsNotNull(_generationItemManager.SuperPacGommePrefab, "SuperPacGommePrefab n'a pas été chargée");
        }

        [Test]
        public void GenerationItemManagerTest()
        {
            _generationItemManager.SetPacGommeOnAllRoadCell(true);

            Assert.AreEqual(5, _generationItemManager.SuperPacGommeCount);
        }

        [TearDown]
        public void TearDown()
        {
            // Nettoie les objets créés pendant le test avec DestroyImmediate
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