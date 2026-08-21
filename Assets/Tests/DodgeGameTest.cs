using NUnit.Framework;
using UnityEngine;

namespace TestMCP.DodgeGame.Tests
{
    /// <summary>
    /// 낙하 회피 게임(Falling Dodge Game)의 핵심 로직(이동 경계 제한, 입력 제어, 점수 획득, 라이프 감소, 게임오버, 재시작, 구체 낙하) 검증을 위한 NUnit 단위 테스트 클래스입니다.
    /// </summary>
    public class DodgeGameTest
    {
        private GameObject _gameManagerObject;
        private GameManager _gameManager;
        private GameObject _playerObject;
        private PlayerController _player;
        private GameObject _sphereObject;
        private FallingSphere _sphere;

        [SetUp]
        public void SetUp()
        {
            _gameManagerObject = new GameObject("Test_GameManager");
            _gameManager = _gameManagerObject.AddComponent<GameManager>();

            _playerObject = new GameObject("Test_Player");
            _player = _playerObject.AddComponent<PlayerController>();
            _player.Initialize(-8.0f, 8.0f);

            _sphereObject = new GameObject("Test_FallingSphere");
            _sphere = _sphereObject.AddComponent<FallingSphere>();
            _sphere.Initialize(null, 5.0f);
        }

        [TearDown]
        public void TearDown()
        {
            if (_sphereObject != null)
            {
                Object.DestroyImmediate(_sphereObject);
            }

            if (_playerObject != null)
            {
                Object.DestroyImmediate(_playerObject);
            }

            if (_gameManagerObject != null)
            {
                Object.DestroyImmediate(_gameManagerObject);
            }
        }

        [Test]
        public void PlayerController_Move_ClampsWithinScreenBounds()
        {
            // Arrange
            _player.transform.position = Vector3.zero;

            // Act - 오른쪽으로 화면 밖까지 이동 시도 (충분히 큰 delta)
            _player.Move(100.0f, 1.0f);

            // Assert
            Assert.AreEqual(8.0f, _player.transform.position.x, "플레이어 X 좌표는 최대 경계(8.0f)로 정확히 클램핑되어야 합니다.");

            // Act - 왼쪽으로 화면 밖까지 이동 시도
            _player.Move(-200.0f, 1.0f);

            // Assert
            Assert.AreEqual(-8.0f, _player.transform.position.x, "플레이어 X 좌표는 최소 경계(-8.0f)로 정확히 클램핑되어야 합니다.");
        }

        [Test]
        public void PlayerController_SetInputActive_TogglesControl()
        {
            // Arrange & Act
            _player.SetInputActive(false);

            // Assert
            Assert.IsFalse(_player.IsInputActive, "SetInputActive(false) 호출 시 IsInputActive는 false여야 합니다.");

            // Act
            _player.SetInputActive(true);

            // Assert
            Assert.IsTrue(_player.IsInputActive, "SetInputActive(true) 호출 시 IsInputActive는 true여야 합니다.");
        }

        [Test]
        public void PlayerController_ResetPosition_SetsPositionToOrigin()
        {
            // Arrange
            _player.transform.position = new Vector3(5.0f, -4.0f, 0.0f);

            // Act
            _player.ResetPosition();

            // Assert
            Assert.AreEqual(0.0f, _player.transform.position.x, "ResetPosition 호출 시 X 좌표는 0으로 초기화되어야 합니다.");
            Assert.AreEqual(-4.0f, _player.transform.position.y, "ResetPosition 호출 시 Y 좌표는 유지되어야 합니다.");
        }

        [Test]
        public void GameManager_InitialState_ScoreIsZero_AndLifeIsFive()
        {
            // Assert
            Assert.AreEqual(0, _gameManager.CurrentScore, "초기 점수는 0점이어야 합니다.");
            Assert.AreEqual(5, _gameManager.CurrentLife, "초기 생명력은 5여야 합니다.");
            Assert.IsFalse(_gameManager.IsGameOver, "초기 상태에서 게임오버가 아니어야 합니다.");
        }

        [Test]
        public void GameManager_OnSphereHitGround_IncreasesScoreByOne()
        {
            // Act
            _gameManager.OnSphereHitGround();

            // Assert
            Assert.AreEqual(1, _gameManager.CurrentScore, "구체가 바닥에 닿으면 점수가 1 증가해야 합니다.");

            // Act
            _gameManager.OnSphereHitGround();

            // Assert
            Assert.AreEqual(2, _gameManager.CurrentScore, "연속 2회 바닥 충돌 시 점수는 2가 되어야 합니다.");
        }

        [Test]
        public void GameManager_OnSphereHitPlayer_DecreasesLifeByOne()
        {
            // Act
            _gameManager.OnSphereHitPlayer();

            // Assert
            Assert.AreEqual(4, _gameManager.CurrentLife, "구체 피격 시 생명력이 1 감소하여 4가 되어야 합니다.");
        }

        [Test]
        public void GameManager_LifeReachesZero_TriggersGameOver()
        {
            // Act - 5회 피격
            for (int i = 0; i < 5; i++)
            {
                _gameManager.OnSphereHitPlayer();
            }

            // Assert
            Assert.AreEqual(0, _gameManager.CurrentLife, "5회 피격 후 생명력은 0이어야 합니다.");
            Assert.IsTrue(_gameManager.IsGameOver, "생명력이 0이 되면 게임오버 상태가 되어야 합니다.");

            // Act - 게임오버 후 바닥 충돌 발생 시 점수가 더 이상 오르지 않는지 검증
            int scoreBefore = _gameManager.CurrentScore;
            _gameManager.OnSphereHitGround();
            Assert.AreEqual(scoreBefore, _gameManager.CurrentScore, "게임오버 상태에서는 추가 점수가 누적되지 않아야 합니다.");
        }

        [Test]
        public void GameManager_RestartGame_ResetsScoreAndLife()
        {
            // Arrange - 게임 진행 및 게임오버 상태 만들기
            _gameManager.OnSphereHitGround();
            _gameManager.OnSphereHitGround();
            for (int i = 0; i < 5; i++)
            {
                _gameManager.OnSphereHitPlayer();
            }
            Assert.IsTrue(_gameManager.IsGameOver);

            // Act - 재시작
            _gameManager.RestartGame();

            // Assert
            Assert.AreEqual(0, _gameManager.CurrentScore, "재시작 후 점수는 0으로 리셋되어야 합니다.");
            Assert.AreEqual(5, _gameManager.CurrentLife, "재시작 후 생명력은 5로 리셋되어야 합니다.");
            Assert.IsFalse(_gameManager.IsGameOver, "재시작 후 게임오버 상태는 해제되어야 합니다.");
        }

        [Test]
        public void FallingSphere_Fall_MovesDownwards()
        {
            // Arrange
            _sphere.transform.position = new Vector3(0.0f, 6.0f, 0.0f);

            // Act - 1초 동안 낙하
            _sphere.Fall(1.0f);

            // Assert - 속도 5.0f로 1초 낙하 시 Y는 6.0 - 5.0 = 1.0f
            Assert.AreEqual(1.0f, _sphere.transform.position.y, 0.001f, "구체는 낙하 속도에 비례하여 Y축 음수 방향으로 하강해야 합니다.");
        }
    }
}
