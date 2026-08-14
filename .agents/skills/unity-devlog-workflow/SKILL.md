---
name: unity-devlog-workflow
description: >-
  Unity MCP를 이용한 작업(씬 편집, 스크립트 작성, 테스트 등)을 수행하고, 
  작업 결과를 Git에 작업 내역(커밋 및 푸시)으로 자동 기록하며, 
  Notion "학습일지" 캘린더 데이터베이스에 당일 작업 일지를 등록/관리하는 표준 워크플로우를 안내할 때 사용합니다.
---

# Unity MCP 기반 개발 및 Git / Notion 일지 기록 워크플로우

이 스킬은 Unity 작업을 `unityMCP` 도구로 진행한 뒤, 작업 결과와 진행 내역을 Git 작업 내역(커밋 및 푸시)으로 자동 반영하고 Notion "학습일지" 캘린더에 일지를 체계적으로 관리하는 표준 절차를 정의합니다.

---

## 1. 전체 워크플로우 요약

```
[1단계: Unity 작업] -> [2단계: 작업 검증] -> [3단계: Git 자동 커밋/푸시] -> [4단계: Notion 학습일지 등록]
   (unityMCP)          (read_console / tests)     (에이전트 자율 관리)             (notion MCP 중복 확인 후)
```

---

## 2. 세부 단계별 실행 지침

### 1단계: Unity MCP 작업 수행
- `unityMCP` 도구를 활용하여 요청된 기능(스크립트 작성, 게임오브젝트 배치, 컴포넌트 추가/수정, 머티리얼 설정 등)을 수행합니다.
- 주요 사용 도구:
  - 스크립트 생성/수정: `create_script`, `manage_script`, `apply_text_edits`
  - 게임오브젝트/컴포넌트: `manage_gameobject`, `manage_components`, `find_gameobjects`
  - 씬/에셋 관리: `manage_scene`, `manage_asset`, `manage_material`
  - 툴/에디터 제어: `execute_code`, `execute_menu_item`

### 2단계: 결과 및 콘솔 검증
- 작업 후 Unity 콘솔 에러를 확인하여 작업 안전성을 검증합니다.
- 콘솔 확인: `unityMCP`의 `read_console` (action: "get", types: ["error", "warning"])
- 스크립트 컴파일 오류 또는 런타임 에러가 없는지 확인합니다.

### 3단계: Git 작업 내역 자동 반영 (에이전트 자율 위임)
- 에이전트가 작업 변경 사항(`git status`, `git diff`)을 분석하여 논리적 단위로 스테이징 및 커밋합니다.
- Conventional Commits 형식(`feat`, `fix`, `chore`, `docs` 등)으로 명확한 커밋 메시지를 작성합니다.
- 로컬 커밋 완료 후 원격 저장소(`origin/main`)로 `git push`까지 에이전트가 주도적으로 수행합니다.
- 상세 규칙: [Git 커밋 가이드](./references/git_conventions.md) 참조

### 4단계: Notion "학습일지" 캘린더 등록 규칙
- **데이터베이스**: `학습일지` (`13cc49b1-3a07-814e-b7b5-cf14b64ca1ee`)
- **제목 형식**: `[YYYY-MM-DD] 작업 기록` (예: `[2026-08-14] 작업 기록`)
- **분류 속성**: `일지`
- **날짜 속성 (Date)**: 작업 당일 날짜 (`YYYY-MM-DD`)
- **중복 방지 규칙**:
  - 일지 등록 전 Notion 검색(`API-post-search` 또는 `API-query-data-source`)으로 당일 날짜의 `[YYYY-MM-DD] 작업 기록` 페이지가 이미 존재하는지 확인합니다.
  - **이미 존재하는 경우**: 새 페이지를 추가로 생성하지 않습니다 (필요 시 기존 페이지 본문에 추가/업데이트).
  - **존재하지 않는 경우**: 새 일지 페이지를 생성하고 상세 작업 내역 마크다운을 작성합니다.
- 상세 템플릿: [Notion 일지 템플릿 문서](./references/notion_template.md) 참조

---

## 3. 참조 문서
- [Git 커밋 가이드](./references/git_conventions.md)
- [Notion 일지 템플릿 및 API 구조](./references/notion_template.md)
- [Unity MCP 도구 활용 가이드](./references/unity_mcp_guide.md)
