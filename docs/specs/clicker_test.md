# 클리커 게임 스펙 명세서 (Clicker Test)

## 1. 개요
화면을 클릭하면 점수가 1씩 증가하고 UI에 표시되는 기본 클리커 게임 시스템.

## 2. 클래스 참조 맵
- `ClickerController`: 점수 관리, `AddScore`, `ResetScore`, `OnScoreChanged` 이벤트
- `ClickerTarget`: `IClickable` 구현체, 클릭 시 컨트롤러에 점수 추가 요청
- `ClickerScoreView`: 점수 변경 이벤트를 구독하여 TextMeshProUGUI 업데이트
- `IClickable`: 클릭 가능한 객체의 인터페이스

## 3. QA 인수 기준 (Acceptance Criteria)
- [x] `ClickerController.AddScore(1)` 호출 시 Score가 1 증가해야 함
- [x] 음수 또는 0 점수 추가 시 무시되어야 함
- [x] `ResetScore()` 호출 시 Score가 0으로 초기화되어야 함
- [x] NUnit 단위 테스트가 모두 통과해야 함
