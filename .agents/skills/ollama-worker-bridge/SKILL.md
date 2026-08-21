---
name: ollama-worker-bridge
description: 원격 사설 Ollama 서버(codellama:7b, gemma4:12b)를 호출하여 연산하고, 300초 타임아웃 제어, 소요시간 측정 및 일일 통합 로그(docs/logs/worker_log_YYYY-MM-DD.md)에 자동 기록하는 워커 브릿지 스킬
---

# Ollama Worker Bridge Skill

이 스킬은 `code_worker`와 `docs_worker`가 원격 Ollama 서버와 통신할 때 사용하는 표준 실행 도구입니다.

## 1. 주요 기능
- **원격 엔드포인트 통신**: `http://baewhv.iptime.org:11435/api/generate`
- **5분(300초) 안전 타임아웃**: VRAM 과부하 및 프리징 방지
- **자동 통합 표 로깅**: `docs/logs/worker_log_YYYY-MM-DD.md` 문서 자동 생성 및 인덱스 누적 기록
- **표준 CLI 인터페이스**: 단일 스크립트 실행으로 호출 및 결과 반환

## 2. 사용법 (CLI 명령어)
```bash
# code_worker가 C# 코드 생성을 요청할 때
node .agents/skills/ollama-worker-bridge/scripts/ollama_worker.js --model codellama:7b --worker code_worker --desc "플레이어 이동 로직 구현" --prompt "프롬프트 내용"

# docs_worker가 기획서/다이어그램 생성을 요청할 때
node .agents/skills/ollama-worker-bridge/scripts/ollama_worker.js --model gemma4:12b --worker docs_worker --desc "인벤토리 시스템 기획 스펙 작성" --prompt "프롬프트 내용"
```
