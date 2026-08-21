using UnityEngine;

namespace Clicker
{
    public class ClickerTarget : MonoBehaviour, IClickable
    {
        [SerializeField] private ClickerController _controller;

        public void OnClicked()
        {
            if (_controller != null)
            {
                _controller.AddScore(1);
            }
        }
    }
}
