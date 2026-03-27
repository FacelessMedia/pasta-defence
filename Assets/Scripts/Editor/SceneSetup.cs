#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using PastaDefence.Core;
using PastaDefence.Towers;
using PastaDefence.Enemies;
using PastaDefence.Hero;
using PastaDefence.Humor;
using PastaDefence.Audio;
using PastaDefence.UI;
using PastaDefence.Meta;

namespace PastaDefence.EditorTools
{
    public static class SceneSetup
    {
        [MenuItem("Pasta Defence/Setup Game Scene")]
        public static void SetupGameScene()
        {
            // Create a new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Remove default directional light
            var light = GameObject.Find("Directional Light");
            if (light != null) Object.DestroyImmediate(light);

            // --- Camera ---
            var cam = Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5.5f;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.85f, 0.85f, 0.83f);
                cam.transform.position = new Vector3(0, 0, -10);
            }

            // --- Background ---
            var bg = new GameObject("Background");
            var bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sortingOrder = -10;

            // Load the kitchen countertop background image
            var bgSprite = LoadBackgroundSprite();
            if (bgSprite != null)
            {
                bgSr.sprite = bgSprite;
                // Scale to fill camera view (ortho size 5.5, 16:9 = ~19.5 x 11 world units)
                float spriteW = bgSprite.bounds.size.x;
                float spriteH = bgSprite.bounds.size.y;
                float targetW = 19.5f;
                float targetH = 11f;
                bg.transform.localScale = new Vector3(targetW / spriteW, targetH / spriteH, 1f);
                Debug.Log($"[Pasta Defence] Background scaled: {bg.transform.localScale}");
            }
            else
            {
                // Fallback: create a solid white sprite so SpriteRenderer can render something
                var fallbackTex = new Texture2D(4, 4);
                Color[] pixels = new Color[16];
                for (int p = 0; p < 16; p++) pixels[p] = Color.white;
                fallbackTex.SetPixels(pixels);
                fallbackTex.Apply();
                bgSr.sprite = Sprite.Create(fallbackTex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 1f);
                bgSr.color = new Color(0.9f, 0.88f, 0.82f); // Warm counter color
                bg.transform.localScale = new Vector3(5f, 3f, 1f);
                Debug.LogWarning("[Pasta Defence] Using fallback background. Drag your image onto the Background object's Sprite field.");
            }

            // --- Path ---
            var pathParent = new GameObject("WaypointPath");
            var waypointPath = pathParent.AddComponent<WaypointPath>();

            // Waypoints matching the kitchen countertop background S-path
            // Path: START (bottom-left) → right → up → right → down → right → up → GOAL (top-right)
            Vector3[] waypointPositions = new Vector3[]
            {
                new Vector3(-8.5f, -4f, 0),   // 0: START bottom-left
                new Vector3(-4f,   -4f, 0),   // 1: along bottom, heading right
                new Vector3(-4f,    3.5f, 0),  // 2: up the left side
                new Vector3( 1f,    3.5f, 0),  // 3: right across the top
                new Vector3( 1f,   -2f, 0),   // 4: down the center
                new Vector3( 5f,   -2f, 0),   // 5: right along the middle
                new Vector3( 5f,    3.5f, 0),  // 6: up the right side
                new Vector3( 8.5f,  3.5f, 0),  // 7: GOAL top-right
            };

            Transform[] waypoints = new Transform[waypointPositions.Length];
            for (int i = 0; i < waypointPositions.Length; i++)
            {
                var wp = new GameObject($"Waypoint_{i}");
                wp.transform.SetParent(pathParent.transform);
                wp.transform.position = waypointPositions[i];
                waypoints[i] = wp.transform;

                // Invisible waypoint marker (path is drawn on background image)
                // Uncomment SpriteRenderer below to debug waypoint positions
                // var wpSr = wp.AddComponent<SpriteRenderer>();
                // wpSr.color = new Color(1f, 0f, 0f, 0.5f);
                // wpSr.sortingOrder = 100;
                // wp.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            }

            // Wire waypoints array via SerializedObject
            var pathSO = new SerializedObject(waypointPath);
            var waypointsProp = pathSO.FindProperty("waypoints");
            waypointsProp.arraySize = waypoints.Length;
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypointsProp.GetArrayElementAtIndex(i).objectReferenceValue = waypoints[i];
            }
            pathSO.ApplyModifiedProperties();

