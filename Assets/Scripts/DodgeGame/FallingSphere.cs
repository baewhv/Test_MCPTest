using UnityEngine;

namespace TestMCP.DodgeGame
{
    /// <summary>
    /// 상단에서 아래로 등속 낙하하며 플레이어 또는 바닥과의 충돌(Trigger 및 Collision)을 감지하는 구체 컴포넌트입니다.
    /// Kinematic Rigidbody와 FixedUpdate 기반 MovePosition을 사용하여 PhysX 물리 트리거를 100% 보장합니다.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class FallingSphere : MonoBehaviour
    {
        [Header("낙하 속도 설정")]
        [Tooltip("구체가 하강하는 기본 속도입니다.")]
        [SerializeField] private float _fallSpeed = 5.0f;

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SphereCollider _sphereCollider;

        private SphereSpawner _ownerSpawner;

        /// <summary>
        /// 낙하 속도 프로퍼티입니다.
        /// </summary>
        public float FallSpeed
        {
            get => _fallSpeed;
            set => _fallSpeed = value;
        }

        private void Awake()
        {
            EnsurePhysicsComponents();
        }

        private void EnsurePhysicsComponents()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            if (_sphereCollider == null)
            {
                _sphereCollider = GetComponent<SphereCollider>();
            }

            if (_sphereCollider != null)
            {
                _sphereCollider.isTrigger = true;
            }
        }

        /// <summary>
        /// 구체 초기화 및 소유 스포너, 낙하 속도를 설정합니다.
        /// </summary>
        /// <param name="spawner">구체를 관리하는 스포너 인스턴스</param>
        /// <param name="speed">낙하 속도</param>
        public void Initialize(SphereSpawner spawner, float speed)
        {
            _ownerSpawner = spawner;
            _fallSpeed = speed;
            EnsurePhysicsComponents();
        }

        private void FixedUpdate()
        {
            FallFixed();
        }

        /// <summary>
        /// PhysX 물리 시뮬레이션 주기에 맞춰 Rigidbody.MovePosition으로 하강 이동합니다.
        /// </summary>
        private void FallFixed()
        {
            if (_rigidbody != null && _rigidbody.isKinematic)
            {
                Vector3 nextPos = _rigidbody.position + (Vector3.down * (_fallSpeed * Time.fixedDeltaTime));
                _rigidbody.MovePosition(nextPos);
            }
            else
            {
                transform.Translate(Vector3.down * (_fallSpeed * Time.fixedDeltaTime), Space.World);
            }
        }

        /// <summary>
        /// 단위 테스트 또는 수동 호출용 Fall 메서드입니다.
        /// </summary>
        public void Fall(float deltaTime = -1.0f)
        {
            float dt = deltaTime > 0.0f ? deltaTime : (Time.deltaTime > 0.0f ? Time.deltaTime : 1.0f);
            transform.Translate(Vector3.down * (_fallSpeed * dt), Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleHit(collision.gameObject);
        }

        /// <summary>
        /// 충돌 대상 오브젝트(Player 또는 Ground)의 태그를 판정하여 이벤트를 처리하고 풀로 회수합니다.
        /// </summary>
        private void HandleHit(GameObject hitObject)
        {
            if (!gameObject.activeInHierarchy || hitObject == null)
            {
                return;
            }

            if (hitObject.CompareTag("Player"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnSphereHitPlayer();
                }

                ReturnToPool();
            }
            else if (hitObject.CompareTag("Ground"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnSphereHitGround();
                }

                ReturnToPool();
            }
        }

        /// <summary>
        /// 활성화된 구체를 소유 스포너의 풀로 반환하거나 비활성화합니다.
        /// </summary>
        public void ReturnToPool()
        {
            if (_ownerSpawner != null)
            {
                _ownerSpawner.ReturnSphere(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
