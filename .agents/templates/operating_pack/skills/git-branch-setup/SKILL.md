---
name: git-branch-setup
description: develop 브랜치를 최신 상태로 패치/동기화하고 로컬 터미널에서 작업 브랜치를 분리/확인하여 작업 환경을 할당하는 Git 브랜치 준비 스킬입니다.
---

# Git 작업 브랜치 분리 및 환경 할당 워크플로우

이 스킬은 GitManager가 Developer 또는 PM의 작업 착수 요청을 수신했을 때, 작업 환경을 안전하게 할당하는 절차를 정의합니다.
**절대 원칙**: `develop` 브랜치에서는 직접 작업이 엄격히 금지되므로 모든 작업은 반드시 독립된 작업 브랜치(`feat/...` 등)에서 수행됩니다.

---

## 1. 작업 브랜치 준비 3단계 절차

### [1단계: 기존 활성 브랜치/PR 재사용 여부 확인 (단일 지속 PR 원칙)]
1. 현재 진행 중인 Phase/에픽의 작업 브랜치가 이미 존재하고 GitHub PR이 열려 있는 상태라면:
   - 브랜치를 새로 쪼개지 않고, 기존 작업 브랜치로 체크아웃하여 해당 브랜치를 유지합니다:
     ```bash
     git checkout feat/[기능명]
     git branch --show-current
     ```
   - 이 경우 즉시 3단계(작업자 인계)로 직행합니다.

### [2단계: 신규 Phase/에픽용 작업 브랜치 분리 (최초 1회 또는 머지 후)]
새로운 Phase/에픽이 시작되거나 이전 PR이 사용자 머지 완료된 경우에만 최신 `develop`에서 신규 분리합니다:
1. **로컬 develop 브랜치 최신화**:
   ```bash
   git checkout develop
   git fetch origin develop
   git pull origin develop
   ```
2. **로컬 신규 작업 브랜치 분리 및 체크아웃**:
   ```bash
   git checkout -b feat/[기능명] develop
   ```
3. **원격 저장소 즉시 브랜치 발행 (Publish Branch)**:
   ```bash
   git push -u origin feat/[기능명]
   ```
4. **물리적 브랜치 전환 자가 검증 (필수)**:
   ```bash
   git branch --show-current
   ```
   - *검증 게이트: 터미널 출력 결과가 지정된 `feat/[기능명]`과 100% 일치하는지 확인합니다.*

### [3단계: 작업자 전환 안내 및 소통 로깅]
1. 작업자(Developer 등)에게 브랜치 준비 완료를 인계합니다:
   ```bash
   node .agents/skills/agent-communication-logger/scripts/log_comm.js --from "GitManager" --to "Developer" --type "브랜치 준비" --msg "feat/[기능명] 작업 브랜치 검증 완료, 개발 착수 가능"
   ```
2. PM에게 실제 전환된 브랜치명(`feat/[기능명]`)과 함께 결과를 보고하고 턴을 종료합니다.


