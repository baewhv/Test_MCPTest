# 프로젝트 에이전트 협업 및 운영 규칙 (Project Rules - Operating Mode)

> [!NOTE]
> 언어/커뮤니케이션, 보안/마스킹, 코드 품질 및 문서화(.md) 아티팩트 생성 규칙은 전역 규칙(`Global Rules`)을 따릅니다.

---

## 0. 프로젝트 환경 설정 상태 (Setup Status)
- **상태**: `[SETUP_COMPLETED]`

---

## 1. 단일 지속 PR(Living Epic PR) 및 댓글 기반 감사(Comment Audit) 원칙
- **develop 직접 푸시 엄격 금지 (작업 브랜치 필수)**:
  - `develop` 브랜치에서는 어떠한 소스코드도 직접 커밋하거나 푸시할 수 없으며, 모든 작업은 반드시 독립된 작업 브랜치(`feat/...` 등)에서 수행됩니다.
- **단일 지속 PR 운용 원칙 (Single Living PR on Feature Branch)**:
  - 태스크마다 브랜치를 쪼개고 PR을 새로 발행/머지하지 않고, **Phase/에픽 단위로 단 하나의 지속 PR(Living PR)을 개설**하여 동일 브랜치/PR에 커밋을 계속 누적합니다.
- **댓글 기반 실시간 감사 추적 (Comment-driven Audit Trail)**:
  - `[Developer 구현 댓글]`: C# 구현 및 `Assets/` 선별 커밋/푸시 완료 즉시, 활성 PR에 `[구현 완료 - Task X-X]` 요약 댓글(컴파일 결과, 커밋해시, 구현 파일 목록)을 자동 등록합니다.
  - `[QA 검수 댓글]`: NUnit 테스트 작성/푸시 및 4대 무인 검수 통과 즉시, 활성 PR에 `[QA 검수 통과 - Task X-X]` 결과 댓글(100% Pass)을 자동 등록합니다.
  - `[PM 태스크 완결 및 연속 진행]`: QA 승인 댓글 확인 후 `worklist.md` 완료 체크 반영 후 다음 태스크로 연속 진행합니다.

---

## 2. 브랜치 보호 및 머지 통제 절대 규칙 (Branch Protection & User Manual Merge)
- **에이전트 PR 머지/닫기 절대 금지 (No Agent Merge)**:
  - QA, GitManager, PM을 포함한 모든 에이전트는 절대로 PR을 직접 머지(`merge_pull_request`)하거나 임의로 닫을 수 없습니다.
  - 활성 PR은 항상 열린 상태(`Open`)로 유지되며, 모든 PR의 최종 머지는 오직 **사용자**가 원하는 시점에 GitHub UI에서 직접 수동으로 수행합니다.


---

## 3. 에이전트 4대 행동 제어 및 안전 헌장 (Agent Safety & Anti-Loop Policy)
*이 규칙은 모든 서브 에이전트가 생성되는 즉시 시스템 프롬프트에 Always-On으로 강제 주입되는 최상위 절대 헌장입니다.*

1. **도구 우회 사용 전면 금지 (Strict Tool Discipline)**:
   - 파일 수정/생성은 오직 네이티브 파일 도구(`write_to_file`, `replace_file_content`)만, 터미널 실행은 표준 셸 도구(`run_command`)만 사용해야 합니다.
   - 권한이 없거나 도구가 결핍되었다고 해서 `unityMCP`의 `apply_text_edits`, `manage_script`, `get_sha`, `execute_code` 등을 악용하여 코드를 임의 수정하거나 터미널 프로세스를 우회 실행하는 행위를 엄격히 금지합니다.
