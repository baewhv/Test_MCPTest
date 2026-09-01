# Git 버전 관리 및 브랜치 운영 규칙 (Git & Branch Management Rule)

## 1. 대상 레포지토리 및 3단계 브랜치 계층 구조
- **허용 저장소**: `baewhv/Test_MCPTest` (https://github.com/baewhv/Test_MCPTest)
- **3단계 브랜치 구조**:
  1. **`main`**: 빌드 및 릴리즈 배포 전용 브랜치. DOCS 외에는 에이전트가 직접 수정/푸시하지 않습니다.
  2. **`develop`**: 개발 총괄 통합 브랜치. 메인 작업 디렉토리에 상시 고정되어 모든 기획/룰/지침의 최신 상태를 보존합니다.
  3. **`작업 브랜치`**: DOCS 외의 실제 개발 작업을 이행하는 브랜치. `develop`으로부터 분기하여 독립된 **Git Worktree**에서 격리 작업합니다.

## 2. Git Worktree 기반 작업 브랜치 격리 규칙 (Worktree Convention)
- **목적**: 브랜치 전환 시 발생하는 에이전트 지침서, 전역 룰(GEMINI.md), 기획 문서의 과거 롤백 및 유니티 에셋 리임포트 지연을 원천 차단합니다.
- **운영 구조**:
  - 메인 저장소 (`TestMCP`): `develop` 브랜치 상시 고정 (기획 문서, 에이전트 룰, 마스터 색인 최신 유지)
  - 격리 워크트리 (`../TestMCP_worktrees/[작업브랜치명]`): 개별 개발 작업 브랜치 체크아웃
- **생애주기 절차**:
  1. **작업 브랜치 생성 (Worktree 추가)**:
     ```bash
     git worktree add ../TestMCP_worktrees/[작업타입]_[작업명] -b [작업타입]_[작업명] develop
     ```
  2. **격리 개발 및 PR 생성**:
     - 생성된 워크트리 디렉토리(`../TestMCP_worktrees/...`)에서 작업, 커밋, 푸시 및 `develop` 대상 PR 생성
  3. **PR 머지 후 정리**:
     - 사용자가 GitHub에서 PR을 머지하면 워크트리 및 브랜치를 삭제 정리:
     ```bash
     git worktree remove ../TestMCP_worktrees/[작업타입]_[작업명]
     git branch -d [작업타입]_[작업명]
     ```

## 3. 작업 브랜치 명칭 규칙 (Branch Naming)
- **형식**: `[작업 타입]_[작업명]`
- **예시**: `feat_player_movement`, `fix_camera_jitter`, `refactor_ui_menu`, `test_inventory_nunit`
- **머지 원칙**: 모든 브랜치 간의 최종 병합(Merge)은 **사용자가 직접 수행**합니다 (에이전트 임의 머지 금지).

## 4. 깃 커밋 메시지 컨벤션 (Commit Convention)
- **헤더 형식**: `[타입] : 메시지 내용`
  - 예시: `[feat] : 플레이어 기본 이동 기능 추가`, `[fix] : 카메라 지터링 버그 수정 #12`
- **8대 허용 타입**:
  - `feat` : 새로운 기능에 대한 커밋
  - `fix` : 버그 수정에 대한 커밋
  - `build` : 빌드 관련 파일 수정 커밋
  - `chore` : 자잘한 수정
  - `docs` : 문서 수정
  - `style` : 코드 스타일 혹은 포맷의 수정
  - `refactor` : 코드 리팩토링에 대한 커밋
  - `test` : 테스트 코드 및 오브젝트에 대한 커밋
- **작성 규칙**:
  - 메시지 내용은 한 줄로 적을 수 있을 만큼 요약해서 작성합니다.
  - Body는 기본적으로 생략하며, 헤더 한 줄로 작성하기 어려울 경우에만 선택적으로 작성합니다.
  - 이슈와 연관되어 있다면 끝에 `#nnn`을 첨부합니다.

## 5. 풀 리퀘스트 규칙 (Pull Request Convention)
- **PR 타이틀 형식**: `작업내용 - [에이전트 명]`
  - 예시: `플레이어 기본 이동 기능 구현 - [unity_builder]`, `카메라 지터링 버그 수정 - [developer]`
- **PR 본문(Body) 작성**:
  - 해당 PR에 포함된 커밋 내용들을 간결하게 요약해서 작성합니다.

## 6. Unity .meta 파일 무결성 검증 규칙
- `Assets/` 폴더 내 C# 스크립트, 씬, 프리팹, 에셋 파일이 추가/수정/삭제될 때 반드시 대응하는 `.meta` 파일이 1:1로 함께 스테이징되었는지 `git status`로 검증합니다.
- `.meta` 파일이 누락된 경우 커밋을 중단하고 누락된 메타 파일을 추가한 뒤 진행합니다.
