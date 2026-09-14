# 프로젝트 환경 명세서 (Project Environment Specification)

- **환경 설정 상태 (Setup Status)**: `[SETUP_COMPLETED]`

> [!NOTE]
> GitHub 토큰, Notion 토큰 등 민감한 인증 키(API Key/PAT)는 이곳에 입력하지 마시고, MCP 설정(`config/mcp_config.json`)을 통해 안전하게 관리됩니다.

---

## 1. 버전 관리 및 저장소 정보 (Git & GitHub)
- **GitHub Repository URL**: `https://github.com/baewhv/Test_MCPTest`
- **Default Integration Branch**: `develop`
- **Release Branch**: `main`
- **Worktree Parent Directory**: `../TestMCP_worktrees`

---

## 2. 외부 연동 명세 (Notion & External Services)
- **Notion Database Name**: `학습일지`
- **Notion Database ID**: `13cc49b1-3a07-814e-b7b5-cf14b64ca1ee`
- **Notion Page Title Format**: `[YYYY-MM-DD] 작업 기록`

---

## 3. 호스트 OS 및 Unity 환경 명세 (Environment Specification)
- **Host OS (작업 환경 운영체제)**: `Windows` <!-- `Windows` 또는 `macOS` 지정 -->
- **Unity Project Name**: `TestMCP`
- **Unity Version**: `6000.5.8f1`
- **Unity Editor Path**: `C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe`
  - *(macOS 예시: `/Applications/Unity/Hub/Editor/6000.5.8f1/Unity.app/Contents/MacOS/Unity`)*
- **Target Platform**: `PC, Mac & Linux Standalone`
- **Asset Root**: `Assets/`
- **Raw Imports Root (Submodule Boundary)**: `Assets/_Imports/`
- **Default Screenshot Output**: `Assets/Screenshots`
- **Core Loop Test Scene**: `Assets/Scenes/SampleScene.unity`

---

## 4. 아키텍처 및 데이터 드리븐 인프라 기준 (Architecture & Data Baseline)
- **Physics Engine Mode**: `2D (Rigidbody2D / Collider2D)`
- **Data-Driven Architecture**: `ScriptableObject 기반 데이터 분리 (Assets/ScriptableObjects/Data/)`
- **Object Pooling Pattern**: `제네릭 풀링 시스템 (Generic Object Pool / IPoolable)`
- **Core State Machine**: `FSM 기반 상태 제어`
- **Input System**: `New Input System (com.unity.inputsystem)`

---
## 5. 필수 5대 도구 인프라 명세 (Essential Tools & MCPs)

| 도구 명칭 | 구분 | 주요 전담 역할 | 우선순위 및 대체 방안 (Fallback) |
| :--- | :--- | :--- | :--- |
| **GitHub MCP** | MCP Server | PR 생성, 커밋 푸시, 이슈/리뷰 코멘트 등록 | 필수 (미연결 시 PR 생성 및 자동 머지 인계 불가) |
| **Unity MCP** | MCP Server | 에디터 플레이 제어, 콘솔 에러 읽기, 런타임/시각 검수 | **최우선 (1순위)** / 에디터 미기동 시 Unity CLI로 자동 대체 |
| **Unity CLI** | CLI Tool | 백그라운드 무인 컴파일 검증, NUnit 무인 테스트, 신규 프로젝트 셋업 | **대체 및 셋업 (2순위)** / Unity MCP 부재 시 무인 검증 전담 |
| **Notion MCP** | MCP Server | 일일 학습일지 자동 생성 및 접힌 토글 피드백 | 선택 (미연결 시 로컬 로그 보존) |
| **Rider MCP** | MCP Server | C# 네이밍 컨벤션 검사 및 IDE 진단 연동 | 선택 (미연결 시 정적 코딩룰 자체 검증) |

