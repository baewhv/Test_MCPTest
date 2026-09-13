---
name: git-pr-workflow
description: 단일 지속 PR(Living Epic PR)을 최초 1회 발행하거나 기존 PR 상태를 확인하고, 작업 내역을 누적 관리하는 표준 PR 워크플로우 스킬입니다.
---

# 단일 지속 PR(Living Epic PR) 운용 및 인계 워크플로우

이 스킬은 GitManager가 Phase/에픽 단위로 **단 하나의 지속적인 PR(Single Living PR)**을 개설하고, 서브 태스크들이 동일한 PR에 커밋과 댓글(Comment)로 안전하게 누적되도록 통제하는 절차를 정의합니다.
**머지 절대 원칙**: 에이전트는 절대로 PR을 직접 머지하지 않으며, 모든 머지는 **사용자**가 원하는 시점에 GitHub UI에서 직접 수동으로 수행합니다.

---

## 1. 단일 지속 PR 운용 4단계 절차

### [1단계: 기존 활성 PR 존재 여부 확인]
1. `docs/work/status.md` 또는 GitHub MCP `list_pull_requests`로 현재 작업 브랜치(`feat/...`)에 열려 있는 PR이 이미 존재하는지 확인합니다.
2. **이미 PR이 열려 있는 경우**:
   - PR을 중복 생성하지 않고 기존 PR 번호(#nn)를 유지합니다.
   - 즉시 4단계(상태판 갱신 및 인계)로 직행합니다.

### [2단계: 신규 Phase/에픽용 초기 PR 최초 1회 발행]
현재 작업 브랜치에 열려 있는 PR이 없을 때만 최초 1회 생성합니다:
1. **작업 브랜치 원격 푸시 확인**:
   ```bash
   git push origin HEAD
   ```
2. **GitHub Pull Request 생성 (`develop` 대상)**:
   - GitHub MCP `create_pull_request`를 호출하여 지속 관리용 PR을 생성합니다:
     - **Title**: `[Epic/Phase] : [기능/마일스톤명]`
     - **Head**: `[작업브랜치명]`
     - **Base**: `develop`
     - **Body**: 해당 Phase/에픽의 목표 및 하위 태스크 체크리스트 기재 (이후 하위 태스크 진행 시 댓글로 상세 내역이 누적됨)

### [3단계: PR 상태판 동기화]
- `docs/work/status.md`에 현재 활성 PR 번호(PR #nn) 및 작업 브랜치명을 명시합니다.

### [4단계: QA/Developer 인계 및 소통 로깅]
1. 작업자들에게 현재 활성 PR 번호(#nn)를 인계하여, 작업 완료 시 해당 PR에 댓글(Comment)을 작성할 수 있도록 합니다.
2. 소통 로거를 실행합니다:
   ```bash
   node .agents/skills/agent-communication-logger/scripts/log_comm.js --from "GitManager" --to "PM" --type "PR 준비 완료" --msg "활성 PR #nn 유지 및 작업 준비 완료"
   ```

