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
  - **Public 필드**: `PascalCase` (필드 공개는 지양하되 필요시)
  - **로컬 변수 및 매개변수**: `camelCase`
  - **상수(const)**: `PascalCase` 또는 `ALL_CAPS`
- 만약 Rider 밑줄/경고 여부가 확인되지 않거나 모호한 경우, 사용자에게 네이밍 규칙 확인을 요청합니다.

## 3. Unity 아키텍처 및 설계 주의사항 (Unity Architecture Guidelines)

### ① 입력 (Input)
- 기존 레거시 `Input` 대신 **New Input System (`UnityEngine.InputSystem`)**을 기본으로 사용합니다.

### ② 동적 에셋 로딩 (Asset Loading)
- `Resources.Load` 사용을 엄격히 지양하고, **Addressables(어드레서블) 시스템**을 통한 비동기 동적 로딩을 사용합니다.

### ③ 직접 직렬화 참조 및 탐색 최소화 (Optimization & Direct Reference)
- 동일 게임오브젝트 및 계층 내에서 참조 가능한 컴포넌트는 `[SerializeField] private`로 선언하여 인스펙터 직렬화 할당을 기본으로 합니다.
- `GetComponentsInChildren`, `FindAnyObjectByType`, `GameObject.Find` 등 광범위 씬 탐색 API는 반드시 사용해야 하는 타당한 이유가 있을 때만, **코드 구조를 작성하기 전에 사용자에게 사전 고지 및 승인**을 받습니다.

### ④ 프리팹 우선 정책 (Prefab-First Policy)
- 씬에 직접 배치할 모든 오브젝트는 **프리팹(Prefab)화**하여 관리 및 배치합니다.
- 메인 카메라(Camera) 및 기본 라이트(Light) 또한 프리팹으로 생성하여 배치합니다.

### ⑤ 런타임 표준 아키텍처 준수 (No Editor Scene Builders)
- 게임 로직 및 씬/오브젝트 구성 시 `UnityEditor` API를 사용한 일회성 자동 생성 스크립트(`EditorSceneBuilder`, `MenuItem` 기반 씬 생성기 등) 작성을 엄격히 금지합니다.
- 모든 게임 동작은 **런타임 컴포넌트(`MonoBehaviour`, `Spawner`, `GameManager`) 및 유니티 표준 프리팹 에셋**으로 구현되어야 합니다.

### ⑥ 태그 및 레이어 (Tags & Layers)
- 태그 및 레이어는 용도에 맞게 최소한으로만 정의하여 사용합니다.