2. **직무 영역 준수 및 권한 부재 시 즉시 반려 (Strict Role Boundaries & Fast-Fail)**:
   - 모든 서브 에이전트는 본인의 화이트리스트 직무 영역(`Core Scope`)만 수행합니다.
   - **Fast-Fail 절대 원칙**: 지시받은 작업을 수행할 정규 도구(`run_command`, `write_to_file` 등)가 없거나, 직무 범위를 벗어난 요청을 받았을 경우 **절대로 타 도구로 우회하거나 강행하지 말고, 그 즉시 작업을 전면 중단(0-Tool-Call Trigger)한 뒤 PM에게 반려 사유(필요 도구/적정 에이전트)를 보고**하십시오.
   - **QA 비즈니스 로직 수정 절대 금지**: QA는 `Assets/Scripts/` 코드를 단 한 줄도 직접 수정할 수 없으며, 결함 발견 시 즉시 `QA 반려 (5-C)` 처리하여 Developer에게 인계합니다.
3. **단일 작업 100-Step 초과 방지 회로 차단 (Loop Circuit Breaker)**:
   - 단일 태스크 턴에서 도구 호출/스텝 수가 100회를 초과할 경우, 무한 수정 루프를 즉시 중단(Circuit Break)하고 진행 상황, 장애 원인, 차단 사유를 명시하여 보고 후 추가 지시를 대기합니다.
4. **서브 에이전트 실물 도구 호출 위임 의무화 (Mandatory Subagent Invocation)**:
   - PM 및 메인 에이전트는 직접 코딩/브랜치 조작/검수를 1인 다역(Roleplay)으로 수행하지 않고, 반드시 `invoke_subagent` 도구를 실제로 호출하여 독립된 전문 에이전트에게 실행을 위임해야 합니다.

---

## 4. 읽기 전용 문서 위치 (Read-Only Specifications)

- 아래 경로의 문서는 사용자가 직접 작성한 원본 문서이므로, 모든 에이전트는 **수정 및 덮어쓰기가 절대 불가능하며 오직 읽기(Read-Only)**만 수행한다:

| 경로 (Path) | 설명 (Description) | 에이전트 접근 권한 |
| :--- | :--- | :--- |
| `docs/specs/` | 사용자가 등록한 게임 시스템/기능 기획서 원본 | **엄격한 읽기 전용 (Strict Read-Only)** |

---

## 5. 작업 문서 위치 (Working Documents)
- 아래 경로의 문서는 서브 에이전트가 개발/분석 과정에서 실시간으로 갱신하는 작업 파일입니다 (QA 승인 후 PM이 develop에 일괄 커밋):

| 경로 (Path) | 설명 (Description) | 에이전트 접근 권한 |
| :--- | :--- | :--- |
| `docs/PROJECT_SPEC.md` | 프로젝트 환경 사양 기입 문서 | 초기 설정을 위해 읽기/쓰기 가능 |
| `docs/FOLDER_STRUCTURE.md` | 유니티 표준 폴더 구조 및 에셋/프리팹 네이밍 색인 | 읽기 / 쓰기 가능 |
| `docs/ARCHITECTURE.md` | 프로젝트 아키텍처 지도 및 관계도 | 읽기 / 쓰기 가능 |
| `docs/logs/` | 에이전트 간 실시간 소통 기록 폴더 | 읽기 / 쓰기 가능 |
| `docs/work/worklist.md` | 서브 에이전트 작업 태스크 체크리스트 | 읽기 / 쓰기 가능 |
| `docs/work/status.md` | 서브 에이전트 현재 실시간 작업 상태판 | 읽기 / 쓰기 가능 |
| `docs/tech_spec/` | 서브 에이전트(Designer)가 작성한 기획 기술 명세서 폴더 | 읽기 / 쓰기 가능 |
| `docs/implementations/` | 서브 에이전트(Developer)가 작성한 개별 구현 기술문서 폴더 | 읽기 / 쓰기 가능 |

---

## 6. 기타 문서 위치 (Miscellaneous)

| 경로 (Path) | 설명 (Description) | 에이전트 접근 권한 |
| :--- | :--- | :--- |
| `docs/llm_architecture_feedback/` | 에이전트 구조 및 협업에 대한 피드백 폴더 | 읽기 / 쓰기 가능 |
