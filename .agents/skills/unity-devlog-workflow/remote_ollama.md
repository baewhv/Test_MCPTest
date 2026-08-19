## 원격 분산 Ollama 클러스터 연동 가이드

현재 작업 환경에서 사용할 수 있는 외부 사설 GPU Ollama 클러스터 게이트웨이 정보입니다.
대량의 코드 생성, 기획서 작성, 분산 연산이 필요할 때 해당 엔드포인트를 호출하여 작업할 수 있습니다.

### 1. 접속 정보
* Base URL: http://baewhv.iptime.org:11435
* 인증 헤더: Authorization: Bearer sk-master-ollama-key
* Content-Type: application/json

### 2. 사용 가능한 모델 및 역할 분담
1. **gemma4:12b** (대화/기획/멀티모달 전담)
    * 역할: 게임 기획서 작성, 긴 글 논리 전개, 텍스트 및 오디오/비전 멀티모달 분석
2. **codellama:7b** (코드 생성 전담)
    * 역할: C#, Unity 스크립트, 자료구조 및 알고리즘 구현

### 3. 호출 방식 (Node.js / Python 스크립트 실행)
작업 중 원격 모델 연산이 필요할 경우, 아래와 같이 HTTP POST 요청을 전송하여 결과를 수신합니다:

- 엔드포인트: http://baewhv.iptime.org:11435/api/generate
- 요청 바디:
    {
    "model": "codellama:7b",  // 또는 "gemma4:12b"
    "prompt": "<요청할 프롬프트 내용>",
    "stream": false
    }
