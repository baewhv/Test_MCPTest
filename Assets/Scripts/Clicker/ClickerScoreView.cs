using TMPro;
using UnityEngine;

namespace Clicker
{
    public class ClickerScoreView : MonoBehaviour
    {
        [SerializeField] private ClickerController _controller;
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void OnEnable()
        {
            if (_controller != null)
            {
                _controller.OnScoreChanged += UpdateScoreText;
                UpdateScoreText(_controller.Score);
            }
        }

        private void OnDisable()
        {
            if (_controller != null)
            {
                _controller.OnScoreChanged -= UpdateScoreText;
            }
        }

        private void UpdateScoreText(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }
    }
}
