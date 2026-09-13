---
name: unity-cli-runner
description: GEMINI_SETUP 초기 프로젝트 셋업 전용 공식 Unity CLI (`unity`) 도구 (실무 에이전트의 일상 개발/검수 작업 중 호출 일체 금지)
---

# Unity CLI Runner Skill (Setup Only)

> [!IMPORTANT]
> **실무 에이전트 호출 전면 금지 (Prohibited in Regular Workflows)**:
> Unity CLI는 일반 기능 개발 및 검수(`Developer`, `QA`, `PM`) 단계에서 호출이 전면 제외/금지됩니다.
> 실무 개발 및 런타임/콘솔 검수는 실행 중인 에디터와 직접 통신하는 `Unity MCP`를 전담으로 사용합니다.

---

## 1. 전담 용도 (Intended Scope)
- **`GEMINI_SETUP` 초기 환경 설정 전용**:
  - 시스템 전역에 설치된 공식 Unity CLI(`unity.exe`)를 활용하여, Node.js 경유 없이 터미널에서 순수 `unity` 명령어로 Unity 에디터 버전 조회, 템플릿 탐색 및 프로젝트 개설을 수행합니다.
  - 신규 프로젝트 복제 후 로컬에 Unity Project(`ProjectSettings/ProjectVersion.txt`)가 설치되지 않은 경우 무인 자동 생성을 지원합니다.

---

## 2. 공식 Unity CLI (`unity`) 표준 명령어

```bash
# 1. 시스템에 설치된 모든 Unity 에디터 버전 및 경로 목록 즉시 조회
unity editors

# 2. 지원되는 프로젝트 템플릿 목록 조회 (예: 6000.5.8f1)
unity templates list -e 6000.5.8f1

# 3. 프로젝트 신규 생성 및 Hub 등록 (공식 커맨드)
unity projects new [프로젝트명] --editor-version [버전] --template [템플릿ID]
# 예시:
unity projects new TestMCP --editor-version 6000.5.8f1 --template com.unity.template.universal-2d

# 4. 저장소 최상단(루트) 직접 초기화 (프로젝트 경로가 이미 존재하는 경우)
unity run . -- -createProject . -quit
```

---

## 3. 동작 절차 및 안전 게이트 (Safety Gates)

1. **설치 여부 우선 판별 (Bypass Safety Gate)**:
   - 프로젝트 최상단에 `ProjectSettings/ProjectVersion.txt`가 이미 존재하는 경우, 기존 프로젝트 환경을 보존하기 위해 신규 생성을 건너뜁니다.
2. **Unity 버전 일치 검증 (Version Check Gate)**:
   - `docs/PROJECT_SPEC.md`에 명시된 `Unity Version`이 `unity editors` 출력 목록에 존재하는지 확인합니다.
   - 일치하는 에디터가 없으면 Fast-Fail 에러를 출력하고 Unity Hub 설치를 안내합니다.
3. **최상단 프로젝트 개설 (Create Project Gate)**:
   - 공식 `unity` CLI를 통해 지정된 버전 및 템플릿(2D / 3D URP 등)으로 프로젝트 환경을 자동 개설합니다.
