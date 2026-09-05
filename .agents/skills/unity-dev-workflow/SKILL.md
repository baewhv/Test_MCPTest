---
name: unity-dev-workflow
description: Developer 에이전트가 docs/tech_spec/ 분석, 4단계 아키텍처 구현, CLI 컴파일 검증, docs/implementations/ 기술문서 작성 및 GitManager 인계를 완결하는 표준 개발 워크플로우 스킬입니다.
---

# Unity 클라이언트 개발 및 구현 기술문서 작성 워크플로우

이 스킬은 Developer 에이전트가 단일 개발 태스크를 수신했을 때 시작부터 인계까지 완결하는 5단계 표준 절차 및 기술 제안 프로토콜을 정의합니다.

---

## 1. 개발 5단계 표준 워크플로우

### [1단계: 사전 명세 분석 및 브랜치 준비]
1. `docs/tech_spec/[시스템명]_tech_spec.md` 및 `docs/PROJECT_SPEC.md`의 아키텍처 기준을 1차 참조합니다.
2. `GitManager`에게 `develop` 최신 패치 및 작업 브랜치 생성을 요청합니다.

### [2단계: 4단계 아키텍처 우선(Architecture-First) 구현]
아래 의존성 순서에 따라 C# 스크립트와 프리팹을 조립합니다:
1. **[1단계] 기반 인프라 & 데이터 계약**: 공유 인터페이스(`IDamageable`), Data SO 스키마, 코어 매니저
2. **[2단계] 수학/이동 유틸리티 & 베이스 클래스**: 궤적 계산 모듈, 추상 클래스(`EnemyBase`), 오브젝트 풀러
3. **[3단계] 액터 엔티티 & Zero-Override 완제품 프리팹**: 플레이어, 적 AI 기체, 2D 히트박스 바인딩
4. **[4단계] HUD/UI 및 연출**: 스코어보드, 파티클 이펙트/사운드 바인딩

### [3단계: 백그라운드 사전 컴파일 검증]
코드 작성 후 아래 명령을 실행하여 컴파일 에러가 0건인지 자체 검증합니다:
```bash
node .agents/skills/unity-cli-runner/scripts/unity_cli.js compile
```

### [4단계: 구현 기술문서 작성 및 아키텍처 관계도 동기화]
1. **개별 구현 기술문서 작성**: `docs/implementations/[태스크명]_impl.md` 파일을 생성하고 아래 양식으로 작성합니다:
   ```markdown
   # [태스크명] 구현 기술 명세서 (Implementation Spec)

   ## 1. 구현 클래스 및 구조 요약
   - **클래스명**: `[클래스명]`
   - **상속/인터페이스**: `MonoBehaviour`, `IDamageable` 등
   - **주요 역할**: ...

   ## 2. 직렬화 필드 및 인스펙터 바인딩 ([SerializeField])
   | 필드명 | 타입 | 바인딩 대상 / 기본값 | 설명 |
   | :--- | :--- | :--- | :--- |

   ## 3. 주요 공개 API 및 메서드 계약 (Public API)
   - `public void MethodName(int arg)`: ...

   ## 4. 핵심 알고리즘 및 설계 Rationale
   - (설계 결정 이유, 최적화 기법, 엣지 케이스 처리)
   ```
2. **아키텍처 관계도 동기화**: `docs/ARCHITECTURE.md`의 상호작용 매트릭스, 충돌 매트릭스, 이벤트 흐름표에 1줄씩 간결하게 관계도를 추가합니다.

### [5단계: 상태 현황판 갱신 및 GitManager 직접 인계]
1. `docs/work/status.md`의 `[현재 상태]`를 `[Developer] [기능명] C# 구현 및 기술문서 작성 완료 ➔ git_manager에게 커밋/PR 인계`로 갱신합니다.
2. 아래 소통 로거를 실행하고 턴을 종료합니다:
   ```bash
   node .agents/skills/agent-communication-logger/scripts/log_comm.js --from "Developer" --to "GitManager" --type "PR 요청" --msg "[기능명] C# 구현 및 docs/implementations/ 작성 완료, 커밋/PR 요청"
   ```

---

## 2. GitHub Issue 기술 제안 프로토콜 (개선/리팩토링 제안 시)
코드 개선점 발견 시 임의 수정하지 않고 `GitManager`에게 아래 양식으로 이슈 등록을 요청합니다:
- **제안 제목**: `[AI_developer][제안] [기능 요약]`
- **제안 본문 양식**:
  ```markdown
  ## 1. 변경 사유
  - (현재 문제 상황, 성능 저하 또는 구조적 한계 기술)

  ## 2. 변경 방법
  - (구체적인 클래스 설계, 인터페이스 도입, 리팩토링 방향 기술)
  - *(필요 시 mermaid 다이어그램 첨부)*

  ## 3. 변경 시 예상되는 결과 및 우려사항
  - **예상되는 결과**: ...
  - **잠재적 우려사항 및 고려점**: ...
  ```
- **반려 이슈 재제안 시**: 기존 반려 사유를 해소할 수 있는 추가적인 보완 근거 및 변경 대안을 상세히 작성하여 `GitManager`에게 전달합니다.
