# 프로젝트 초기 환경 설정 및 셋업 규칙 (Project Rules - Setup Mode)

> [!IMPORTANT]
> 본 프로젝트는 현재 **초기 환경 설정 단계(Setup Phase)**에 있습니다.
> `docs/PROJECT_SPEC.md`의 환경 설정이 완료되고 상태가 `[SETUP_COMPLETED]`로 갱신되기 전까지, 모든 에이전트는 게임 코드 개발이나 작업 브랜치 생성을 진행하지 않고 환경 구성과 기획서 등록 지원에만 전념합니다.

---

## 0. 프로젝트 환경 설정 상태 (Setup Status)
- **상태**: `[SETUP_IN_PROGRESS]`

---

## 1. 초기 셋업 단계 필수 행동 수칙 (Setup Discipline)

1. **조기 개발 착수 절대 금지 (No Early Coding Gate)**:
   - 환경 설정이 완료되기 전에는 어떠한 에이전트도 `Assets/` 소스코드를 작성하거나, `feat/...` 작업 브랜치를 생성할 수 없습니다.
2. **필수 환경 정보 기입 (PROJECT_SPEC.md)**:
   - `docs/PROJECT_SPEC.md`의 필수 항목을 사용자와 함께 점검 및 기입합니다:
     - GitHub Repository URL 및 기본 브랜치(`develop`, `main`)
     - 새 PC의 Unity Editor 실행 경로(`Unity Editor Path`)
     - 대상 플랫폼 및 프로젝트 기본 사양
3. **사용자 기획서 원본 등록 안내 (Strict Read-Only Specs)**:
   - 사용자가 구상한 게임 시스템/기능 기획서 원본을 `docs/specs/[기획서명].md` 경로에 배치하도록 안내합니다.
   - `docs/specs/` 하위 파일은 **엄격한 읽기 전용(Strict Read-Only)**으로 취급하며, 에이전트는 절대 임의 수정/덮어쓰기를 하지 않습니다.

---

## 2. 개발 운영 헌장 전환 및 에셋 자동 배포 규칙 (Deploy & Transition Trigger)

- **전환 조건**:
  1. `docs/PROJECT_SPEC.md`의 필수 정보가 모두 기입 완료됨.
  2. `docs/PROJECT_SPEC.md`의 환경 설정 상태가 `[SETUP_COMPLETED]`로 갱신됨.
- **전환 및 배포 2단계 실행 절차**:
  1. **실전 운영 에셋 일괄 배포 (Deploy Operating Pack)**:
     - PM 에이전트는 격리 보관되어 있던 실전 개발 에셋들을 `.agents/` 정규 경로로 일괄 복사 배포합니다:
       ```powershell
       Copy-Item -Path ".agents/templates/operating_pack/*" -Destination ".agents" -Recurse -Force
       ```
     - 이로써 6대 전문 에이전트(`pm`, `designer`, `developer`, `qa` 등) 및 18대 실전 스킬이 정식 활성화됩니다.
  2. **운영 헌장 교체 (Swap to Operating Rules)**:
     - 프로젝트 루트의 `GEMINI.md`를 아래 파일의 내용으로 완전히 덮어씁니다:
       - **소스 파일**: `.agents/templates/GEMINI_OPERATING.md`
       - **대상 파일**: 루트 디렉토리의 `GEMINI.md`
  3. 덮어쓰기 완료 즉시 시스템은 **6대 전문 에이전트 표준 5단계 개발 라이프사이클(운영 모드)**로 공식 전환됩니다.
  4. PM은 사용자에게 "초기 환경 셋업 완료, 운영 에셋 배포 완료 및 정규 개발 라이프사이클 전환"을 공식 보고합니다.

---

## 3. 에이전트 4대 안전 헌장 (Always-On)
1. **도구 우회 사용 전면 금지**: `run_command`, `write_to_file`, `replace_file_content` 외 우회 도구(`unityMCP/execute_code` 등) 사용 금지.
2. **직무 영역 엄수 및 Fast-Fail**: 필요 도구 부재 또는 직무 외 작업 지시 시 즉시 0-Tool-Call로 중단하고 보고.
3. **100-Step 회로 차단**: 단일 턴 100회 초과 시 즉시 작업 중단 및 보고.
4. **서브 에이전트 실물 호출 위임**: PM의 1인 다역 금지, `invoke_subagent` 실물 호출 의무화.
