---
name: unity-devlog-workflow
description: >-
  Unity MCP를 이용한 작업(씬 편집, 스크립트 작성, 테스트 등)을 수행하고, 
  작업 결과를 Git에 작업 내역(커밋)으로 기록하며, 
  Notion 캘린더 데이터베이스에 개발 일지(Dev Log) 형태로 등록하는 표준 워크플로우를 안내할 때 사용합니다.
---

# Unity MCP 기반 개발 및 Git / Notion 일지 기록 워크플로우

이 스킬은 Unity 작업을 `unityMCP` 도구로 진행한 뒤, 작업 결과와 진행 내역을 Git 작업 내역(커밋) 및 Notion 캘린더 일지로 체계화하여 기록하는 표준 절차를 정의합니다.

---

## 1. 전체 워크플로우 요약

```
[1단계: Unity 작업] -> [2단계: 작업 검증] -> [3단계: Git 커밋] -> [4단계: Notion 캘린더 일지 작성]
   (unityMCP)          (read_console / tests)    (git CLI / GitHub MCP)   (notion MCP)
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

### 3단계: Git 작업 내역 기록
- 작업 변경 사항(`git status`, `git diff`)을 확인하고 변경된 파일들을 스테이징합니다.
- Conventional Commits 형식으로 명확한 커밋 메시지를 작성합니다:
  - 형식: `<type>(<scope>): <간결한 작업 요약>`
  - 본문: 구체적인 구현 내역, 생성/수정된 에셋 및 스크립트 목록
  - 상세 규칙: [Git 커밋 규칙 문서](./references/git_conventions.md) 참조
- 로컬 `git commit` 실행 또는 `github` MCP 도구(`push_files` 등)를 통해 작업 내역을 반영합니다.

### 4단계: Notion 캘린더 개발 일지(Dev Log) 등록
- Notion MCP를 사용하여 지정된 캘린더 데이터베이스에 오늘 날짜의 개발 일지 페이지를 생성합니다.
- 기록 항목:
  - **제목**: `[YYYY-MM-DD] <작업 제목/요약>`
  - **날짜 속성 (Date)**: 오늘 날짜 (`YYYY-MM-DD`)
  - **태그/상태 (Select/Multi-select)**: 작업 분류 (예: Unity, Feature, Bugfix, Test)
  - **본문 내용**:
    1. **작업 목표 & 요약**
    2. **Unity MCP 작업 내역** (생성/수정된 오브젝트, 컴포넌트, 스크립트)
    3. **Git 커밋 정보** (커밋 해시 / 커밋 메시지)
    4. **검증 결과 & 특이사항** (콘솔 에러 유무, 이슈 사항)
    5. **다음 진행 예정 작업**
  - 상세 템플릿: [Notion 일지 템플릿 문서](./references/notion_template.md) 참조

---

## 3. 참조 문서
- [Git 커밋 가이드](./references/git_conventions.md)
- [Notion 일지 템플릿 및 API 구조](./references/notion_template.md)
- [Unity MCP 도구 활용 가이드](./references/unity_mcp_guide.md)
