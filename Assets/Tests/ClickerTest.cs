using NUnit.Framework;
using UnityEngine;
using Clicker;

namespace Tests
{
    public class ClickerTest
    {
        [Test]
        public void Clicker_AddScore_IncreasesScore()
        {
            var go = new GameObject();
            var controller = go.AddComponent<ClickerController>();

            controller.AddScore(1);

            Assert.AreEqual(1, controller.Score);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Clicker_AddScore_ZeroOrNegative_DoesNotChangeScore()
        {
            var go = new GameObject();
            var controller = go.AddComponent<ClickerController>();

            controller.AddScore(-5);
            controller.AddScore(0);

            Assert.AreEqual(0, controller.Score);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Clicker_ResetScore_SetsScoreToZero()
        {
            var go = new GameObject();
            var controller = go.AddComponent<ClickerController>();

            controller.AddScore(10);
            controller.ResetScore();

            Assert.AreEqual(0, controller.Score);
            Object.DestroyImmediate(go);
        }
    }
}
