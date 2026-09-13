---
name: unity-cli-runner
description: GEMINI_SETUP 초기 프로젝트 셋업 전용 Unity CLI 도구 (실무 에이전트의 일상 개발/검수 작업 중 호출 일체 금지)
---

# Unity CLI Runner Skill (Setup Only)

> [!IMPORTANT]
> **실무 에이전트 호출 전면 금지 (Prohibited in Regular Workflows)**:
> Unity CLI는 일반 기능 개발 및 검수(`Developer`, `QA`, `PM`) 단계에서 호출이 전면 제외/금지됩니다.
> 실무 개발 및 런타임/콘솔 검수는 실행 중인 에디터와 직접 통신하는 `Unity MCP`를 전담으로 사용합니다.

---

## 1. 전담 용도 (Intended Scope)
- **`GEMINI_SETUP` 초기 환경 설정 전용**:
  - 신규 프로젝트 셋업 시 로컬에 Unity Project가 설치/생성되지 않은 경우 빈 유니티 프로젝트를 자동 생성(`-createProject`)하거나 초기 패키지를 설치하는 용도로 한정 운용됩니다.
  - 구체적인 설치 파라미터 및 실행 절차는 사용자 추가 가이드에 따라 `GEMINI_SETUP` 스크립트와 연계됩니다.

## 2. 실무 에이전트 준수 수칙
- `Developer`: C# 컴파일 확인 시 Unity CLI를 호출하지 않고 Unity MCP를 통해 무결성을 확인합니다.
- `QA`: NUnit 테스트 검수 시 Unity CLI 무인 실행을 호출하지 않고 작성된 테스트 코드의 정적 무결성 및 엔진 런타임을 점검합니다.
