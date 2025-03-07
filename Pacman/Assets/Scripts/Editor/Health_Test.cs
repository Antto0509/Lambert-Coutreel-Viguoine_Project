using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class Health_Test
    {
        private HealthManager manager;

        [SetUp]
        public void Setup()
        {
            // Créer un objet HealthManager
            manager = new GameObject("HealthManager").AddComponent<HealthManager>();

            // Configurer le HealthManager
            manager.maxHearts = 20; // Définir la santé maximale
            manager.currentHealth = 3; // Définir la santé initiale

            // Simuler heartObjects
            manager.heartObjects = new GameObject[manager.maxHearts];
            for (int i = 0; i < manager.maxHearts; i++)
            {
                manager.heartObjects[i] = new GameObject("Heart");
            }
        }

        [Test]
        public void Add_Health_Test()
        {
            // Act
            manager.AddHealth(10);

            // Assert
            Assert.AreEqual(13, manager.currentHealth); // Vérifier que la santé a augmenté

            // Act
            manager.DecreaseHealth(2);

            // Assert
            Assert.AreEqual(11, manager.currentHealth); // Vérifier que la santé a diminué
        }

        [Test]
        public void Add_Health_DoesNotExceedMaxHearts()
        {
            // Act
            manager.AddHealth(100); // Essayer d'ajouter plus de santé que maxHearts

            // Assert
            Assert.AreEqual(manager.maxHearts, manager.currentHealth); // La santé ne doit pas dépasser maxHearts
        }

        [Test]
        public void Decrease_Health_DoesNotGoBelowZero()
        {
            // Act
            manager.DecreaseHealth(100); // Essayer de retirer plus de santé que disponible

            // Assert
            Assert.AreEqual(0, manager.currentHealth); // La santé ne doit pas descendre en dessous de 0
        }

        [TearDown]
        public void TearDown()
        {
            // Nettoyer après le test
            Object.DestroyImmediate(manager.gameObject);
        }
    }
}