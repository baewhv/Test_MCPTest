# AI 리소스 생성 및 프로토타입 규칙 (Asset Generation & Prototype Rules)

이 문서는 토큰 및 API 비용을 극대화하여 절약하는 **프로토타입/더미 리소스 기본 원칙**과, 사용자의 명시적 요청 시 나노바나나/UnityMCP를 가동하는 **AI 리소스 정식 제작 파이프라인**을 규정합니다.

---

## 1. 토큰 절약형 프로토타입/더미 리소스 기본 원칙 (Token-Saving Default Policy)

기능 구현 및 템플릿 개발 단계에서는 불필요한 토큰/비용 소모를 방지하기 위해 **항상 아래의 초경량 더미 사양을 기본(Default)으로 사용**합니다:

1. **2D/3D 외형 기본 사양 (Primitive First)**:
   - 화려한 AI 생성 이미지나 3D 모델 대신, **유니티 기본 프리미티브(Primitive: Capsule, Cube, Sphere, Cylinder)** 또는 단순 기본 단색 도형 스프라이트로 기호화하여 구현합니다.
   - 예시:
     - 검 / 둔기 / 무기: `Capsule` 형태의 단순 막대기 오브젝트
     - 플레이어 / 몬스터: `Capsule` 또는 `Cube` 기본 도형에 단색 머티리얼 적용
     - 코인 / 수집 아이템: 얇은 `Cylinder` 또는 노란색 `Sphere`
2. **사운드 기본 사양 (Simple Tone)**:
   - 복잡한 배경음악/효과음 AI 생성을 지양하고, 단순 단음(Simple Beep/Tone) 형태나 무음/더미 오디오 소스로 처리합니다.
3. **3D 모델링 AI 생성 엄격 제한**:
   - 테스트 및 템플릿 개발 단계에서는 **3D 모델 AI 생성을 일체 진행하지 않습니다** (토큰 및 시간 낭비 방지).
4. **정식 AI 리소스 제작 가동 조건**:
   - 사용자가 **"실제 리소스 제작해줘"**, **"고품질 에셋으로 만들어줘"**라고 명시적으로 요청한 경우에만 `Artist` 에이전트가 가동되어 실제 AI 생성을 진행합니다.

---

## 2. 정식 AI 리소스 생성 도구 및 규격 (사용자 명시 요청 시)

### ① 2D 스프라이트 및 텍스처 (2D Sprites & Textures)
- **도구**: Antigravity `generate_image` (NanoBanana / Imagen) 또는 UnityMCP `generate_image`
- **저장 위치**: 반드시 **`Assets/_Imports/Textures/`** 또는 **`Assets/_Imports/Sprites/`**에 저장합니다.
- **네이밍**: `T_[이름].png` (텍스처), `SP_[이름].png` (스프라이트)
- **임포트 후처리**: UnityMCP `manage_texture`로 `Sprite (2D and UI)` 설정.

### ② 오디오 및 사운드 효과 (Audio: BGM & SFX)
- **도구**: UnityMCP `generate_audio` (fal-ai/stable-audio-25, cassetteai)
- **저장 위치**: 반드시 **`Assets/_Imports/Audio/`**에 저장합니다.
- **네이밍**: `BGM_[이름].wav` (배경음), `SFX_[이름].wav` (효과음)

### ③ 3D 모델 및 메시 (3D Models & Meshes)
- **도구**: UnityMCP `generate_model` (Tripo, Meshy)
- **저장 위치**: 반드시 **`Assets/_Imports/Models/`**에 저장합니다.
- **네이밍**: `M_[이름].[ext]`

---

## 3. 리소스 가공 및 프리팹 완결 4단계 파이프라인

정식 AI 리소스 제작 시 아래 4단계를 거쳐 완제품으로 가공합니다:

```
[1단계: AI 원본 생성] ➔ [2단계: _Imports 격리] ➔ [3단계: 머티리얼/스프라이트 세팅] ➔ [4단계: 프리팹 완제품 조립]
 (사용자 명시 요청 시)   (Audio/Textures/Models)    (Sprite 전환, M_*.mat 생성)     (PF_*.prefab 완성)
```

---

## 4. 에이전트 준수 의무
- **`Developer`**: 평소 기능 개발 시 프리미티브(Capsule, Cube 등) 기반의 경량 더미 리소스로 프리팹을 조립합니다.
- **`Artist`**: 사용자의 명시적인 리소스 제작 요청이 있을 때만 AI 생성 파이프라인을 가동합니다.
- **`QA`**: 더미 리소스 상태에서도 게임 로직 및 코어루프가 정상 동작하는지 우선 검증합니다.
