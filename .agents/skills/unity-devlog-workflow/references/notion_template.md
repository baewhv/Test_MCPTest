# Notion 캘린더 개발 일지 작성 템플릿

## 1. 대상 캘린더 데이터베이스 정보

- **데이터베이스 이름**: `학습일지`
- **Database ID**: `13cc49b1-3a07-814e-b7b5-cf14b64ca1ee`
- **Data Source ID**: `13cc49b1-3a07-815e-816e-000b846608a5`
- **속성 구조**:

| 속성명 | 타입 | 옵션 / 값 설명 | 예시 |
| :--- | :--- | :--- | :--- |
| **Name** | Title | 일지 제목 (형식 고정) | `[2026-08-14] 작업 기록` |
| **Date** | Date | 작업 수행 일자 | `{"start": "2026-08-14"}` |
| **분류** | Select | `일지`, `알아보기`, `학습완료` | `일지` |
| **생성 일시** | Created Time | 시스템 자동 기록 | (자동 생성) |

---

## 2. 일지 작성 및 중복 방지 절차

1. **사전 조회**:
   - `API-post-search` (query: `[YYYY-MM-DD] 작업 기록`)를 호출하여 해당 날짜의 일지 페이지가 이미 존재하는지 확인합니다.
2. **분기 처리**:
   - **페이지가 이미 존재하는 경우**: 새 페이지를 추가하지 않습니다 (필요 시 기존 페이지 본문에 추가 기록).
   - **페이지가 없는 경우**: 아래 3단계에 따라 `API-post-page`로 페이지를 생성하고 본문을 작성합니다.

---

## 3. 일지 페이지 본문 마크다운 템플릿

```markdown
# 🛠️ 작업 개요
- **작업 일시**: YYYY-MM-DD
- **작업자**: Antigravity & User
- **핵심 목표**: [오늘 달성하고자 한 핵심 목표]

---

## 📋 세부 작업 내역 (Unity MCP)
1. **생성/수정된 스크립트**
   - `Assets/Scripts/Player/PlayerController.cs`: 이동 및 점프 로직 추가
2. **Unity 씬 & 오브젝트 구성**
   - `Player` 게임오브젝트에 `Rigidbody`, `CapsuleCollider` 부착
   - `GroundCheck` 자식 오브젝트 추가 및 레이어 설정
3. **사용된 Unity MCP 도구**
   - `create_script`, `manage_gameobject`, `manage_components`

---

## 🔍 테스트 및 검증 결과
- **Console 로그**: 에러 0건, 경고 0건 (`unityMCP:read_console` 확인 완료)
- **플레이 모드 테스트**: 키 입력에 따른 정상 이동 확인

---

## 📦 Git 반영 내역
- **Commit**: `feat(player): 플레이어 이동 로직 및 컴포넌트 구성`
- **변경 파일 수**: N개 파일 변경

---

## 📌 다음 작업 예정 (Next Steps)
- [ ] 플레이어 애니메이션 연동
- [ ] 카메라 팔로우(Cinemachine) 설정
```

---

## 4. Notion MCP API 호출 예시

```json
{
  "parent": {
    "database_id": "13cc49b1-3a07-814e-b7b5-cf14b64ca1ee"
  },
  "properties": {
    "Name": {
      "title": [
        {
          "text": {
            "content": "[2026-08-14] 작업 기록"
          }
        }
      ]
    },
    "Date": {
      "date": {
        "start": "2026-08-14"
      }
    },
    "분류": {
      "select": {
        "name": "일지"
      }
    }
  }
}
```
