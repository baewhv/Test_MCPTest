# Unity MCP 도구 활용 가이드

## 1. 주요 도구 카테고리별 매핑

### 스크립트 작성 및 수정
- `create_script`: 신규 C# 스크립트 생성
- `manage_script`: 스크립트 메타데이터 및 분석
- `apply_text_edits`: C# 스크립트의 특정 라인/블록 단위 수정

### 게임 오브젝트 및 컴포넌트 관리
- `find_gameobjects`: 씬 내의 게임오브젝트 검색
- `manage_gameobject`: 오브젝트 생성, 삭제, 이름 변경, 활성화/비활성화
- `manage_components`: 컴포넌트 부착, 제거, 필드/프로퍼티 값 설정

### 씬 및 에셋 제어
- `manage_scene`: 씬 열기, 저장, 새로 만들기
- `manage_asset`: 에셋 임포트, 이동, 이름 변경
- `manage_material`: 머티리얼 및 셰이더 프로퍼티 설정
- `manage_physics`: 물리 레이어 및 충돌 매트릭스 설정

### 검증 및 에디터 기능
- `read_console`: 콘솔 에러, 경고, 로그 확인
- `execute_code`: 에디터 환경에서 C# 코드 즉시 실행 (테스트용)
- `run_tests`: Unity Test Framework 테스트 실행

---

## 2. 작업 안전 수칙

1. **작업 전 씬 저장**: 큰 변경 전에 `manage_scene`으로 현재 씬을 저장합니다.
2. **에러 즉시 확인**: 스크립트나 컴포넌트 조작 후 반드시 `read_console`을 호출하여 컴파일 오류나 Missing Reference가 없는지 확인합니다.
3. **.meta 파일 보존**: 에셋이나 스크립트 이동/생성 시 `.meta` 파일이 손상되지 않도록 유의합니다.
