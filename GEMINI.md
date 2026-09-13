# 프로젝트 최상위 진입 및 레포지토리 식별 헌장 (Project Entry Gate - GEMINI_FIRST)

> [!IMPORTANT]
> 본 파일은 프로젝트 구동 시 가장 먼저 평가되는 최상위 게이트웨이(Gateway) 규칙입니다.
> 에이전트는 세션 시작 즉시 현재 레포지토리 명칭을 확인하고, 결과에 따라 적절한 작업 모드로 분기하거나 헌장을 자동 전환해야 합니다.

---

## 1. 레포지토리 명칭 판별 및 2대 분기 규칙 (Repository Identification Gate)

에이전트는 터미널 명령(`git remote get-url origin` 또는 프로젝트 루트 디렉토리명)을 통해 현재 레포지토리 명칭을 식별한 뒤, 아래 2가지 규칙 중 하나를 엄격히 적용합니다:

### [조건 1: 레포지토리 명칭이 `Test_MCPTest` (또는 `TestMCP`)인 경우]
- **판정**: **에이전트 거버넌스 및 템플릿 마스터 저장소 (Agent Configuration & Template Master)**
- **행동 수칙**:
  1. 본 저장소는 6대 전문 에이전트 체계와 룰, 스킬, 도구를 설계하고 검증하는 마스터 템플릿입니다.
  2. 실제 게임 플레이 로직 개발이 아닌, **에이전트 구성 설정(프롬프트, 룰, 스킬, 거버넌스 문서 및 자동화 스크립트)의 제작, 보수, 최적화 작업만을 전담**하여 진행합니다.
  3. 실전 운영용 에셋 원본은 `.agents/templates/operating_pack/` 및 `.agents/`에 일치하도록 정합성을 유지합니다.
  4. 다른 헌장으로 임의 전환하지 않고, 본 헌장 및 `.agents/templates/` 하위 템플릿의 정밀도를 유지합니다.

---

### [조건 2: 레포지토리 명칭이 다른 명칭인 경우 (신규 프로젝트로 복제된 경우)]
- **판정**: **신규 게임 개발 프로젝트 (New Game Development Project)**
- **행동 수칙 (즉시 GEMINI_SETUP 전환 및 운영 에셋 격리)**:
  1. **초기 셋업 헌장 교체**: 에이전트는 즉시 프로젝트 루트의 `GEMINI.md`를 아래 파일의 내용으로 완전히 덮어씁니다:
     - **소스 파일**: `.agents/templates/GEMINI_SETUP.md`
     - **대상 파일**: 프로젝트 루트의 `GEMINI.md`
  2. **운영 에셋 격리(Clean Isolation)**:
     - 실전 운영용 에셋(`agents/`, `skills/`)은 `.agents/templates/operating_pack/`에 안전하게 보관되어 있습니다.
     - 셋업 단계에서 불필요한 스킬/에이전트의 자동 로드(Auto-Discovery) 및 토큰 소모를 방지하기 위해, 루트의 `.agents/agents/` 및 `.agents/skills/` 폴더가 존재한다면 정리(격리 보관 상태 유지)합니다.
  3. 덮어쓰기 및 정리가 완료되면 시스템은 **신규 프로젝트 초기 환경 설정 모드(Setup Mode)**로 진입합니다.
  4. 에이전트는 사용자에게 다음과 같이 보고하고 초기 셋업을 시작합니다:
     - *"신규 게임 개발 프로젝트가 감지되었습니다. 초기 환경 셋업 헌장(GEMINI_SETUP)으로 자동 전환을 완료했습니다. docs/PROJECT_SPEC.md 설정을 시작합니다."*

---

## 2. 에이전트 4대 안전 헌장 (Always-On)
1. **도구 우회 사용 전면 금지**: `run_command`, `write_to_file`, `replace_file_content` 외 우회 도구(`unityMCP/execute_code` 등) 사용 금지.
2. **직무 영역 엄수 및 Fast-Fail**: 필요 도구 부재 또는 직무 외 작업 지시 시 즉시 0-Tool-Call로 중단하고 보고.
3. **100-Step 회로 차단**: 단일 턴 100회 초과 시 즉시 작업 중단 및 보고.
4. **서브 에이전트 실물 호출 위임**: PM의 1인 다역 금지, `invoke_subagent` 실물 호출 의무화.
