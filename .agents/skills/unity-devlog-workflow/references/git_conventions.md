# Git 작업 내역 기록 가이드라인

## 1. 커밋 메시지 규칙 (Conventional Commits)

작업 내역은 다른 개발자나 에이전트가 쉽게 파악할 수 있도록 표준 형식을 준수합니다.

```
<type>(<scope>): <간결한 요약 (한글 또는 영문)>

[본문]
- 상세 변경 내용 1
- 상세 변경 내용 2
- 생성/수정된 Unity 에셋 및 스크립트

[꼬리말 (선택사항)]
Fixes #이슈번호 또는 Ref #이슈번호
```

### Type 목록
- `feat`: 새로운 기능, 컴포넌트, 게임 시스템 추가
- `fix`: 버그 수정, 런타임/컴파일 에러 해결
- `refactor`: 코드 리팩토링, 구조 개선 (기능 변경 없음)
- `style`: 코드 포맷팅, 네이밍 정리, 씬 레이아웃 정리
- `docs`: 문서, 주석, README 수정
- `test`: 유닛 테스트, 통합 테스트 추가/수정
- `chore`: 빌드 설정, 패키지 매니저 종속성, 프로젝트 세팅 수정

### Scope 예시
- `scene`: 특정 씬 관련 작업 (예: `feat(scene): MainMenu 씬 UI 배치`)
- `player`: 플레이어 컨트롤러 또는 캐릭터 관련
- `ui`: UI/UX 컴포넌트 관련
- `audio`: 사운드 및 오디오 믹서 관련
- `physics`: 물리 엔진 및 충돌체 설정 관련

---

## 2. 작업 내역 커밋 절차

1. **상태 확인**: `git status`로 변경된 파일 및 메타 파일(`.meta`) 누락 여부 확인
2. **스테이징**: `git add <경로>` (Unity 작업 시 `.meta` 파일이 함께 추가되었는지 필수 확인)
3. **커밋 실행**: 
   ```bash
   git commit -m "feat(player): 이동 스크립트 및 Rigidbody 설정" -m "- PlayerMovement.cs 생성\n- Inspector 속성 바인딩\n- Unity MCP 검증 완료"
   ```
4. **원격 동기화**: `git push origin <브랜치명>` 또는 GitHub MCP 연동
