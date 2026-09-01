# C# 코딩 컨벤션 및 Unity 아키텍처 규칙 (C# Coding & Unity Architecture Rule)

## 1. C# 타입별 명칭 규칙 (Type Naming Conventions)
- 별도로 지정되지 않았다면 쓰임새에 맞게 접두사를 할당합니다:
  - **열거형(Enum)**: `E*` 접두사 사용 (예: `EMonsterType`, `EGameState`)
  - **인터페이스(Interface)**: `I*` 접두사 사용 (예: `IInteractable`, `IDamageable`)
  - **추상 클래스(Abstract Class)**: `Base*` 접두사 사용 (예: `BaseObject`, `BaseCharacter`)

## 2. 필드 및 메서드 명칭 (Rider IDE 규칙)
- JetBrains Rider IDE 기본 네이밍 검사 규칙을 준수하여 밑줄(Warning)이 발생하지 않도록 케이싱을 적용합니다:
  - **메서드 및 프로퍼티**: `PascalCase`
  - **Private / Protected 필드**: `_camelCase` (언더스코어 + 카멜케이스)
  - **로컬 변수 및 매개변수**: `camelCase`
  - **상수(const)**: `PascalCase` 또는 `ALL_CAPS`
- 만약 Rider 밑줄/경고 여부가 확인되지 않거나 모호한 경우, 사용자에게 네이밍 규칙 확인을 요청합니다.

## 3. Unity 직렬화 및 인스펙터 캡슐화 규칙 (Serialization & Inspector)
1. **인스펙터 노출 원칙**:
   - 인스펙터에 노출할 모든 필드는 **`[SerializeField] private` (또는 `[SerializeField] protected`)**로 선언하며, `public` 필드 선언은 엄격히 금지합니다.
2. **외부 읽기 프로퍼티 캡슐화**:
   - 외부 클래스에서 접근해야 하는 변수는 `public` 프로퍼티(`public int MaxHp => _maxHp;` 또는 `public int CurrentHp { get; private set; }`)로 캡슐화하여 제공합니다.
3. **데이터 구조체/클래스 직렬화**:
   - 인스펙터에 중첩 노출하거나 리스트로 다룰 커스텀 데이터 클래스/구조체는 반드시 **`[System.Serializable]`** 어트리뷰트를 명시합니다.
4. **인스펙터 가독성 향상**:
   - `[Header("...")], [Tooltip("...")], [Range(min, max)]`를 적극 활용하여 인스펙터 설정 편의성과 가독성을 높입니다.

## 4. 라이프사이클 및 초기화 순서 규칙 (Lifecycle & Initialization)
1. **`Awake()`**:
   - 자체 컴포넌트 캐싱(`GetComponent<T>()`) 및 내부 데이터 초기화만 전담합니다.
2. **`OnEnable()` / `OnDisable()` (메모리 누수 방지)**:
   - C# 이벤트(`Action`, `UnityAction`) 구독(`+=`)은 `OnEnable()`에서 수행하고, **`OnDisable()`에서 반드시 해제(`-=`)**합니다.
3. **`Start()`**:
   - 타 매니저, 타 오브젝트와의 외부 참조 연결 및 게임 루프를 시작합니다.

## 5. 성능 최적화 및 GC 방어 규칙 (Performance & GC Defense)
1. **`Update()` / `FixedUpdate()` 내 할당 금지**:
   - 매 프레임 실행되는 루프 내에서 `new` 객체 생성, 문자열 결합(`+`), LINQ 쿼리(`Where`, `Select`), `GetComponent` 호출을 엄격히 금지합니다 (필드 사전 캐싱 의무).
2. **해시값 캐싱**:
   - 애니메이터 파라미터나 셰이더 프로퍼티는 `Animator.StringToHash()`, `Shader.PropertyToID()`로 정적 캐싱하여 전달합니다.
3. **광범위 씬 탐색 최소화**:
   - `FindAnyObjectByType`, `GameObject.Find`, `GetComponentsInChildren` 등 광범위 탐색 API 사용을 지양하고 직렬화 바인딩을 기본으로 합니다.

## 6. Null 검사 및 이벤트 안전 수칙
1. **Unity Fake Null 안전 검사**:
   - `UnityEngine.Object` 상속 객체는 C# 널 조건 연산자(`?.`) 대신 **`if (target != null)` 명시적 비교**를 수행합니다.
2. **C# 이벤트 안전 호출**:
   - 순수 C# 이벤트나 델리게이트 트리거 시 `OnChanged?.Invoke(value);` 형태로 방어적으로 호출합니다.

## 7. Unity 아키텍처 및 에셋 운영 원칙
1. **입력 시스템**: New Input System (`UnityEngine.InputSystem`)을 기본으로 사용합니다.
2. **에셋 로딩**: `Resources.Load`를 지양하고 **Addressables(어드레서블)** 시스템을 통한 비동기 로딩을 기본으로 합니다.
3. **프리팹 우선 정책 (Prefab-First)**:
   - 씬에 배치할 모든 오브젝트, 카메라, 라이트는 반드시 **프리팹(Prefab)화**하여 인스턴스화합니다.
4. **런타임 표준 컴포넌트 준수**:
   - 일회성 에디터 스크립트(`EditorSceneBuilder`) 작성을 금지하며, 순수 런타임 컴포넌트(`MonoBehaviour`, `Spawner`)로 구현합니다.
5. **데이터 분리 (ScriptableObject)**:
   - 정적 기획 수치 및 테이블 데이터는 `[CreateAssetMenu]`를 적용한 `ScriptableObject`로 분리하여 관리합니다.
