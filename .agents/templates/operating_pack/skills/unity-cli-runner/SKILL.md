---
name: unity-cli-runner
description: GEMINI_SETUP 초기 프로젝트 셋업 전용 Unity CLI 도구 (실무 에이전트의 일상 개발/검수 작업 중 호출 일체 금지)
---

# Unity CLI Runner Skill (Setup Only)

> [!IMPORTANT]
> **실무 에이전트 호출 전면 금지 (Prohibited in Regular Workflows)**:
> Unity CLI는 일반 기능 개발 및 검수(`Developer`, `QA`, `PM`) 단계에서 호출이 전면 제외/금지됩니다.
> 실무 개발 및 런타임/콘솔 검수는 실행 중인 에디터와 직접 통신하는 `Unity MCP`를 전담으로 사용합니다.

---

## 1. 전담 용도 (Intended Scope)
- **`GEMINI_SETUP` 초기 환경 설정 전용**:
  - 신규 프로젝트 복제 후 로컬에 Unity Project(`ProjectSettings/ProjectVersion.txt`)가 설치되지 않은 경우, `docs/PROJECT_SPEC.md`의 명세를 기반으로 프로젝트 최상단에 빈 유니티 프로젝트를 자동 생성(`-createProject`)합니다.
  - 워크스페이스에 명시된 `Unity Version`이 실제로 설치되어 있는지 자동 탐색 및 검증합니다.

---

## 2. CLI 실행 명령어

```bash
# 1. 시스템에 설치된 모든 Unity 에디터 버전 및 경로 목록 조회
node .agents/templates/operating_pack/skills/unity-cli-runner/scripts/unity_cli.js versions

# 2. 현재 Unity 프로젝트 설치 상태 및 docs/PROJECT_SPEC.md 버전 일치 진단
node .agents/templates/operating_pack/skills/unity-cli-runner/scripts/unity_cli.js check

# 3. 프로젝트 최상단에 신규 Unity 프로젝트 개설 및 설치 (-createProject)
node .agents/templates/operating_pack/skills/unity-cli-runner/scripts/unity_cli.js init
```

---

## 3. 동작 절차 및 안전 게이트 (Safety Gates)

1. **설치 여부 우선 판별 (Bypass Safety Gate)**:
   - 프로젝트 최상단에 `ProjectSettings/ProjectVersion.txt`가 이미 존재하는 경우, 기존 프로젝트 환경을 보존하기 위해 신규 생성을 건너뜁니다.
2. **Unity 버전 일치 검증 (Version Check Gate)**:
   - `docs/PROJECT_SPEC.md`에 명시된 `Unity Version`을 읽어 시스템 경로(`C:\Program Files\Unity\Hub\Editor\<Version>\Editor\Unity.exe`)에 해당 버전이 설치되어 있는지 확인합니다.
   - 일치하는 에디터가 없으면 Fast-Fail 에러를 출력하고 중단합니다.
3. **최상단 프로젝트 개설 (Create Project)**:
   - 검증된 Unity 실행 파일로 `Unity.exe -batchmode -createProject "<ProjectRoot>" -quit -logFile "<LogFile>"`를 무인 실행하여 프로젝트 최상단에 표준 Unity 프로젝트 구조를 자동 초기화합니다.
