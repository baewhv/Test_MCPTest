# 프로젝트 마스터 색인 (Project Master Index)

이 문서는 프로젝트의 모든 기획 명세, 아키텍처, 실시간 작업 상태 및 외부 연동 설정을 총괄하는 최상위 마스터 색인입니다.

---

## 0. 프로젝트 환경 설정 및 표준 규칙
- [프로젝트 환경 명세서 (PROJECT_SPEC.md)](./PROJECT_SPEC.md): GitHub 저장소 정보, Notion DB ID, Unity 환경 설정
- [C# 코딩 및 아키텍처 규칙 (csharp_coding_rule.md)](../.agents/rules/csharp_coding_rule.md): 직렬화 캡슐화, 라이프사이클, 에디터 스크립팅 제한
- [유니티 폴더 및 에셋 네이밍 규칙 (unity_folder_rule.md)](../.agents/rules/unity_folder_rule.md): 디렉토리 구조, `_Imports/` 원본 분리, 에셋 접두사/접미사
- [AI 리소스 생성 및 임포트 규칙 (asset_generation_rule.md)](../.agents/rules/asset_generation_rule.md): 나노바나나/UnityMCP 리소스 제작 및 프리팹 가공 4단계

---

## 1. 시스템 및 기능 기획 명세서 (Specifications)
- [기획서 등록 가이드 및 템플릿](./specs/README.md)
- *(신규 기능 기획서가 `docs/specs/`에 추가되면 이곳에 링크가 등록됩니다)*

---

## 2. 작업 상태 및 개발 워크플로우 (Work & Status)
- [현재 개발/기획 진행 상태 (status.md)](./work/status.md): AI 상태 제어 및 실시간 현황판
- [세분화 태스크 체크리스트 (worklist.md)](./work/worklist.md): 미완료/완료 개발 단위 체크리스트

---

## 3. 실시간 협업 소통 로그 (Agent Communication Logs)
- [실시간 소통 타임라인 로그 폴더](./logs/): 사용자 모니터링 및 감사용 일일 타임라인
