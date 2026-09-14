---
name: git-doc-sync
description: 사용자가 GitHub UI에서 Living PR 머지를 완료한 후(Post-Merge), develop 브랜치를 최신화(Pull)하고 로컬에 누적된 작업 문서(docs/)를 일괄 커밋/푸시하여 마일스톤/에픽 사이클을 최종 완결하는 사후 문서 동기화 스킬입니다.
---

# 작업 문서 사후 동기화 (Post-Merge Doc Sync) 워크플로우

이 스킬은 사용자가 GitHub UI에서 Living PR의 수동 머지(Merge)를 완료한 후, GitManager가 `develop` 브랜치를 최신화하고 해당 Phase/에픽 동안 로컬에 안전하게 누적 보존되었던 작업 문서(`docs/`, `status.md`, `worklist.md`, `implementations/`, `tech_spec/` 등)를 `develop` 브랜치에 일괄 커밋/푸시하여 마일스톤/에픽 1사이클을 공식 완결하는 표준 절차를 정의합니다.

> [!IMPORTANT]
> **Post-Merge 실행 원칙**:
> - 본 스킬은 사용자가 PR을 머지하기 전에 사전 실행되지 않습니다.
> - 반드시 사용자가 GitHub UI에서 PR 머지를 완료했음을 확인(또는 사용자로부터 머지 완료 통보를 수신)한 후 실행하여 `develop` 브랜치의 형상 역전 및 문서 오염을 원천 차단합니다.

---

## 1. Post-Merge 문서 동기화 4단계 절차

### [1단계: 사용자 PR 머지 확인 및 develop 최신화 (Pull)]
1. 사용자의 PR 수동 머지 완료 여부를 확인합니다.
2. `develop` 브랜치로 전환하고 머지된 최신 커밋을 로컬로 가져옵니다:
   ```bash
   git checkout develop
   git pull origin develop
   ```

### [2단계: 누적 작업 문서 일괄 스테이징]
로컬에 누적 작성되었던 상태판, 체크리스트, 기획/구현 기술문서, 소통 로그를 스테이징합니다:
```bash
git add docs/
```

### [3단계: 문서 최종 커밋 및 원격 푸시]
```bash
git commit -m "[docs] : [Phase/기능명] 작업 완료 문서 및 상태판 최종 동기화"
git push origin develop
```

### [4단계: 클린 워킹 트리 검증 및 마일스톤 완결 보고]
1. `git status`로 깨끗한 Working Tree 상태를 확인합니다:
   ```bash
   git status
   ```
2. PM은 사용자에게 "마일스톤 Post-Merge 문서 동기화 완료 및 1사이클 공식 완결"을 보고하고, 다음 Phase 준비 상태로 진입합니다.
