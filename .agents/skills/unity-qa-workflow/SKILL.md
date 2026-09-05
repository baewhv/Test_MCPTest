---
name: unity-qa-workflow
description: QA 에이전트가 기술 명세서를 바탕으로 Assets/Tests/에 NUnit 단위/통합 테스트 코드를 직접 작성하고, 4대 필수 런타임 검수, 스크린샷 캡처 및 PR 승인을 완결하는 표준 검수 워크플로우 스킬입니다.
---

# Unity QA 4대 검수 및 NUnit 테스트 작성 워크플로우

이 스킬은 QA 에이전트가 개발 완료된 기능에 대해 NUnit 단위/통합 테스트 코드를 직접 작성하고, 4대 필수 검수를 거쳐 승인(Pass) 또는 반려(Fail)를 완결하는 표준 절차를 정의합니다.

---

## 1. QA 4대 필수 검수 워크플로우

### [사전 준비: 상태 명시 및 소통 로깅]
1. `docs/work/status.md`의 `[현재 상태]`를 `[QA] [기능명] QA 4대 검수 진행 중 (NUnit 테스트 작성/검증, 콘솔, 코어루프, 스크린샷)`으로 갱신합니다.
2. 아래 명령으로 로깅을 기록합니다:
   ```bash
   node .agents/skills/agent-communication-logger/scripts/log_comm.js --from "QA" --to "QA" --type "검수 착수" --msg "[기능명] QA 4대 검수 절차 착수"
   ```

### [1단계: 명세 기반 NUnit 테스트 코드 직접 작성 및 100% Pass 검증]
1. **명세 기반 테스트 코드 작성**: `docs/tech_spec/` 및 `docs/implementations/`를 기준으로 `Assets/Tests/Editor/[클래스명]Tests.cs` (단위/로직) 또는 `Assets/Tests/Runtime/[클래스명]Tests.cs` (통합/수명주기)를 작성합니다.
   - (※ 에디터 소켓 점유 프리징을 방지하기 위해 `write_to_file` 도구를 사용합니다.)
2. **테스트 무인 자동 실행**: 백그라운드 CLI 러너로 단위/통합 테스트를 일괄 실행하여 100% Pass를 검증합니다:
   ```bash
   node .agents/skills/unity-cli-runner/scripts/unity_cli.js test EditMode
   node .agents/skills/unity-cli-runner/scripts/unity_cli.js test PlayMode
   ```

### [2단계: 콘솔 에러 0건, Zero-Override 및 컨벤션 검증]
1. `read_console` 또는 `unity_cli.js compile`을 실행하여 에러가 **0건**인지 확인합니다.
2. 프리팹 인스턴스에 로컬 오버라이드가 없는지(Zero-Override), `docs/ARCHITECTURE.md`에 관계도가 등록되었는지 검증합니다.

### [3단계: 코어루프 런타임 정상 실행 검증]
`manage_editor` (action: "play") 또는 `execute_code`를 사용하여 에디터 실행 상태에서 게임의 코어 루프가 기획 명세대로 결함 없이 구동되는지 검증합니다.

### [4단계: 기능 구현 검증 스크린샷 촬영]
`manage_camera` (action: "screenshot", capture_source: "game_view", output_folder: "Assets/Screenshots")를 호출하여 동작 중인 화면을 캡처합니다.

---

## 2. 검수 결과 처리 및 승인/반려 절차

### ① 4대 검수 모두 통과(Pass) 시:
1. `docs/work/worklist.md`에서 해당 태스크를 `- [ ]`에서 **`- [x] [태스크명] (PR #nn)`**으로 갱신합니다.
2. GitHub MCP `add_issue_comment` 도구로 PR에 4대 검증 통과 내역(NUnit 통과, 콘솔 에러 0건, 코어루프 정상 구동, 캡처된 스크린샷 경로) 승인 코멘트를 작성합니다.
3. `docs/work/status.md`를 `[QA] [기능명] QA 4대 검수 통과 및 worklist [x] 완료 ➔ 사용자 최종 Merge 대기`로 갱신합니다.
4. GitManager에게 직접 인계하고 턴을 마칩니다:
   ```bash
   node .agents/skills/agent-communication-logger/scripts/log_comm.js --from "QA" --to "GitManager" --type "QA 승인" --msg "[기능명] QA 4대 검수 통과 및 worklist [x] 완료, 머지 대기"
   ```

### ② 이상/결함 발견(Fail) 시:
1. 등록된 PR에 실패 원인 코멘트를 작성합니다.
2. `docs/work/status.md`를 `[QA] [기능명] QA 검수 반려 (결함 발견) ➔ developer에게 수정 요청 인계`로 갱신합니다.
3. Developer에게 수정을 직접 요청하고 턴을 마칩니다:
   ```bash
   node .agents/skills/agent-communication-logger/scripts/log_comm.js --from "QA" --to "Developer" --type "QA 반려/수정요청" --msg "[기능명] 결함 발견으로 수정 요청"
   ```