            // --- Spawn Point ---
            var spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.position = waypointPositions[0];

            // --- Tower Placement Spots ---
            var placementsParent = new GameObject("TowerPlacements");
            // Tower spots placed in open counter grid spaces around the S-path
            Vector3[] placementPositions = new Vector3[]
            {
                // Left column (between start path and left vertical)
                new Vector3(-7f,  -1.5f, 0),
                new Vector3(-7f,   1.5f, 0),
                // Inside first S-curve (open area upper-left of center)
                new Vector3(-2f,   1f, 0),
                new Vector3(-1f,  -1.5f, 0),
                new Vector3(-6f,  -2.5f, 0),
                // Between the two vertical paths (center corridor)
                new Vector3(-1.5f, -4f, 0),
                new Vector3( 3f,    1f, 0),
                new Vector3( 3f,   -0.5f, 0),
                // Inside second S-curve (open area lower-right)
                new Vector3( 3f,   -4f, 0),
                new Vector3( 7f,   -0.5f, 0),
                // Near goal area
                new Vector3( 7f,    1.5f, 0),
                new Vector3( 6.5f, -3.5f, 0),
            };

            for (int i = 0; i < placementPositions.Length; i++)
            {
                var spot = new GameObject($"Placement_{i}");
                spot.transform.SetParent(placementsParent.transform);
                spot.transform.position = placementPositions[i];

                // Visual indicator
                var spotSr = spot.AddComponent<SpriteRenderer>();
                spotSr.color = new Color(0.3f, 0.8f, 0.3f, 0.15f);
                spotSr.sortingOrder = -2;
                spot.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                // Collider for click detection
                var col = spot.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1f, 1f);

                // Highlight child
                var highlight = new GameObject("Highlight");
                highlight.transform.SetParent(spot.transform);
                highlight.transform.localPosition = Vector3.zero;
                highlight.transform.localScale = Vector3.one;
                var hlSr = highlight.AddComponent<SpriteRenderer>();
                hlSr.color = new Color(0f, 1f, 0f, 0.3f);
                hlSr.sortingOrder = -1;
                hlSr.enabled = false;

