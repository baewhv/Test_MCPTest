using System;
using UnityEngine;

namespace Clicker
{
    public class ClickerController : MonoBehaviour
    {
        [SerializeField] private int _score = 0;
        public int Score => _score;

        public event Action<int> OnScoreChanged;

        public void AddScore(int amount)
        {
            if (amount <= 0) return;
            _score += amount;
            OnScoreChanged?.Invoke(_score);
        }

        public void ResetScore()
        {
            _score = 0;
            OnScoreChanged?.Invoke(_score);
        }
    }
}
