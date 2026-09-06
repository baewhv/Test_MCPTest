# Git 버전 관리 및 Clean PR 운영 규칙 (Git Rules)

이 문서는 프로젝트의 모든 버전 관리, 브랜치 생명주기, Clean PR 컨벤션 및 Post-Merge 문서 동기화 원칙을 규정하는 Git 표준 규칙입니다.

---

## 1. 3단계 브랜치 운영 구조 (Branch Strategy)
- **`main`**: 최종 릴리즈 및 안정화 배포 브랜치 (사용자 최종 머지 전용)
- **`develop`**: 전체 기능 통합 및 최신 문서(DOCS) 유지 브랜치
- **`작업 브랜치`**: 실제 기능 개발 및 버그 수정을 격리 수행하는 로컬 브랜치 (`feat/[기능명]`, `fix/[버그명]`, `refactor/[개선명]`)

---

## 2. 5단계 Clean PR 원칙 (Clean PR Policy)
1. **코드 순수성 유지 (Assets Only)**:
   - 작업 브랜치(`feat/...`)에는 오직 실제 게임 소스 코드, 프리팹, 테스트 코드(`Assets/`)만 선별 커밋(`git add Assets/`)하여 PR을 생성합니다.
   - `docs/` 폴더의 문서(협업 로그, 상태판, 구현 문서 등)는 작업 브랜치에 커밋하지 않고 로컬에 격리 보존합니다 (PR diff 잡음 0건 보장).
2. **GitManager 원격 푸시 & PR 발행**:
   - `Developer`가 `Assets/` 선별 커밋을 완료하면, `GitManager`가 원격으로 브랜치를 푸시하고 develop 대상 Clean PR을 발행합니다.
3. **PR 머지 권한 절대 원칙 (No Auto-Merge)**:
   - 모든 PR의 최종 머지는 오직 **사용자**가 GitHub UI에서 직접 수행합니다.
   - 어떠한 에이전트도 PR을 직접 머지(`merge_pull_request`)하거나 임의로 닫지 않습니다.
4. **Post-Merge 일괄 문서 동기화 (`git-doc-sync`)**:
   - 사용자가 PR을 머지한 후 다음 지시 시, PM이 `develop`을 최신 pull하고 로컬에 누적된 `docs/` 문서를 `develop`에 일괄 커밋/푸시하여 1루프를 최종 완결합니다.

---

## 3. 커밋 메시지 컨벤션 (Commit Convention)
- **형식**: `[타입] : 메시지 내용`
- **8대 허용 타입**:
  - `feat`: 새로운 기능 추가 (Developer)
  - `test`: 단위/통합 테스트 코드 추가 (QA)
  - `fix`: 버그 및 오류 수정 (Developer)
  - `refactor`: 코드 리팩토링 (Developer)
  - `docs`: 기획/구현 문서 및 상태판 갱신 (Post-Merge 시 PM/GitManager)
  - `chore`: 설정 및 빌드 환경 변경
  - `build`: 패키지 매니저 및 종속성 변경
  - `style`: 코드 포맷팅 (로직 변경 없음)
- 메시지 내용은 한 줄로 명확하게 요약 작성합니다.
- 특정 이슈와 연관된 경우 끝에 `(PR #nn)` 또는 `#이슈번호`를 첨부합니다.

---

## 4. Pull Request(PR) 컨벤션
- **타이틀 형식**: `[타입] [작업 내용 요약] - [담당 에이전트명]`
  - 예시: `[feat] 플레이어 기본 이동 및 입력 처리 구현 - Developer`
- **대상 베이스 브랜치**: 반드시 `develop` 브랜치를 베이스로 생성합니다.
- **본문 구성**:
  - 작업 개요 및 목적
  - 주요 구현 및 변경 내역 (Assets/ 파일 목록)
  - QA 검수 요청 항목 (NUnit 테스트 및 4대 런타임 검수)

---

## 5. .meta 파일 무결성 보존 (Unity Meta Integrity)
- Assets 폴더 내의 모든 파일/디렉토리 변경 시, 반드시 대응하는 `.meta` 파일이 1:1 쌍으로 존재하는지 검증 후 커밋합니다.

---

## 6. 에이전트별 Git 역할 분담 (Role Boundaries)
- **`Developer`**: 로컬 브랜치 확인(`git branch --show-current`) 및 구현 완료 후 **`Assets/` 선별 커밋(`git add Assets/`)만 직접 수행** (브랜치 분리, 원격 푸시, PR 생성 일체 관여 금지).
- **`QA`**: 변경 파일 타겟 NUnit 테스트 작성 후 **`Assets/Tests/` 선별 커밋 및 PR Approve 리뷰 등록만 수행** (PR 머지/푸시 금지).
- **`GitManager`**: 로컬 브랜치 생성/전환(`git-branch-setup`), 작업 브랜치 원격 푸시 & Clean PR 발행(`git-pr-workflow`), Post-Merge 문서 동기화(`git-doc-sync`) 독점 전담.
- **`PM`**: 물리적 교차 검증 게이트(Double-Check Gate)를 통해 브랜치 전환 및 워킹 트리 상태 최종 검증 후 사용자에게 알림.

