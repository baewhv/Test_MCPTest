# 프로젝트 환경 명세서 (Project Environment Specification)

이 문서는 4대 에이전트와 연동 스킬이 올바르게 작동하기 위해 필요한 프로젝트 고유 메타데이터 및 외부 연동 명세서입니다.
새로운 프로젝트를 시작하거나 템플릿을 복제한 경우, 아래 항목을 실제 프로젝트 환경에 맞게 입력해 주세요.

> [!NOTE]
> GitHub 토큰, Notion 토큰 등 민감한 인증 키(API Key/PAT)는 이곳에 입력하지 마시고, MCP 설정(`config/mcp_config.json`)을 통해 안전하게 관리됩니다.

---

## 1. 버전 관리 및 저장소 정보 (Git & GitHub)
- **GitHub Repository Owner**: `baewhv`
- **GitHub Repository Name**: `Test_MCPTest`
- **GitHub Repository URL**: `https://github.com/baewhv/Test_MCPTest`
- **Default Integration Branch**: `develop`
- **Release Branch**: `main`
- **Worktree Parent Directory**: `../TestMCP_worktrees`

---

## 2. 외부 연동 명세 (Notion & External Services)
- **Notion Database Name**: `학습일지`
- **Notion Database ID**: `13cc49b1-3a07-814e-b7b5-cf14b64ca1ee`
- **Notion Page Title Format**: `[YYYY-MM-DD] 작업 기록`

---

## 3. Unity 프로젝트 환경 명세 (Unity Specification)
- **Unity Project Name**: `TestMCP`
- **Target Platform**: `PC, Mac & Linux Standalone`
- **Unity Asset Root**: `Assets/`
- **Default Screenshot Output**: `Assets/Screenshots`
- **Core Loop Test Scene**: `Assets/Scenes/SampleScene.unity`
