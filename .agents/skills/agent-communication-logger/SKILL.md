---
name: agent-communication-logger
description: 에이전트 간의 실시간 인계(Handoff), 일감 위임, 결과 반환, PR 요청, QA 검수 요청 과정을 당일 타임라인 로그(docs/logs/agent_comm_YYYY-MM-DD.md)에 자동 누적 기록하는 표준 로깅 스킬
---

# Agent Communication Logger Skill

에이전트들이 서로 소통하고 작업을 인계할 때 실시간으로 기록을 남기는 표준 도구입니다.

## 1. CLI 실행 명령어

```bash
node .agents/skills/agent-communication-logger/scripts/log_comm.js --from <발신자> --to <수신자> --type <소통유형> --msg "<전달내용요약>"
```

## 2. 매개변수 (Parameters)
- `--from` (필수): 발신 에이전트명 (예: `Designer`, `Developer`, `UnityBuilder`, `GitManager`, `UnityDebugger`)
- `--to` (필수): 수신 대상명 (예: `Developer`, `code_worker`, `unity_builder`, `git_manager`, `GitHub PR #1`)
- `--type` (필수): 소통 유형 (예: `기획 인계`, `코드 연산 위임`, `결과 반환`, `에셋 조립 인계`, `PR 요청`, `QA 검증 요청`, `QA 리뷰 댓글`)
- `--msg` (필수): 전달하는 핵심 내용 및 변경점 요약
