# 프로젝트 에이전트 협업 및 운영 규칙 (Project Rules)

> [!NOTE]
> 언어/커뮤니케이션, 보안/마스킹, 코드 품질 및 문서화(.md) 아티팩트 생성 규칙은 전역 규칙(`Global Rules`)을 따릅니다.

---

## 0. 프로젝트 환경 설정 상태 (Setup Status)
- **상태**: `[SETUP_COMPLETED]`
<!-- 새 프로젝트 템플릿 복제 시 "미완료"로 시작하며, docs/PROJECT_SPEC.md 설정 완료 후 "[SETUP_COMPLETED]"로 갱신됩니다. -->

---

## 1. 사용자 작업 지시 및 PM 위임 원칙 (PM Delegation Rule)
- 메인(Default) 에이전트 및 PM은 시스템 프롬프트에 상시 로드된 상단의 **`프로젝트 환경 설정 상태`**를 확인한다:
  - `[SETUP_COMPLETED]`: 별도의 파일 읽기 도구 호출 없이 작업을 즉시 진행한다 (0-Tool-Call).
  - `미완료`: `docs/PROJECT_SPEC.md`를 확인하여 사용자에게 필수 환경 정보 입력을 요청하고 작업을 대기한다.
- 메인 에이전트는 작업 실행 지시 수신 시 직접 코딩이나 검수를 수행하지 않고 **`invoke_subagent` 도구를 호출하여 `PM` 에이전트에게 지시를 위임**한다.

---

## 2. 읽기 전용 문서 위치 (Read-Only Specifications)
- 아래 경로의 문서는 사용자가 직접 작성한 원본 문서이므로, 모든 에이전트는 **수정 및 덮어쓰기가 절대 불가능하며 오직 읽기(Read-Only)**만 수행한다:

| 경로 (Path) | 설명 (Description) | 에이전트 접근 권한 |
| :--- | :--- | :--- |
| `docs/specs/` | 사용자가 등록한 게임 시스템/기능 기획서 원본 | **엄격한 읽기 전용 (Strict Read-Only)** |
