using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class Health_Test
    {
        private HealthManager _healthManager;

        [SetUp]
        public void Setup()
        {
            // Créer un objet HealthManager
            _healthManager = new GameObject("HealthManager").AddComponent<HealthManager>();

            // Simuler heartObjects
            _healthManager.heartObjects = new GameObject[_healthManager.maxHearts];
            for (int i = 0; i < _healthManager.maxHearts; i++)
            {
                _healthManager.heartObjects[i] = new GameObject("Heart");
            }
        }

        [Test]
        public void Add_Health_Test()
        {
            // Act
            _healthManager.AddHealth(10);

            // Assert
            Assert.AreEqual(13, _healthManager.currentHealth); // Vérifier que la santé a augmenté
        }

        [Test]
        public void Remove_Health_Test()
        {
            // Act
            _healthManager.DecreaseHealth(2);

            // Assert
            Assert.AreEqual(1, _healthManager.currentHealth); // Vérifier que la santé a diminué
        }

        [Test]
        public void Add_Health_DoesNotExceedMaxHearts()
        {
            // Act
            _healthManager.AddHealth(100); // Essayer d'ajouter plus de santé que maxHearts

            // Assert
            Assert.AreEqual(_healthManager.maxHearts, _healthManager.currentHealth); // La santé ne doit pas dépasser maxHearts
        }

        [Test]
        public void Decrease_Health_DoesNotGoBelowZero()
        {
            // Act
            _healthManager.DecreaseHealth(100); // Essayer de retirer plus de santé que disponible

            // Assert
            Assert.AreEqual(0, _healthManager.currentHealth); // La santé ne doit pas descendre en dessous de 0
        }

        [TearDown]
        public void TearDown()
        {
            // Nettoyer après le test
            Object.DestroyImmediate(_healthManager.gameObject);
        }
    }
}