using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class Ghost_Test
    {
        private GhostMovement _ghostMovement;
        
        /// <summary>
        /// Initialise l'objet GhostMovement avant chaque test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Crée un GameObject et lui ajoute le composant GhostMovement
            _ghostMovement = new GameObject("GhostMovement").AddComponent<GhostMovement>();
        }

        /// <summary>
        /// Vérifie que la mise à jour des directions disponibles fonctionne correctement.
        /// </summary>
        [Test]
        public void TestMajAvalableDirection()
        {
            // Définit la dernière direction comme étant vers la gauche
            _ghostMovement._lastDirection = Vector2.left;

            // Initialise une liste de directions possibles
            var directions = new List<Vector2>
            {
                Vector2.left,
                Vector2.down,
                Vector2.right
            };

            // Met à jour les directions disponibles
            _ghostMovement.MajAvalableDirections(directions);
            
            // Vérifie que seule la direction vers le bas reste disponible
            Assert.AreEqual(directions.Count, 1);
            Assert.AreEqual(directions[0], Vector2.down);
        }
        
        /// <summary>
        /// Nettoie les objets créés après chaque test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Détruit l'objet GhostMovement après le test
            Object.DestroyImmediate(_ghostMovement);
        }
    }
}