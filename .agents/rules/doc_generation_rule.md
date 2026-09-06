# 프로젝트 문서 작성 및 관리 표준 규칙 (Documentation Standards)

이 문서는 프로젝트의 4대 핵심 문서 체계(`specs`, `tech_spec`, `implementations`, `ARCHITECTURE.md`) 및 에이전트별 문서 작성/갱신 규격을 규정합니다.

---

## 1. 4대 핵심 문서 체계 및 작성 주체 (Document Architecture)

| 문서 체계 | 저장 경로 | 작성/갱신 주체 | 성격 및 설명 |
| :--- | :--- | :--- | :--- |
| **기획서 원본** | `docs/specs/` | **사용자** (User) | 사용자가 작성한 게임 기획서 원본 (**엄격한 읽기 전용**) |
| **기획 상세 명세서** | `docs/tech_spec/` | **Designer** | 원본 기획서를 분석하여 작성한 5대 무결성 기술 명세서 |
| **구현 기술문서** | `docs/implementations/` | **Developer** | C# 구현 코드와 1:1 대응하는 클래스/API/직렬화 상세 문서 |
| **중앙 아키텍처 지도** | `docs/ARCHITECTURE.md` | **Developer** / **Designer** | 컴포넌트 관계도, 충돌 매트릭스, 이벤트 색인 |
| **실시간 작업 상태판** | `docs/work/status.md` | **전 에이전트** | 현재 진행 중인 태스크 및 FSM 상태판 |
| **작업 체크리스트** | `docs/work/worklist.md` | **Designer** / **QA** | 전체 구현 태스크 목록 및 완료 체크 |
| **협업 타임라인 로그** | `docs/logs/` | **전 에이전트** | `agent-communication-logger` 실시간 인계 기록 |

---

## 2. Designer 기획 상세 명세서 작성 규격 (`docs/tech_spec/`)
1. **명세서 파일명**: `[번호]_[시스템명]_tech_spec.md` (예: `01_system_mechanics_tech_spec.md`)
2. **5대 필수 포함 항목**:
   - 시스템 개요 및 코어 게임 루프 연동점
   - 정밀 수치 규칙 (이동 속도, 쿨다운, 점수, 피격 판정)
   - 상태 머신(FSM) 전이 다이어그램 및 파라미터 규격
   - 엣지 케이스 및 예외 처리 방어 규칙
   - Developer를 위한 `worklist.md` 구현 태스크 도출

---

## 3. Developer 구현 기술문서 작성 규격 (`docs/implementations/`)
1. **기술문서 파일명**: `[태스크명]_impl.md` (예: `task4_3_challenging_stage_impl.md`)
2. **필수 포함 항목**:
   - 구현 목적 및 설계 의도 (Rationale)
   - C# 클래스 구조 및 주요 Public API 시그니처
   - 인스펙터 직렬화 필드(`[SerializeField] private`) 바인딩 표
   - 프리팹 계층 구조 및 컴포넌트 구성도
   - `.meta` 파일 및 의존성 관계

---

## 4. 문서 보존 및 Post-Merge 일괄 반영 원칙
- 모든 작업 문서는 `feat/...` 브랜치에 커밋하지 않고 로컬에 격리 보존됩니다.
- 사용자가 PR을 머지한 후, PM이 `develop` 브랜치에서 일괄 커밋/푸시(`git-doc-sync`)하여 문서를 최종 반영합니다.

