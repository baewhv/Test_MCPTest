using UnityEngine;
using UnityEngine.InputSystem;

namespace TestMCP.DodgeGame
{
    /// <summary>
    /// 플레이어 캡슐의 좌우 이동 및 화면 경계 제한(Clamp)을 제어하는 컨트롤러 클래스입니다.
    /// New Input System(PlayerInput 컴포넌트 및 Keyboard.current)을 활용하여 순수 New Input System 환경에서 구동됩니다.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("이동 설정")]
        [Tooltip("플레이어의 좌우 이동 속도입니다.")]
        [SerializeField] private float _moveSpeed = 8.0f;

        [Header("경계 제한 설정")]
        [Tooltip("플레이어가 이동할 수 있는 최소 X 좌표입니다.")]
        [SerializeField] private float _minX = -8.0f;

        [Tooltip("플레이어가 이동할 수 있는 최대 X 좌표입니다.")]
        [SerializeField] private float _maxX = 8.0f;

        private bool _isInputActive = true;
        private float _inputX;

        /// <summary>
        /// 이동 가능 여부 프로퍼티입니다.
        /// </summary>
        public bool IsInputActive => _isInputActive;

        /// <summary>
        /// 이동 속도 프로퍼티입니다.
        /// </summary>
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        /// <summary>
        /// 플레이어의 이동 가능 X 경계값을 초기화합니다.
        /// </summary>
        /// <param name="minX">최소 X 좌표</param>
        /// <param name="maxX">최대 X 좌표</param>
        public void Initialize(float minX, float maxX)
        {
            _minX = minX;
            _maxX = maxX;
        }

        /// <summary>
        /// 플레이어의 키보드/컨트롤러 입력 활성화 상태를 설정합니다.
        /// </summary>
        /// <param name="isActive">활성화 여부</param>
        public void SetInputActive(bool isActive)
        {
            _isInputActive = isActive;
            if (!isActive)
            {
                _inputX = 0.0f;
            }
        }

        /// <summary>
        /// New Input System의 PlayerInput 컴포넌트로부터 Move 액션 메시지를 수신합니다.
        /// </summary>
        /// <param name="value">수신된 InputValue (Vector2)</param>
        public void OnMove(InputValue value)
        {
            if (!_isInputActive)
            {
                _inputX = 0.0f;
                return;
            }

            Vector2 moveVector = value.Get<Vector2>();
            _inputX = moveVector.x;
        }

        private void Update()
        {
            if (!_isInputActive)
            {
                return;
            }

            float horizontal = _inputX;

            // PlayerInput 콜백이 비어있거나 직접 키보드 입력 시 Keyboard.current 폴링으로 대체
            if (Mathf.Abs(horizontal) < Mathf.Epsilon)
            {
                horizontal = ReadKeyboardInput();
            }

            if (Mathf.Abs(horizontal) > Mathf.Epsilon)
            {
                Move(horizontal);
            }
        }

        /// <summary>
        /// New Input System의 Keyboard.current를 통해 키보드 입력을 폴링합니다. (레거시 Input 클래스 미사용)
        /// </summary>
        /// <returns>좌측(-1), 우측(+1), 미입력(0)</returns>
        private float ReadKeyboardInput()
        {
            float horizontal = 0.0f;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    horizontal -= 1.0f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    horizontal += 1.0f;
                }
            }

            return horizontal;
        }

        /// <summary>
        /// 입력값에 따라 플레이어를 좌우로 이동시키고 경계 내로 클램핑합니다.
        /// </summary>
        /// <param name="horizontalInput">수평 입력값 (-1 ~ 1)</param>
        /// <param name="deltaTime">프레임 경과 시간 (0 이하 전달 시 Time.deltaTime 또는 기본 1.0f 사용)</param>
        public void Move(float horizontalInput, float deltaTime = -1.0f)
        {
            float dt = deltaTime > 0.0f ? deltaTime : (Time.deltaTime > 0.0f ? Time.deltaTime : 1.0f);
            Vector3 currentPosition = transform.position;
            float targetX = currentPosition.x + (horizontalInput * _moveSpeed * dt);
            float clampedX = Mathf.Clamp(targetX, _minX, _maxX);

            transform.position = new Vector3(clampedX, currentPosition.y, currentPosition.z);
        }

        /// <summary>
        /// 플레이어 위치를 원점 또는 지정된 위치로 리셋합니다.
        /// </summary>
        public void ResetPosition()
        {
            _inputX = 0.0f;
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(0.0f, currentPosition.y, currentPosition.z);
        }
    }
}
