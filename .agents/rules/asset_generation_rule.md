# AI 리소스 생성 및 임포트 규칙 (AI Asset Generation Rules)

이 문서는 나노바나나(NanoBanana), Antigravity 내장 이미지 생성기 및 UnityMCP 생성 도구를 활용하여 2D 이미지, 오디오, 3D 모델 리소스를 제작하고 유니티 프로젝트에 임포트/가공하는 표준 워크플로우를 규정합니다.

---

## 1. 리소스 유형별 생성 도구 및 파라미터 표준

### ① 2D 스프라이트 및 텍스처 (2D Sprites & Textures)
- **도구**: Antigravity `generate_image` (NanoBanana / Imagen) 또는 UnityMCP `generate_image`
- **저장 위치**: 반드시 **`Assets/_Imports/Textures/`** 또는 **`Assets/_Imports/Sprites/`**에 저장합니다.
- **네이밍**: `T_[이름].png` (텍스처), `SP_[이름].png` (스프라이트)
- **임포트 후처리**:
  - UnityMCP `manage_texture`를 호출하여 Texture Type을 **`Sprite (2D and UI)`**로 설정하고, 픽셀 아트일 경우 Filter Mode를 `Point (no filter)`, Compression을 `None`으로 설정합니다.

### ② 오디오 및 사운드 효과 (Audio: BGM & SFX)
- **도구**: UnityMCP `generate_audio` (fal-ai/stable-audio-25, cassetteai)
- **저장 위치**: 반드시 **`Assets/_Imports/Audio/`**에 저장합니다.
- **네이밍**: `BGM_[이름].wav` (배경음), `SFX_[이름].wav` (효과음)
- **파라미터 가이드**: SFX는 duration 1~5초, BGM은 duration 30~120초 지정.

### ③ 3D 모델 및 메시 (3D Models & Meshes)
- **도구**: UnityMCP `generate_model` (Tripo, Meshy 등 - text->3D, image->3D)
- **저장 위치**: 반드시 **`Assets/_Imports/Models/`**에 저장합니다.
- **포맷**: `fbx` 또는 `obj`, `glb`
- **네이밍**: `M_[이름].[ext]`

---

## 2. 리소스 가공 및 프리팹 완결 4단계 파이프라인

모든 리소스 생성 요청 시 아래의 **4단계 파이프라인**을 거쳐 완제품으로 가공합니다:

```
[1단계: 원본 생성] ➔ [2단계: _Imports 배치] ➔ [3단계: 머티리얼/임포터 설정] ➔ [4단계: 프리팹 조립]
(NanoBanana/MCP)    (Audio/Textures/Models)    (Sprite 세팅, M_*.mat 생성)     (PF_*.prefab 완성)
```

1. **1단계 (생성)**: 사용자의 요청 의도(화풍, 테마, 길이)에 맞춰 프롬프트를 정제하고 생성 도구를 호출합니다.
2. **2단계 (원본 격리)**: 생성된 원본 파일을 `Assets/_Imports/` 내 해당 하위 폴더에 저장합니다.
3. **3단계 (가공 및 머티리얼)**:
   - 2D 에셋: 스프라이트 임포터 세팅 완료.
   - 3D/텍스처 에셋: `manage_material`을 호출하여 `Assets/Materials/M_[이름].mat` 머티리얼을 생성하고 텍스처를 바인딩합니다.
4. **4단계 (프리팹 조립)**:
   - 최종적으로 리소스가 부착된 독립 프리팹(`Assets/Prefabs/PF_[이름].prefab`)을 생성하여 즉시 사용 가능한 상태로 완결합니다.

---

## 3. 에이전트 준수 의무
- **`Developer`**: 리소스 제작 요청 수신 시 본 규칙의 4단계 파이프라인을 준수하여 원본 분리 및 프리팹 조립을 완결합니다.
- **`QA`**: 생성된 리소스가 `_Imports/`에 올바르게 격리되었는지, 프리팹 바인딩이 누락(Missing) 없이 정상 작동하는지 검수합니다.