                var placement = spot.AddComponent<TowerPlacement>();
                // Wire highlight renderer
                var placeSO = new SerializedObject(placement);
                var hlProp = placeSO.FindProperty("highlightRenderer");
                hlProp.objectReferenceValue = hlSr;
                placeSO.ApplyModifiedProperties();
            }

            // --- Managers ---
            var managers = new GameObject("--- MANAGERS ---");

            // GameManager
            var gmObj = new GameObject("GameManager");
            gmObj.transform.SetParent(managers.transform);
            gmObj.AddComponent<GameManager>();

            // EconomyManager
            var econObj = new GameObject("EconomyManager");
            econObj.transform.SetParent(managers.transform);
            econObj.AddComponent<EconomyManager>();

            // WaveManager
            var waveObj = new GameObject("WaveManager");
            waveObj.transform.SetParent(managers.transform);
            var waveMgr = waveObj.AddComponent<WaveManager>();

            // Wire WaveManager references
            var waveSO = new SerializedObject(waveMgr);
            waveSO.FindProperty("spawnPoint").objectReferenceValue = spawnPoint.transform;
            waveSO.FindProperty("waypointPath").objectReferenceValue = waypointPath;

            // Load wave data assets
            var wavesProp = waveSO.FindProperty("waves");
            string[] waveNames = {
                "TheAppetizer", "NoodleNibble", "CarbLoading", "PenneAnte",
                "HolyCannelloni!", "TheThickening", "SwarmWarning",
                "TankParade", "MixedPlatter", "TheSpaghettiWestern"
            };
            wavesProp.arraySize = 10;
            for (int i = 0; i < 10; i++)
            {
                var waveData = AssetDatabase.LoadAssetAtPath<WaveData>(
                    $"Assets/ScriptableObjects/Waves/WaveData_{i + 1:D2}_{waveNames[i]}.asset");
                wavesProp.GetArrayElementAtIndex(i).objectReferenceValue = waveData;
            }
            waveSO.ApplyModifiedProperties();

            // TowerManager
            var towerMgrObj = new GameObject("TowerManager");
            towerMgrObj.transform.SetParent(managers.transform);
            var towerMgr = towerMgrObj.AddComponent<TowerManager>();

            // Wire available towers
            var towerSO = new SerializedObject(towerMgr);
            var towersProp = towerSO.FindProperty("availableTowers");
            string[] towerDataNames = {
                "RollingPin", "ChefsKnife", "PepperGrinder", "Whisk", "Colander",
                "Blowtorch", "MeatTenderizer", "FryingPan", "SpiceRack", "TipJar"
            };
            towersProp.arraySize = towerDataNames.Length;
            for (int i = 0; i < towerDataNames.Length; i++)
            {
                var td = AssetDatabase.LoadAssetAtPath<TowerData>(
                    $"Assets/ScriptableObjects/Towers/TowerData_{towerDataNames[i]}.asset");
                towersProp.GetArrayElementAtIndex(i).objectReferenceValue = td;
            }
            towerSO.ApplyModifiedProperties();

            // ObjectPool
            var poolObj = new GameObject("ObjectPool");
            poolObj.transform.SetParent(managers.transform);
            poolObj.AddComponent<ObjectPool>();

            // PunManager
            var punObj = new GameObject("PunManager");
            punObj.transform.SetParent(managers.transform);
            var punMgr = punObj.AddComponent<PunManager>();

            var punSO = new SerializedObject(punMgr);
            var quipDbProp = punSO.FindProperty("quipDatabase");
            var quipDb = AssetDatabase.LoadAssetAtPath<QuipDatabase>(
                "Assets/ScriptableObjects/Humor/QuipDatabase_Main.asset");
            quipDbProp.objectReferenceValue = quipDb;
            punSO.ApplyModifiedProperties();

            // AudioManager
            var audioObj = new GameObject("AudioManager");
            audioObj.transform.SetParent(managers.transform);
            var audioMgr = audioObj.AddComponent<AudioManager>();
            var musicSrc = audioObj.AddComponent<AudioSource>();
            musicSrc.loop = true;
            musicSrc.playOnAwake = false;
            var sfxSrc = audioObj.AddComponent<AudioSource>();
            sfxSrc.playOnAwake = false;

            var audioSO = new SerializedObject(audioMgr);
            audioSO.FindProperty("musicSource").objectReferenceValue = musicSrc;
            audioSO.FindProperty("sfxSource").objectReferenceValue = sfxSrc;
            audioSO.ApplyModifiedProperties();

            // --- Chef Hero ---
            var chefPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hero/Chef.prefab");
            if (chefPrefab != null)
            {
                var chef = (GameObject)PrefabUtility.InstantiatePrefab(chefPrefab);
                chef.transform.position = new Vector3(0, 0, 0);
            }

            // --- UI Canvas ---
            CreateGameUI();

            // --- Save Scene ---
            EnsureFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Stage_CuttingBoard_01.unity");

            Debug.Log("[Pasta Defence] Game scene 'Stage_CuttingBoard_01' fully set up! Let's cook!");
        }

        [MenuItem("Pasta Defence/Setup Main Menu Scene")]
        public static void SetupMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var light = GameObject.Find("Directional Light");
            if (light != null) Object.DestroyImmediate(light);

            var cam = Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.backgroundColor = new Color(0.12f, 0.1f, 0.08f);
            }

            // Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // Title
            var titleObj = CreateUIText(canvasObj.transform, "Title", "PASTA DEFENCE",
                new Vector2(0, 200), 72, new Color(1f, 0.9f, 0.6f));

            // Tagline
            var taglineObj = CreateUIText(canvasObj.transform, "Tagline", "Use your noodle.",
                new Vector2(0, 120), 24, new Color(0.9f, 0.8f, 0.6f));

            // Play Button
            CreateUIButton(canvasObj.transform, "PlayButton", "PLAY", new Vector2(0, 0),
                new Vector2(300, 60), new Color(0.3f, 0.6f, 0.2f));

            // Settings Button
            CreateUIButton(canvasObj.transform, "SettingsButton", "SETTINGS", new Vector2(0, -80),
                new Vector2(300, 60), new Color(0.3f, 0.35f, 0.5f));

            // Quit Button
            CreateUIButton(canvasObj.transform, "QuitButton", "QUIT", new Vector2(0, -160),
                new Vector2(300, 60), new Color(0.5f, 0.2f, 0.2f));

            // Loading Tip
            CreateUIText(canvasObj.transform, "Tip", "Did you know? Penne is the angriest pasta.",
                new Vector2(0, -300), 16, new Color(0.7f, 0.65f, 0.55f));

            // MainMenuUI script
            var menuScript = canvasObj.AddComponent<MainMenuUI>();
            var menuSO = new SerializedObject(menuScript);

            menuSO.FindProperty("titleText").objectReferenceValue =
                canvasObj.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            menuSO.FindProperty("taglineText").objectReferenceValue =
                canvasObj.transform.Find("Tagline")?.GetComponent<TextMeshProUGUI>();
            menuSO.FindProperty("tipText").objectReferenceValue =
                canvasObj.transform.Find("Tip")?.GetComponent<TextMeshProUGUI>();
            menuSO.FindProperty("playButton").objectReferenceValue =
                canvasObj.transform.Find("PlayButton")?.GetComponent<Button>();
            menuSO.FindProperty("quitButton").objectReferenceValue =
                canvasObj.transform.Find("QuitButton")?.GetComponent<Button>();
            menuSO.FindProperty("settingsButton").objectReferenceValue =
                canvasObj.transform.Find("SettingsButton")?.GetComponent<Button>();

            menuSO.ApplyModifiedProperties();

            // EventSystem
            var eventSysObj = new GameObject("EventSystem");
            eventSysObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSysObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            EnsureFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");

            Debug.Log("[Pasta Defence] Main Menu scene created!");
        }

        // --- UI Builder for Game Scene ---

        private static void CreateGameUI()
        {
            // Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // EventSystem
            var eventSysObj = new GameObject("EventSystem");
            eventSysObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSysObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            // === TOP BAR ===
            var topBar = CreateUIPanel(canvasObj.transform, "TopBar",
                new Vector2(0, -30), new Vector2(0, 60),
                new Vector2(0, 1), new Vector2(1, 1),
                new Color(0.15f, 0.12f, 0.1f, 0.9f));

            CreateUIText(topBar, "DoughText", "120 Dough",
                new Vector2(-350, 0), 28, new Color(1f, 0.9f, 0.5f));
            CreateUIText(topBar, "ServingsText", "Servings: 20",
                new Vector2(0, 0), 28, new Color(1f, 0.6f, 0.5f));
            CreateUIText(topBar, "WaveText", "Wave: 0/10",
                new Vector2(350, 0), 28, new Color(0.8f, 0.9f, 1f));

            // === TOWER SELECTION BAR (bottom) ===
            var towerBar = CreateUIPanel(canvasObj.transform, "TowerBar",
                new Vector2(0, 50), new Vector2(0, 100),
                new Vector2(0, 0), new Vector2(1, 0),
                new Color(0.15f, 0.12f, 0.1f, 0.9f));

            var towerContainer = new GameObject("TowerButtonContainer");
            towerContainer.transform.SetParent(towerBar, false);
            var containerRect = towerContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new Vector2(10, 5);
            containerRect.offsetMax = new Vector2(-10, -5);
            var hlg = towerContainer.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            // === WAVE ANNOUNCEMENT (center) ===
            var waveAnnouncementPanel = CreateUIPanel(canvasObj.transform, "WaveAnnouncement",
                new Vector2(0, 100), new Vector2(500, 80),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Color(0.2f, 0.15f, 0.1f, 0.95f));
            waveAnnouncementPanel.gameObject.SetActive(false);

            CreateUIText(waveAnnouncementPanel, "WaveAnnouncementText", "Wave 1: The Appetizer",
                Vector2.zero, 36, new Color(1f, 0.95f, 0.7f));

            // === QUIP TEXT (floating) ===
            var quipObj = CreateUIText(canvasObj.transform, "QuipText", "",
                new Vector2(0, -150), 20, new Color(1f, 0.95f, 0.8f, 0.9f));
            quipObj.gameObject.SetActive(false);

            // === START WAVE BUTTON ===
            var startWaveBtn = CreateUIButton(canvasObj.transform, "StartWaveButton",
                "Send Next Wave!", new Vector2(0, 140),
                new Vector2(250, 50), new Color(0.3f, 0.6f, 0.2f));

            // === SPEED BUTTON ===
            var speedBtn = CreateUIButton(canvasObj.transform, "SpeedButton",
                "1x", new Vector2(420, 140),
                new Vector2(80, 50), new Color(0.4f, 0.4f, 0.5f));

            // === TOWER INFO PANEL (right side) ===
            var towerInfoPanel = CreateUIPanel(canvasObj.transform, "TowerInfoPanel",
                new Vector2(-10, 0), new Vector2(280, 350),
                new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Color(0.18f, 0.15f, 0.12f, 0.95f));
            towerInfoPanel.gameObject.SetActive(false);

            CreateUIText(towerInfoPanel, "TowerName", "Rolling Pin",
                new Vector2(0, 140), 24, new Color(1f, 0.95f, 0.7f));
            CreateUIText(towerInfoPanel, "TowerPunName", "\"The Roller Coaster\"",
                new Vector2(0, 110), 16, new Color(0.9f, 0.8f, 0.6f));
            CreateUIText(towerInfoPanel, "TowerStats", "DMG: 15  SPD: 0.8  RNG: 2.0",
                new Vector2(0, 70), 16, Color.white);
            CreateUIText(towerInfoPanel, "TowerFlavor", "Flattens your problems. And your pasta.",
                new Vector2(0, 30), 14, new Color(0.8f, 0.75f, 0.6f));

            var upgradeBtn = CreateUIButton(towerInfoPanel, "UpgradeButton",
                "Warmed Up (40 Dough)", new Vector2(0, -30),
                new Vector2(240, 40), new Color(0.2f, 0.5f, 0.3f));
            var sellBtn = CreateUIButton(towerInfoPanel, "SellButton",
                "Return to Drawer (42 Dough)", new Vector2(0, -80),
                new Vector2(240, 40), new Color(0.5f, 0.25f, 0.2f));

            // === CHEF INFO (top-left) ===
            CreateUIText(canvasObj.transform, "ChefTitle", "Chef (Line Cook) Lv.1",
                new Vector2(-350, -80), 18, new Color(1f, 0.85f, 0.6f));

            // === GAME OVER / VICTORY PANEL ===
            var gameOverPanel = CreateUIPanel(canvasObj.transform, "GameOverPanel",
                Vector2.zero, new Vector2(600, 400),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Color(0.12f, 0.1f, 0.08f, 0.97f));
            gameOverPanel.gameObject.SetActive(false);

            var victoryContent = new GameObject("VictoryContent");
            victoryContent.transform.SetParent(gameOverPanel, false);
            victoryContent.AddComponent<RectTransform>();
            CreateUIText(victoryContent.transform, "VictoryTitle", "BON APPETIT!",
                new Vector2(0, 120), 48, new Color(1f, 0.9f, 0.4f));
            CreateUIText(victoryContent.transform, "VictoryQuip", "Chef's kiss. Literally.",
                new Vector2(0, 60), 20, new Color(0.9f, 0.85f, 0.7f));
            CreateUIText(victoryContent.transform, "StarRating", "Clean plate club!",
                new Vector2(0, 20), 18, new Color(1f, 0.95f, 0.6f));

            var defeatContent = new GameObject("DefeatContent");
            defeatContent.transform.SetParent(gameOverPanel, false);
            defeatContent.AddComponent<RectTransform>();
            CreateUIText(defeatContent.transform, "DefeatTitle", "YOU'VE BEEN SERVED.",
                new Vector2(0, 120), 48, new Color(0.9f, 0.3f, 0.2f));
            CreateUIText(defeatContent.transform, "DefeatQuip", "The pasta has won this round. Fork.",
                new Vector2(0, 60), 20, new Color(0.8f, 0.6f, 0.5f));

            CreateUIText(gameOverPanel, "WavesCompleted", "Waves: 10",
                new Vector2(0, -20), 18, Color.white);
            CreateUIText(gameOverPanel, "DoughEarned", "Dough Earned: 500",
                new Vector2(0, -50), 18, Color.white);

            var retryBtn = CreateUIButton(gameOverPanel, "RetryButton", "Try Again",
                new Vector2(-100, -120), new Vector2(180, 45), new Color(0.3f, 0.5f, 0.3f));
            var menuBtn = CreateUIButton(gameOverPanel, "MenuButton", "Main Menu",
                new Vector2(100, -120), new Vector2(180, 45), new Color(0.4f, 0.3f, 0.3f));

            // === PAUSE MENU ===
            var pausePanel = CreateUIPanel(canvasObj.transform, "PausePanel",
                Vector2.zero, new Vector2(500, 350),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Color(0.12f, 0.1f, 0.08f, 0.97f));
            pausePanel.gameObject.SetActive(false);

            CreateUIText(pausePanel, "PauseTitle", "Al Dente-tion: Game Paused",
                new Vector2(0, 120), 32, new Color(1f, 0.9f, 0.5f));
            CreateUIText(pausePanel, "PauseQuip", "The pasta is judging your bathroom break.",
                new Vector2(0, 70), 16, new Color(0.8f, 0.75f, 0.6f));

            var resumeBtn = CreateUIButton(pausePanel, "ResumeButton", "Resume",
                new Vector2(0, 0), new Vector2(220, 45), new Color(0.3f, 0.6f, 0.3f));
            var restartBtn = CreateUIButton(pausePanel, "RestartButton", "Restart",
                new Vector2(0, -60), new Vector2(220, 45), new Color(0.5f, 0.4f, 0.2f));
            var pauseMenuBtn = CreateUIButton(pausePanel, "MenuButton_Pause", "Main Menu",
                new Vector2(0, -120), new Vector2(220, 45), new Color(0.4f, 0.3f, 0.3f));

            // === Wire HUD Script ===
            var hudScript = canvasObj.AddComponent<GameHUD>();
            var hudSO = new SerializedObject(hudScript);

            hudSO.FindProperty("doughText").objectReferenceValue = FindTMP(canvasObj, "DoughText");
            hudSO.FindProperty("servingsText").objectReferenceValue = FindTMP(canvasObj, "ServingsText");
            hudSO.FindProperty("waveText").objectReferenceValue = FindTMP(canvasObj, "WaveText");
            hudSO.FindProperty("towerButtonContainer").objectReferenceValue = towerContainer.transform;
            hudSO.FindProperty("towerButtonPrefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/TowerButton.prefab");

            hudSO.FindProperty("towerInfoPanel").objectReferenceValue = towerInfoPanel.gameObject;
            hudSO.FindProperty("towerNameText").objectReferenceValue = FindChildTMP(towerInfoPanel, "TowerName");
            hudSO.FindProperty("towerPunNameText").objectReferenceValue = FindChildTMP(towerInfoPanel, "TowerPunName");
            hudSO.FindProperty("towerStatsText").objectReferenceValue = FindChildTMP(towerInfoPanel, "TowerStats");
            hudSO.FindProperty("towerFlavorText").objectReferenceValue = FindChildTMP(towerInfoPanel, "TowerFlavor");
            hudSO.FindProperty("upgradeButton").objectReferenceValue = FindChildButton(towerInfoPanel, "UpgradeButton");
            hudSO.FindProperty("upgradeButtonText").objectReferenceValue = FindChildTMP(towerInfoPanel.Find("UpgradeButton"), "Text");
            hudSO.FindProperty("sellButton").objectReferenceValue = FindChildButton(towerInfoPanel, "SellButton");
            hudSO.FindProperty("sellButtonText").objectReferenceValue = FindChildTMP(towerInfoPanel.Find("SellButton"), "Text");

            hudSO.FindProperty("waveAnnouncementPanel").objectReferenceValue = waveAnnouncementPanel.gameObject;
            hudSO.FindProperty("waveAnnouncementText").objectReferenceValue = FindChildTMP(waveAnnouncementPanel, "WaveAnnouncementText");
            hudSO.FindProperty("quipText").objectReferenceValue = quipObj.GetComponent<TextMeshProUGUI>();
            hudSO.FindProperty("startWaveButton").objectReferenceValue = FindButton(canvasObj, "StartWaveButton");
            hudSO.FindProperty("startWaveButtonText").objectReferenceValue = FindChildTMP(canvasObj.transform.Find("StartWaveButton"), "Text");
            hudSO.FindProperty("speedButton").objectReferenceValue = FindButton(canvasObj, "SpeedButton");
            hudSO.FindProperty("speedButtonText").objectReferenceValue = FindChildTMP(canvasObj.transform.Find("SpeedButton"), "Text");
            hudSO.FindProperty("chefTitleText").objectReferenceValue = FindTMP(canvasObj, "ChefTitle");

            hudSO.ApplyModifiedProperties();

            // === Wire GameOver Script ===
            var goScript = canvasObj.AddComponent<GameOverScreen>();
            var goSO = new SerializedObject(goScript);
            goSO.FindProperty("panel").objectReferenceValue = gameOverPanel.gameObject;
            goSO.FindProperty("victoryContent").objectReferenceValue = victoryContent;
            goSO.FindProperty("defeatContent").objectReferenceValue = defeatContent;
            goSO.FindProperty("victoryTitle").objectReferenceValue = FindChildTMP(victoryContent.transform, "VictoryTitle");
            goSO.FindProperty("victoryQuip").objectReferenceValue = FindChildTMP(victoryContent.transform, "VictoryQuip");
            goSO.FindProperty("starRatingText").objectReferenceValue = FindChildTMP(victoryContent.transform, "StarRating");
            goSO.FindProperty("defeatTitle").objectReferenceValue = FindChildTMP(defeatContent.transform, "DefeatTitle");
            goSO.FindProperty("defeatQuip").objectReferenceValue = FindChildTMP(defeatContent.transform, "DefeatQuip");
            goSO.FindProperty("wavesCompletedText").objectReferenceValue = FindChildTMP(gameOverPanel, "WavesCompleted");
            goSO.FindProperty("doughEarnedText").objectReferenceValue = FindChildTMP(gameOverPanel, "DoughEarned");
            goSO.FindProperty("retryButton").objectReferenceValue = FindChildButton(gameOverPanel, "RetryButton");
            goSO.FindProperty("menuButton").objectReferenceValue = FindChildButton(gameOverPanel, "MenuButton");
            goSO.ApplyModifiedProperties();

            // === Wire Pause Script ===
            var pauseScript = canvasObj.AddComponent<PauseMenu>();
            var pauseSO = new SerializedObject(pauseScript);
            pauseSO.FindProperty("panel").objectReferenceValue = pausePanel.gameObject;
            pauseSO.FindProperty("pauseTitleText").objectReferenceValue = FindChildTMP(pausePanel, "PauseTitle");
            pauseSO.FindProperty("pauseQuipText").objectReferenceValue = FindChildTMP(pausePanel, "PauseQuip");
            pauseSO.FindProperty("resumeButton").objectReferenceValue = FindChildButton(pausePanel, "ResumeButton");
            pauseSO.FindProperty("restartButton").objectReferenceValue = FindChildButton(pausePanel, "RestartButton");
            pauseSO.FindProperty("menuButton").objectReferenceValue = FindChildButton(pausePanel, "MenuButton_Pause");
            pauseSO.ApplyModifiedProperties();
        }

        // --- UI Helper Methods ---

        private static GameObject CreateUIText(Transform parent, string name, string text,
            Vector2 position, float fontSize, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(500, fontSize + 20);

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = false;

            return obj;
        }

        private static Transform CreateUIButton(Transform parent, string name, string text,
            Vector2 position, Vector2 size, Color bgColor)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var img = obj.AddComponent<Image>();
            img.color = bgColor;

            var btn = obj.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(bgColor.r + 0.1f, bgColor.g + 0.1f, bgColor.b + 0.1f, 1f);
            colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f, 1f);
            btn.colors = colors;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 2);
            textRect.offsetMax = new Vector2(-5, -2);

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return obj.transform;
        }

        private static Transform CreateUIPanel(Transform parent, string name,
            Vector2 position, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Color bgColor)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var img = obj.AddComponent<Image>();
            img.color = bgColor;

            return obj.transform;
        }

        // --- Finder Helpers ---

        private static TextMeshProUGUI FindTMP(GameObject root, string name)
        {
            var found = root.transform.Find(name);
            return found != null ? found.GetComponent<TextMeshProUGUI>() : null;
        }

        private static TextMeshProUGUI FindChildTMP(Transform parent, string name)
        {
            if (parent == null) return null;
            var found = parent.Find(name);
            return found != null ? found.GetComponent<TextMeshProUGUI>() : null;
        }

        private static Button FindButton(GameObject root, string name)
        {
            var found = root.transform.Find(name);
            return found != null ? found.GetComponent<Button>() : null;
        }

        private static Button FindChildButton(Transform parent, string name)
        {
            if (parent == null) return null;
            var found = parent.Find(name);
            return found != null ? found.GetComponent<Button>() : null;
        }

        private static Sprite LoadBackgroundSprite()
        {
            // Direct path to the kitchen countertop background
            string bgPath = "Assets/Art/Cartoon_kitchen_countertop_202603271721.jpeg";

            // Force import as Sprite
            var importer = AssetImporter.GetAtPath(bgPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.maxTextureSize = 4096;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(bgPath, ImportAssetOptions.ForceUpdate);
                Debug.Log($"[Pasta Defence] Reimported background as Sprite: {bgPath}");
            }
            else
            {
                Debug.LogWarning($"[Pasta Defence] Could not find importer for: {bgPath}");
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
            if (sprite != null)
            {
                Debug.Log($"[Pasta Defence] Background loaded successfully! Size: {sprite.texture.width}x{sprite.texture.height}");
                return sprite;
            }

            Debug.LogWarning("[Pasta Defence] Background sprite is null. Check Assets/Art/ for the image file.");
            return null;
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{folderName}"))
                AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
