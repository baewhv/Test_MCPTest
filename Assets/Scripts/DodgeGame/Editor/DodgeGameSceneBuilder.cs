#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TestMCP.DodgeGame.Editor
{
    /// <summary>
    /// 낙하 회피 게임(Falling Dodge Game)의 씬 및 프리팹, UI, 컴포넌트 레퍼런스를 1클릭으로 자동 생성/연동하는 에디터 툴입니다.
    /// New Input System(InputSystemUIInputModule, PlayerInput)을 완벽 지원합니다.
    /// </summary>
    public static class DodgeGameSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/DodgeGameScene.unity";
        private const string PrefabDir = "Assets/Prefabs";
        private const string SpherePrefabPath = "Assets/Prefabs/FallingSphere.prefab";
        private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";

        [MenuItem("Tools/DodgeGame/Generate Dodge Game Scene & Prefabs", false, 1)]
        public static void GenerateSceneAndPrefabs()
        {
            // 1. 디렉토리 확인 및 생성
            if (!Directory.Exists(PrefabDir))
            {
                Directory.CreateDirectory(PrefabDir);
            }

            if (!Directory.Exists("Assets/Scenes"))
            {
                Directory.CreateDirectory("Assets/Scenes");
            }

            // 2. FallingSphere 프리팹 생성
            FallingSphere spherePrefab = CreateOrUpdateSpherePrefab();

            // 3. 신규 씬 생성
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 4. Main Camera 및 Directional Light 생성
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.14f, 0.18f, 1.0f);
            camera.orthographic = true;
            camera.orthographicSize = 6.0f;
            cameraObj.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            cameraObj.AddComponent<AudioListener>();

            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.0f;
            light.color = Color.white;
            lightObj.transform.rotation = Quaternion.Euler(50.0f, -30.0f, 0.0f);

            // 5. 바닥(Ground) 생성 (Sphere 도달 트리거)
            GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            groundObj.name = "Ground";
            groundObj.transform.position = new Vector3(0.0f, -5.5f, 0.0f);
            groundObj.transform.localScale = new Vector3(22.0f, 1.0f, 2.0f);
            BoxCollider groundCollider = groundObj.GetComponent<BoxCollider>();
            groundCollider.isTrigger = true;

            // Ground 태그 등록 확인 및 지정
            EnsureTagExists("Ground");
            groundObj.tag = "Ground";

            // 6. 플레이어 캡슐(Player) 생성
            GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObj.name = "Player";
            playerObj.transform.position = new Vector3(0.0f, -4.0f, 0.0f);
            playerObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            EnsureTagExists("Player");
            playerObj.tag = "Player";

            PlayerController playerController = playerObj.AddComponent<PlayerController>();
            playerController.Initialize(-8.0f, 8.0f);

            // New Input System PlayerInput 부착
            InputActionAsset actionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            if (actionsAsset != null)
            {
                PlayerInput playerInput = playerObj.AddComponent<PlayerInput>();
                playerInput.actions = actionsAsset;
                playerInput.defaultActionMap = "Player";
                playerInput.defaultControlScheme = "Keyboard&Mouse";
                playerInput.notificationBehavior = PlayerNotifications.SendMessages;
            }

            // 7. 구체 스포너(SphereSpawner) 생성
            GameObject spawnerObj = new GameObject("SphereSpawner");
            spawnerObj.transform.position = new Vector3(0.0f, 6.0f, 0.0f);
            SphereSpawner sphereSpawner = spawnerObj.AddComponent<SphereSpawner>();

            // 직렬화 필드 할당 (SerializedObject)
            SerializedObject spawnerSo = new SerializedObject(sphereSpawner);
            spawnerSo.FindProperty("_spherePrefab").objectReferenceValue = spherePrefab;
            spawnerSo.FindProperty("_spawnInterval").floatValue = 0.8f;
            spawnerSo.FindProperty("_spawnY").floatValue = 6.0f;
            spawnerSo.FindProperty("_minSpawnX").floatValue = -7.5f;
            spawnerSo.FindProperty("_maxSpawnX").floatValue = 7.5f;
            spawnerSo.FindProperty("_sphereFallSpeed").floatValue = 5.5f;
            spawnerSo.ApplyModifiedPropertiesWithoutUndo();

            // 8. EventSystem 생성 (InputSystemUIInputModule 사용으로 레거시 Input 제거)
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            InputSystemUIInputModule uiInputModule = eventSystemObj.AddComponent<InputSystemUIInputModule>();
            if (actionsAsset != null)
            {
                uiInputModule.actionsAsset = actionsAsset;
            }

            // 9. UI Canvas 생성 (1920 x 1080 해상도)
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // 9-1. 상단 점수 UI (ScoreText)
            GameObject scoreTextObj = new GameObject("ScoreText");
            scoreTextObj.transform.SetParent(canvasObj.transform, false);
            TextMeshProUGUI scoreTmp = scoreTextObj.AddComponent<TextMeshProUGUI>();
            scoreTmp.text = "SCORE: 0";
            scoreTmp.fontSize = 48;
            scoreTmp.alignment = TextAlignmentOptions.Center;
            scoreTmp.color = Color.white;
            RectTransform scoreRect = scoreTextObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.5f, 1.0f);
            scoreRect.anchorMax = new Vector2(0.5f, 1.0f);
            scoreRect.pivot = new Vector2(0.5f, 1.0f);
            scoreRect.anchoredPosition = new Vector2(0.0f, -30.0f);
            scoreRect.sizeDelta = new Vector2(400.0f, 80.0f);

            // 9-2. 좌측 상단 생명력 UI (LifeText)
            GameObject lifeTextObj = new GameObject("LifeText");
            lifeTextObj.transform.SetParent(canvasObj.transform, false);
            TextMeshProUGUI lifeTmp = lifeTextObj.AddComponent<TextMeshProUGUI>();
            lifeTmp.text = "LIFE: 5";
            lifeTmp.fontSize = 42;
            lifeTmp.alignment = TextAlignmentOptions.Left;
            lifeTmp.color = new Color(1.0f, 0.35f, 0.35f, 1.0f);
            RectTransform lifeRect = lifeTextObj.GetComponent<RectTransform>();
            lifeRect.anchorMin = new Vector2(0.0f, 1.0f);
            lifeRect.anchorMax = new Vector2(0.0f, 1.0f);
            lifeRect.pivot = new Vector2(0.0f, 1.0f);
            lifeRect.anchoredPosition = new Vector2(40.0f, -30.0f);
            lifeRect.sizeDelta = new Vector2(300.0f, 80.0f);

            // 9-3. 게임오버 패널 (GameOverPanel)
            GameObject gameOverPanelObj = new GameObject("GameOverPanel");
            gameOverPanelObj.transform.SetParent(canvasObj.transform, false);
            Image panelImage = gameOverPanelObj.AddComponent<Image>();
            panelImage.color = new Color(0.0f, 0.0f, 0.0f, 0.85f);
            RectTransform panelRect = gameOverPanelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // GameOver 타이틀 Text
            GameObject goTitleObj = new GameObject("TitleText");
            goTitleObj.transform.SetParent(gameOverPanelObj.transform, false);
            TextMeshProUGUI goTitleTmp = goTitleObj.AddComponent<TextMeshProUGUI>();
            goTitleTmp.text = "GAME OVER";
            goTitleTmp.fontSize = 72;
            goTitleTmp.fontStyle = FontStyles.Bold;
            goTitleTmp.alignment = TextAlignmentOptions.Center;
            goTitleTmp.color = new Color(1.0f, 0.25f, 0.25f, 1.0f);
            RectTransform goTitleRect = goTitleObj.GetComponent<RectTransform>();
            goTitleRect.anchoredPosition = new Vector2(0.0f, 120.0f);
            goTitleRect.sizeDelta = new Vector2(600.0f, 100.0f);

            // FinalScore Text
            GameObject finalScoreObj = new GameObject("FinalScoreText");
            finalScoreObj.transform.SetParent(gameOverPanelObj.transform, false);
            TextMeshProUGUI finalScoreTmp = finalScoreObj.AddComponent<TextMeshProUGUI>();
            finalScoreTmp.text = "FINAL SCORE: 0";
            finalScoreTmp.fontSize = 44;
            finalScoreTmp.alignment = TextAlignmentOptions.Center;
            finalScoreTmp.color = Color.white;
            RectTransform finalScoreRect = finalScoreObj.GetComponent<RectTransform>();
            finalScoreRect.anchoredPosition = new Vector2(0.0f, 20.0f);
            finalScoreRect.sizeDelta = new Vector2(500.0f, 70.0f);

            // Restart Button
            GameObject restartBtnObj = new GameObject("RestartButton");
            restartBtnObj.transform.SetParent(gameOverPanelObj.transform, false);
            Image btnImg = restartBtnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.6f, 1.0f, 1.0f);
            Button restartBtn = restartBtnObj.AddComponent<Button>();
            RectTransform restartBtnRect = restartBtnObj.GetComponent<RectTransform>();
            restartBtnRect.anchoredPosition = new Vector2(0.0f, -100.0f);
            restartBtnRect.sizeDelta = new Vector2(260.0f, 80.0f);

            GameObject btnTextObj = new GameObject("Text");
            btnTextObj.transform.SetParent(restartBtnObj.transform, false);
            TextMeshProUGUI btnTmp = btnTextObj.AddComponent<TextMeshProUGUI>();
            btnTmp.text = "RESTART";
            btnTmp.fontSize = 36;
            btnTmp.fontStyle = FontStyles.Bold;
            btnTmp.alignment = TextAlignmentOptions.Center;
            btnTmp.color = Color.white;
            RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.offsetMin = Vector2.zero;
            btnTextRect.offsetMax = Vector2.zero;

            gameOverPanelObj.SetActive(false);

            // UIManager 부착 및 직렬화 필드 연결
            UIManager uiManager = canvasObj.AddComponent<UIManager>();
            SerializedObject uiSo = new SerializedObject(uiManager);
            uiSo.FindProperty("_scoreText").objectReferenceValue = scoreTmp;
            uiSo.FindProperty("_lifeText").objectReferenceValue = lifeTmp;
            uiSo.FindProperty("_gameOverPanel").objectReferenceValue = gameOverPanelObj;
            uiSo.FindProperty("_finalScoreText").objectReferenceValue = finalScoreTmp;
            uiSo.FindProperty("_restartButton").objectReferenceValue = restartBtn;
            uiSo.ApplyModifiedPropertiesWithoutUndo();

            // 10. GameManager 생성 및 직렬화 필드 바인딩
            GameObject gmObj = new GameObject("GameManager");
            GameManager gm = gmObj.AddComponent<GameManager>();
            SerializedObject gmSo = new SerializedObject(gm);
            gmSo.FindProperty("_maxLife").intValue = 5;
            gmSo.FindProperty("_player").objectReferenceValue = playerController;
            gmSo.FindProperty("_spawner").objectReferenceValue = sphereSpawner;
            gmSo.FindProperty("_uiManager").objectReferenceValue = uiManager;
            gmSo.ApplyModifiedPropertiesWithoutUndo();

            // 11. 씬 파일 저장
            EditorSceneManager.SaveScene(newScene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=#00FF00>[DodgeGameSceneBuilder] New Input System 기반 씬 생성이 완료되었습니다: {ScenePath}</color>");
        }

        private static FallingSphere CreateOrUpdateSpherePrefab()
        {
            GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereObj.name = "FallingSphere";
            SphereCollider collider = sphereObj.GetComponent<SphereCollider>();
            collider.isTrigger = true;

            Rigidbody rb = sphereObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            FallingSphere sphereComp = sphereObj.AddComponent<FallingSphere>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(sphereObj, SpherePrefabPath);
            Object.DestroyImmediate(sphereObj);

            return prefab.GetComponent<FallingSphere>();
        }

        private static void EnsureTagExists(string tagName)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
                n.stringValue = tagName;
                tagManager.ApplyModifiedProperties();
            }
        }
    }
}
#endif
