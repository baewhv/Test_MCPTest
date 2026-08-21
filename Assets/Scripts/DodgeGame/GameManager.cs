using System;
using UnityEngine;

namespace TestMCP.DodgeGame
{
    /// <summary>
    /// 낙하 회피 게임의 진행 상태, 점수, 생명력을 총괄 관리하고 이벤트를 브로드캐스팅하는 싱글톤 매니저입니다.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("게임 수치 설정")]
        [Tooltip("플레이어의 최대 생명력입니다.")]
        [SerializeField] private int _maxLife = 5;

        [Header("참조 컴포넌트")]
        [Tooltip("플레이어 컨트롤러 컴포넌트 참조입니다.")]
        [SerializeField] private PlayerController _player;

        [Tooltip("구체 스포너 컴포넌트 참조입니다.")]
        [SerializeField] private SphereSpawner _spawner;

        [Tooltip("UI 매니저 컴포넌트 참조입니다.")]
        [SerializeField] private UIManager _uiManager;

        private int _currentScore;
        private int _currentLife;
        private bool _isGameOver;

        /// <summary>
        /// 점수 변경 시 호출되는 이벤트입니다. (인자: 현재 점수)
        /// </summary>
        public event Action<int> OnScoreChanged;

        /// <summary>
        /// 생명력 변경 시 호출되는 이벤트입니다. (인자: 현재 생명력)
        /// </summary>
        public event Action<int> OnLifeChanged;

        /// <summary>
        /// 게임오버 시 호출되는 이벤트입니다. (인자: 최종 점수)
        /// </summary>
        public event Action<int> OnGameOver;

        /// <summary>
        /// 현재 누적 점수 프로퍼티입니다.
        /// </summary>
        public int CurrentScore => _currentScore;

        /// <summary>
        /// 현재 남은 생명력 프로퍼티입니다.
        /// </summary>
        public int CurrentLife => _currentLife;

        /// <summary>
        /// 최대 생명력 프로퍼티입니다.
        /// </summary>
        public int MaxLife => _maxLife;

        /// <summary>
        /// 게임오버 여부 프로퍼티입니다.
        /// </summary>
        public bool IsGameOver => _isGameOver;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _currentLife = _maxLife;
            }
            else if (Instance != this)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
                return;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            StartGame();
        }

        /// <summary>
        /// 게임을 초기 상태(점수 0, 최대 라이프, 스폰 시작)로 시작합니다.
        /// </summary>
        public void StartGame()
        {
            _isGameOver = false;
            _currentScore = 0;
            _currentLife = _maxLife;

            OnScoreChanged?.Invoke(_currentScore);
            OnLifeChanged?.Invoke(_currentLife);

            if (_player != null)
            {
                _player.ResetPosition();
                _player.SetInputActive(true);
            }

            if (_spawner != null)
            {
                _spawner.ClearAllActiveSpheres();
                _spawner.StartSpawning();
            }
        }

        /// <summary>
        /// 구체가 플레이어를 피해 바닥에 도달했을 때 호출되어 점수를 1 증가시킵니다.
        /// </summary>
        public void OnSphereHitGround()
        {
            if (_isGameOver)
            {
                return;
            }

            _currentScore++;
            OnScoreChanged?.Invoke(_currentScore);
        }

        /// <summary>
        /// 구체가 플레이어와 충돌했을 때 호출되어 생명력을 1 감소시키고 0 도달 시 게임오버를 트리거합니다.
        /// </summary>
        public void OnSphereHitPlayer()
        {
            if (_isGameOver)
            {
                return;
            }

            _currentLife--;
            OnLifeChanged?.Invoke(_currentLife);

            if (_currentLife <= 0)
            {
                TriggerGameOver();
            }
        }

        /// <summary>
        /// 게임오버 상태로 전환하고 구체 스폰 정지 및 플레이어 조작을 차단합니다.
        /// </summary>
        public void TriggerGameOver()
        {
            if (_isGameOver)
            {
                return;
            }

            _isGameOver = true;

            if (_spawner != null)
            {
                _spawner.StopSpawning();
                _spawner.ClearAllActiveSpheres();
            }

            if (_player != null)
            {
                _player.SetInputActive(false);
            }

            OnGameOver?.Invoke(_currentScore);
        }

        /// <summary>
        /// 게임오버 후 새로운 게임을 재시작합니다.
        /// </summary>
        public void RestartGame()
        {
            StartGame();
        }
    }
}
