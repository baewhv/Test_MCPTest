using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TestMCP.DodgeGame
{
    /// <summary>
    /// 점수, 생명력, 게임오버 팝업 등 인게임 UI 뷰를 제어하고 버튼 이벤트를 처리하는 매니저 클래스입니다.
    /// New Input System 환경에서 레거시 StandaloneInputModule이 존재할 경우 자동 교체합니다.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("인게임 HUD UI")]
        [Tooltip("화면 상단 점수를 표시하는 텍스트입니다.")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Tooltip("화면 좌측 상단 생명력을 표시하는 텍스트입니다.")]
        [SerializeField] private TextMeshProUGUI _lifeText;

        [Header("게임오버 팝업 UI")]
        [Tooltip("게임오버 시 노출되는 결과 패널 오브젝트입니다.")]
        [SerializeField] private GameObject _gameOverPanel;

        [Tooltip("게임오버 패널 내 최종 점수를 표시하는 텍스트입니다.")]
        [SerializeField] private TextMeshProUGUI _finalScoreText;

        [Tooltip("게임 재시작 버튼입니다.")]
        [SerializeField] private Button _restartButton;

        private void Awake()
        {
            FixEventSystemInputModule();
        }

        private void Start()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartButtonClicked);
            }

            SubscribeGameManagerEvents();
        }

        /// <summary>
        /// 씬 내 EventSystem에 레거시 StandaloneInputModule이 있을 경우 InputSystemUIInputModule로 자동 교체합니다.
        /// </summary>
        private void FixEventSystemInputModule()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                eventSystem = FindFirstObjectByType<EventSystem>();
            }

            if (eventSystem != null)
            {
                StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
                if (legacyModule != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(legacyModule);
                    }
                    else
                    {
                        DestroyImmediate(legacyModule);
                    }
                }

                InputSystemUIInputModule newModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                if (newModule == null)
                {
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }
        }

        private void OnDestroy()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            }

            UnsubscribeGameManagerEvents();
        }

        /// <summary>
        /// GameManager의 상태 변경 이벤트를 구독합니다.
        /// </summary>
        private void SubscribeGameManagerEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnLifeChanged += UpdateLife;
                GameManager.Instance.OnGameOver += ShowGameOver;

                // 초기 상태 동기화
                UpdateScore(GameManager.Instance.CurrentScore);
                UpdateLife(GameManager.Instance.CurrentLife);
            }
        }

        /// <summary>
        /// GameManager의 상태 변경 이벤트 구독을 해제합니다.
        /// </summary>
        private void UnsubscribeGameManagerEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= UpdateScore;
                GameManager.Instance.OnLifeChanged -= UpdateLife;
                GameManager.Instance.OnGameOver -= ShowGameOver;
            }
        }

        /// <summary>
        /// 화면 상단 점수 UI 텍스트를 갱신합니다.
        /// </summary>
        /// <param name="score">현재 점수</param>
        public void UpdateScore(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"SCORE: {score}";
            }
        }

        /// <summary>
        /// 화면 좌측 상단 생명력 UI 텍스트를 갱신합니다.
        /// </summary>
        /// <param name="life">현재 남은 생명력</param>
        public void UpdateLife(int life)
        {
            if (_lifeText != null)
            {
                _lifeText.text = $"LIFE: {life}";
            }
        }

        /// <summary>
        /// 게임오버 결과 패널을 활성화하고 최종 점수를 표시합니다.
        /// </summary>
        /// <param name="finalScore">게임오버 시점의 최종 점수</param>
        public void ShowGameOver(int finalScore)
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }

            if (_finalScoreText != null)
            {
                _finalScoreText.text = $"FINAL SCORE: {finalScore}";
            }
        }

        /// <summary>
        /// 재시작 버튼 클릭 시 호출되어 게임오버 패널을 닫고 새 게임을 시작합니다.
        /// </summary>
        private void OnRestartButtonClicked()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }
    }
}
