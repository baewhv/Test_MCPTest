---
name: unity-cli-runner
description: 신규 프로젝트 셋업 및 Unity MCP 미연결 시 백그라운드 무인 검증/테스트를 수행하는 공식 Unity CLI 도구 스킬
---

# Unity CLI Runner Skill (Setup & Fallback Verification)

> [!NOTE]
> **검증 2중 파이프라인 (Dual Verification Hierarchy)**:
> 1. **1순위 (최우선)**: 유니티 에디터가 실행 중이고 Unity MCP가 연결되어 있다면, 에디터 콘솔 로그(`read_console`), 씬 인스펙션(`find_gameobjects`), 플레이모드 제어를 최우선으로 사용합니다.
> 2. **2순위 (대체 / 폴백)**: 에디터가 꺼져 있거나 Unity MCP를 찾을 수 없는 경우, 작업을 블로킹하지 않고 본 스킬의 **공식 `unity` CLI 명령어**로 백그라운드 무인 컴파일 검증 및 NUnit 테스트를 자동 대체 수행합니다.

---

## 1. 전담 직무 영역 (Core Scope)
- **`GEMINI_SETUP` 초기 환경 설정**:
  - 시스템 전역에 설치된 공식 Unity CLI(`unity`)를 활용하여 버전 조회, 템플릿 탐색 및 프로젝트 개설을 수행합니다.
- **실무 개발/검수 무인 대체 (Fallback Verification)**:
  - 에디터 미기동 환경에서 백그라운드 무인 컴파일 에러 0건 검증 및 EditMode/PlayMode NUnit 테스트 무인 실행을 전담합니다.

---

## 2. 공식 Unity CLI (`unity`) 표준 명령어 목록

### 1) 프로젝트 셋업 및 환경 탐색
```bash
# 1. 시스템에 설치된 모든 Unity 에디터 버전 및 경로 목록 즉시 조회
unity editors

# 2. 지원되는 프로젝트 템플릿 목록 조회 (예: 6000.5.8f1)
unity templates list -e 6000.5.8f1

# 3. 프로젝트 신규 생성 및 Hub 등록 (공식 커맨드)
unity projects new [프로젝트명] --editor-version [버전] --template [템플릿ID]

# 4. 저장소 최상단(루트) 직접 초기화 (프로젝트 디렉토리가 이미 존재하는 경우)
unity run . -- -createProject . -quit
```

### 2) 실무 개발/검수 무인 검증 (Unity MCP 부재 시 대체 실행)
```bash
# 1. 무인 백그라운드 컴파일 무결성 검증 (Developer - 에러 시 Exit Code 1)
unity run . -- -quit -batchmode

# 2. EditMode NUnit 단위 테스트 무인 실행 (QA - 결과 XML 자동 생성)
unity test --platform EditMode

# 3. PlayMode NUnit 통합 테스트 무인 실행
unity test --platform PlayMode

# 4. 재구축 가능한 캐시 폴더(Library, Temp, Logs) 안전 청소
unity projects clean
```

---

## 3. 동작 절차 및 안전 게이트 (Safety Gates)

1. **설치 여부 우선 판별 (Bypass Safety Gate)**:
   - 프로젝트 최상단에 `ProjectSettings/ProjectVersion.txt`가 이미 존재하는 경우, 기존 프로젝트 환경을 보존하기 위해 신규 생성을 건너뜁니다.
2. **Unity 버전 일치 검증 (Version Check Gate)**:
   - `docs/PROJECT_SPEC.md`에 명시된 `Unity Version`이 `unity editors` 목록에 존재하는지 확인합니다.
   - 일치하는 에디터가 없으면 Fast-Fail 에러를 출력하고 Unity Hub 설치를 안내합니다.
3. **무인 검증 무결성 게이트 (Proof-of-Verification Gate)**:
   - Unity MCP가 없을 때 CLI 무인 실행 시, 반환되는 Exit Code 0 및 테스트 100% Pass를 확인한 후 작업을 인계합니다.
