using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class Health_Test
    {
        private HealthManager _healthManager;

        /// <summary>
        /// Initialise l'objet HealthManager avant chaque test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Crée un objet HealthManager
            _healthManager = new GameObject("HealthManager").AddComponent<HealthManager>();

            // Initialise les objets représentant les cœurs
            _healthManager.heartObjects = new GameObject[_healthManager.maxHearts];
            for (int i = 0; i < _healthManager.maxHearts; i++)
            {
                _healthManager.heartObjects[i] = new GameObject("Heart");
            }
        }

        /// <summary>
        /// Vérifie que l'ajout de santé fonctionne correctement.
        /// </summary>
        [Test]
        public void Add_Health_Test()
        {
            // Act : Ajouter de la santé
            _healthManager.AddHealth(10);

            // Assert : Vérifier que la santé a bien augmenté
            Assert.AreEqual(13, _healthManager.currentHealth);
        }

        /// <summary>
        /// Vérifie que la diminution de la santé fonctionne correctement.
        /// </summary>
        [Test]
        public void Remove_Health_Test()
        {
            // Act : Retirer de la santé
            _healthManager.DecreaseHealth(2);

            // Assert : Vérifier que la santé a bien diminué
            Assert.AreEqual(1, _healthManager.currentHealth);
        }

        /// <summary>
        /// Vérifie que la santé ne dépasse pas le nombre maximal de cœurs.
        /// </summary>
        [Test]
        public void Add_Health_DoesNotExceedMaxHearts()
        {
            // Act : Ajouter plus de santé que le maximum autorisé
            _healthManager.AddHealth(100);

            // Assert : Vérifier que la santé reste au maximum
            Assert.AreEqual(_healthManager.maxHearts, _healthManager.currentHealth);
        }

        /// <summary>
        /// Nettoie les objets créés après chaque test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Détruit l'objet HealthManager après le test
            Object.DestroyImmediate(_healthManager.gameObject);
        }
    }
}
