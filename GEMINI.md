# 프로젝트 에이전트 협업 및 운영 규칙 (Project Rules)

> [!NOTE]
> 언어/커뮤니케이션, 보안/마스킹, 코드 품질 및 문서화(.md) 아티팩트 생성 규칙은 전역 규칙(`Global Rules`)을 따릅니다.

---

## 1. 사용자 작업 지시 및 PM 위임 원칙 (PM Delegation Rule)
- 메인(Default) 에이전트는 사용자로부터 작업 실행 지시("기획서 분석해줘", "작업 하나 진행해줘", "N개 작업 진행해줘", "리팩토링해줘", "현재 상태" 등)를 수신하면, 직접 코딩이나 검수를 수행하지 않고 **`invoke_subagent` 도구를 호출하여 `PM` 에이전트에게 지시를 위임**한다.
- `PM` 에이전트는 전체 파이프라인을 총괄 지휘하며 5대 전문 서브에이전트(`designer`, `artist`, `developer`, `qa`, `git_manager`)를 오케스트레이션하여 작업 1루프를 완결한다.

---

## 2. 서브에이전트 작업 보고 및 턴 종료 규칙 (Subagent Reporting & Turn Completion)
- 모든 서브에이전트(Subagent)는 할당받은 작업이 **완료되거나 중단(도구 차단, 결함 발견, 기획 보완 대기 등)되면, 그 결과 내용 및 사유를 `PM`에게 명확히 보고하고 도구 호출을 중단하여 턴을 마친다.**
- 서브에이전트는 임의로 다음 단계의 서브에이전트를 직접 연쇄 호출하지 않고, PM에게 결과를 반환하여 PM이 중앙 집중식으로 다음 파이프라인을 제어하도록 한다.

---

## 3. 에이전트 실시간 소통 기록 규칙 (Communication Logger - 사용자 실시간 모니터링/검증용)
- **목적**: 사용자가 5대 에이전트의 실제 협업 흐름, 데이터 인계, PR 생성, QA 검증 과정을 시간대별로 감사(Audit) 및 검증.
- **관리 파일**: `docs/logs/agent_comm_YYYY-MM-DD.md`
- 에이전트 간 인계(Handoff), 일감 위임, 결과 반환, PR 요청, QA 검증 요청 및 결과 반환이 일어날 때마다 `agent-communication-logger` 스킬을 사용하여 실시간 소통 로그를 **1줄씩 누적 기록**한다:
  ```bash
  node .agents/skills/agent-communication-logger/scripts/log_comm.js --from <발신자> --to <수신자> --type <소통유형> --msg "<전달내용요약>"
  ```
