using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Editor
{
    public class Score_Test
    {
        private ScoreManager _scoreManager;
        
        [SetUp]
        public void Setup()
        {
            // Crée un GameObject pour le ScoreManager
            _scoreManager = new GameObject("ScoreManager").AddComponent<ScoreManager>();

            // Crée un GameObject pour le TextMeshPro et ajoute le composant TextMeshPro
            GameObject textObject = new GameObject("ScoreText");
            _scoreManager.scoreText = textObject.AddComponent<TextMeshPro>();
        }

        [Test]
        public void Add_Score()
        {
            _scoreManager.AddScore(10);
            
            Assert.AreEqual("10", _scoreManager.scoreText.text);
            Assert.AreEqual(10, _scoreManager.score);
            
            _scoreManager.AddScore(20);
            
            Assert.AreEqual("30", _scoreManager.scoreText.text);
            Assert.AreEqual(30, _scoreManager.score);
        }

        [TearDown]
        public void TearDown()
        {
            // Détruit les objets créés pendant le test
            Object.DestroyImmediate(_scoreManager.gameObject);
            Object.DestroyImmediate(_scoreManager.scoreText.gameObject);
        }
    }
}