using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestMCP.DodgeGame
{
    /// <summary>
    /// 구체(FallingSphere)의 오브젝트 풀을 관리하고 주기적으로 화면 상단에 스폰하는 스포너 클래스입니다.
    /// 인스펙터 프리팹 미할당 시에도 런타임 폴백 구체를 자동 생성하여 Null 예외를 방지합니다.
    /// </summary>
    public class SphereSpawner : MonoBehaviour
    {
        [Header("프리팹 및 풀 설정")]
        [Tooltip("스폰할 구체 프리팹입니다.")]
        [SerializeField] private FallingSphere _spherePrefab;

        [Tooltip("초기 오브젝트 풀 크기입니다.")]
        [SerializeField] private int _poolSize = 20;

        [Header("스폰 설정")]
        [Tooltip("구체 생성 주기(초 단위)입니다.")]
        [SerializeField] private float _spawnInterval = 0.8f;

        [Tooltip("구체 스폰 Y 좌표입니다.")]
        [SerializeField] private float _spawnY = 6.0f;

        [Tooltip("스폰 위치의 최소 X 좌표입니다.")]
        [SerializeField] private float _minSpawnX = -7.5f;

        [Tooltip("스폰 위치의 최대 X 좌표입니다.")]
        [SerializeField] private float _maxSpawnX = 7.5f;

        [Tooltip("스폰된 구체의 기본 낙하 속도입니다.")]
        [SerializeField] private float _sphereFallSpeed = 5.5f;

        private Queue<FallingSphere> _pool;
        private List<FallingSphere> _activeList;
        private Coroutine _spawnCoroutine;
        private bool _isInitialized;
        private GameObject _fallbackPrefabObj;

        /// <summary>
        /// 스폰 주기 프로퍼티입니다.
        /// </summary>
        public float SpawnInterval
        {
            get => _spawnInterval;
            set => _spawnInterval = value;
        }

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 오브젝트 풀을 초기화하고 사전 인스턴스를 생성합니다.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _pool = new Queue<FallingSphere>(_poolSize);
            _activeList = new List<FallingSphere>(_poolSize);

            // 프리팹 누락 시 자동 복구 로직 (에셋 로드 또는 런타임 프리미티브 생성)
            EnsureSpherePrefabValid();

            if (_spherePrefab != null)
            {
                for (int i = 0; i < _poolSize; i++)
                {
                    FallingSphere instance = CreateNewSphere();
                    instance.gameObject.SetActive(false);
                    _pool.Enqueue(instance);
                }
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 프리팹 참조가 없을 경우 Assets 경로에서 로드하거나 런타임 폴백 프리팹을 생성합니다.
        /// </summary>
        private void EnsureSpherePrefabValid()
        {
            if (_spherePrefab != null)
            {
                return;
            }

#if UNITY_EDITOR
            _spherePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<FallingSphere>("Assets/Prefabs/FallingSphere.prefab");
            if (_spherePrefab != null)
            {
                return;
            }
#endif

            // 런타임 폴백 구체 생성
            _fallbackPrefabObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _fallbackPrefabObj.name = "FallbackFallingSphere";
            _fallbackPrefabObj.transform.SetParent(transform);

            SphereCollider collider = _fallbackPrefabObj.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            Rigidbody rb = _fallbackPrefabObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            _spherePrefab = _fallbackPrefabObj.AddComponent<FallingSphere>();
            _fallbackPrefabObj.SetActive(false);
        }

        /// <summary>
        /// 새로운 구체 인스턴스를 생성합니다.
        /// </summary>
        /// <returns>생성된 FallingSphere 컴포넌트</returns>
        private FallingSphere CreateNewSphere()
        {
            EnsureSpherePrefabValid();

            FallingSphere instance = Instantiate(_spherePrefab, transform);
            instance.Initialize(this, _sphereFallSpeed);
            return instance;
        }

        /// <summary>
        /// 구체 스폰 코루틴을 시작합니다.
        /// </summary>
        public void StartSpawning()
        {
            StopSpawning();
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        /// <summary>
        /// 구체 스폰 코루틴을 중단합니다.
        /// </summary>
        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        /// <summary>
        /// 현재 씬에 활성화되어 있는 모든 구체를 비활성화하고 풀로 회수합니다.
        /// </summary>
        public void ClearAllActiveSpheres()
        {
            if (_activeList == null)
            {
                return;
            }

            for (int i = _activeList.Count - 1; i >= 0; i--)
            {
                FallingSphere sphere = _activeList[i];
                if (sphere != null)
                {
                    sphere.gameObject.SetActive(false);
                    if (_pool != null && !_pool.Contains(sphere))
                    {
                        _pool.Enqueue(sphere);
                    }
                }
            }

            _activeList.Clear();
        }

        /// <summary>
        /// 풀에서 구체를 인출하거나 필요 시 새로 생성하여 활성화 목록에 추가합니다.
        /// </summary>
        /// <returns>활성화 가능한 FallingSphere 인스턴스</returns>
        public FallingSphere GetSphere()
        {
            FallingSphere sphere;

            if (_pool != null && _pool.Count > 0)
            {
                sphere = _pool.Dequeue();
            }
            else
            {
                sphere = CreateNewSphere();
            }

            if (sphere != null)
            {
                sphere.gameObject.SetActive(true);
                if (_activeList != null && !_activeList.Contains(sphere))
                {
                    _activeList.Add(sphere);
                }
            }

            return sphere;
        }

        /// <summary>
        /// 사용이 끝난 구체를 풀로 반환하고 비활성화합니다.
        /// </summary>
        /// <param name="sphere">반환할 구체 컴포넌트</param>
        public void ReturnSphere(FallingSphere sphere)
        {
            if (sphere == null)
            {
                return;
            }

            if (_activeList != null)
            {
                _activeList.Remove(sphere);
            }

            sphere.gameObject.SetActive(false);

            if (_pool != null && !_pool.Contains(sphere))
            {
                _pool.Enqueue(sphere);
            }
        }

        /// <summary>
        /// 지정된 주기마다 무작위 X 위치에 구체를 인출하여 배치하는 코루틴입니다.
        /// </summary>
        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnInterval);

                float randomX = Random.Range(_minSpawnX, _maxSpawnX);
                Vector3 spawnPosition = new Vector3(randomX, _spawnY, 0.0f);

                FallingSphere sphere = GetSphere();
                if (sphere != null)
                {
                    sphere.transform.position = spawnPosition;
                    sphere.Initialize(this, _sphereFallSpeed);
                }
            }
        }

        private void OnDestroy()
        {
            if (_fallbackPrefabObj != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_fallbackPrefabObj);
                }
                else
                {
                    DestroyImmediate(_fallbackPrefabObj);
                }
            }
        }
    }
}
