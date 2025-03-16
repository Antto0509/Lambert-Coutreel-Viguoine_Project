using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class Ghost_Test
    {
        private GhostMovement _ghostMovement;
        
        [SetUp]
        public void Setup()
        {
            _ghostMovement = new GameObject("GhostMovement").AddComponent<GhostMovement>();
        }

        [Test]
        public void TestMajAvalableDirection()
        {
            _ghostMovement._lastDirection = Vector2.left;

            var directons = new List<Vector2>
            {
                Vector2.left,
                Vector2.down,
                Vector2.right
            };

            _ghostMovement.MajAvalableDirections(directons);
            
            Assert.AreEqual(directons.Count, 1);
            Assert.AreEqual(directons[0], Vector2.down);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_ghostMovement);
        }
    }
}