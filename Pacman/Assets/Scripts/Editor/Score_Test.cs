using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class Score_Test
    {
        private ScoreManager _scoreManager;
        
        /// <summary>
        /// Initialise le ScoreManager et le texte affichant le score avant chaque test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Crée un GameObject pour le ScoreManager
            _scoreManager = new GameObject("ScoreManager").AddComponent<ScoreManager>();

            // Crée un GameObject pour le TextMeshPro et ajoute le composant TextMeshPro
            GameObject textObject = new GameObject("ScoreText");
            _scoreManager.scoreText = textObject.AddComponent<TextMeshPro>();
        }

        /// <summary>
        /// Vérifie que l'ajout d'un score fonctionne correctement.
        /// </summary>
        [Test]
        public void Add_Score()
        {
            _scoreManager.AddScore(10);
            
            Assert.AreEqual("10", _scoreManager.scoreText.text);
            Assert.AreEqual(10, _scoreManager.score);
        }

        /// <summary>
        /// Vérifie que plusieurs ajouts de score s'additionnent correctement.
        /// </summary>
        [Test]
        public void Add_Score_2()
        {
            _scoreManager.AddScore(5);
            _scoreManager.AddScore(20);
            
            Assert.AreEqual("25", _scoreManager.scoreText.text);
            Assert.AreEqual(25, _scoreManager.score);
        }

        /// <summary>
        /// Vérifie que plusieurs ajouts de score s'additionnent correctement (soustraction).
        /// </summary>
        [Test]
        public void Add_Score_Negative()
        {
            _scoreManager.AddScore(25);
            
            _scoreManager.AddScore(-10);
            
            Assert.GreaterOrEqual(_scoreManager.score, 15);
        }

        /// <summary>
        /// Vérifie que la réinitialisation du score fonctionne.
        /// </summary>
        [Test]
        public void Reset_Score()
        {
            _scoreManager.AddScore(30);
            _scoreManager.ResetScore();
            
            Assert.AreEqual("0", _scoreManager.scoreText.text);
            Assert.AreEqual(0, _scoreManager.score);
        }

        /// <summary>
        /// Nettoie les objets après chaque test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_scoreManager.gameObject);
            Object.DestroyImmediate(_scoreManager.scoreText.gameObject);
        }
    }
}
