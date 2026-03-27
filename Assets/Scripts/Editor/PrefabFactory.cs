#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using PastaDefence.Towers;
using PastaDefence.Enemies;
using PastaDefence.UI;

namespace PastaDefence.EditorTools
{
    public static class PrefabFactory
    {
        private const string PREFAB_ROOT = "Assets/Prefabs";
        private const string TOWER_PATH = PREFAB_ROOT + "/Towers";
        private const string ENEMY_PATH = PREFAB_ROOT + "/Enemies";
        private const string HERO_PATH = PREFAB_ROOT + "/Hero";
        private const string UI_PATH = PREFAB_ROOT + "/UI";

        [MenuItem("Pasta Defence/Create All Prefabs")]
        public static void CreateAll()
        {
            EnsureFolders();
            CreateTowerPrefabs();
            CreateEnemyPrefabs();
            CreateHeroPrefab();
            CreateUIButtonPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Pasta Defence] All prefabs created! Ready to cook!");
        }

        [MenuItem("Pasta Defence/Create Tower Prefabs")]
        public static void CreateTowerPrefabs()
        {
            EnsureFolders();

            CreateTowerPrefab("RollingPin", new Color(0.82f, 0.71f, 0.55f), typeof(RollingPinTower));
            CreateTowerPrefab("ChefsKnife", new Color(0.8f, 0.8f, 0.85f), typeof(ChefsKnifeTower));
            CreateTowerPrefab("PepperGrinder", new Color(0.3f, 0.3f, 0.3f), typeof(PepperGrinderTower));
            CreateTowerPrefab("Whisk", new Color(0.85f, 0.85f, 0.9f), typeof(BaseTower));
            CreateTowerPrefab("Colander", new Color(0.7f, 0.7f, 0.75f), typeof(BaseTower));
            CreateTowerPrefab("Blowtorch", new Color(0.9f, 0.3f, 0.1f), typeof(BaseTower));
            CreateTowerPrefab("MeatTenderizer", new Color(0.6f, 0.55f, 0.5f), typeof(BaseTower));
            CreateTowerPrefab("FryingPan", new Color(0.25f, 0.25f, 0.28f), typeof(BaseTower));
            CreateTowerPrefab("SpiceRack", new Color(0.85f, 0.5f, 0.2f), typeof(BaseTower));
            CreateTowerPrefab("TipJar", new Color(0.7f, 0.9f, 0.7f), typeof(BaseTower));

            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] 10 Tower prefabs created!");
        }

        [MenuItem("Pasta Defence/Create Enemy Prefabs")]
        public static void CreateEnemyPrefabs()
        {
            EnsureFolders();

            CreateEnemyPrefab("Penne", new Color(1f, 0.9f, 0.6f), 0.4f, typeof(PenneEnemy));
            CreateEnemyPrefab("Spaghetti", new Color(1f, 0.95f, 0.7f), 0.25f, typeof(SpaghettiEnemy));
            CreateEnemyPrefab("Rigatoni", new Color(0.95f, 0.85f, 0.55f), 0.6f, typeof(RigatoniEnemy));
            CreateEnemyPrefab("Farfalle", new Color(1f, 0.92f, 0.65f), 0.45f, typeof(BaseEnemy));
            CreateEnemyPrefab("Ravioli", new Color(1f, 0.88f, 0.5f), 0.5f, typeof(BaseEnemy));
            CreateEnemyPrefab("Lasagna", new Color(0.85f, 0.45f, 0.2f), 0.9f, typeof(BaseEnemy));
            CreateEnemyPrefab("Fusilli", new Color(1f, 0.93f, 0.65f), 0.35f, typeof(BaseEnemy));
            CreateEnemyPrefab("Orzo", new Color(1f, 0.95f, 0.75f), 0.2f, typeof(BaseEnemy));
            CreateEnemyPrefab("Macaroni", new Color(1f, 0.88f, 0.4f), 0.35f, typeof(BaseEnemy));
            CreateEnemyPrefab("AngelHair", new Color(1f, 0.97f, 0.8f), 0.2f, typeof(BaseEnemy));
            CreateEnemyPrefab("Tortellini", new Color(0.95f, 0.85f, 0.5f), 0.35f, typeof(BaseEnemy));
            CreateEnemyPrefab("Cannelloni", new Color(0.9f, 0.75f, 0.45f), 0.55f, typeof(BaseEnemy));
            CreateEnemyPrefab("Gnocchi", new Color(1f, 0.92f, 0.75f), 0.4f, typeof(BaseEnemy));
            CreateEnemyPrefab("Linguine", new Color(1f, 0.95f, 0.7f), 0.3f, typeof(BaseEnemy));
            CreateEnemyPrefab("StuffedShell", new Color(0.95f, 0.8f, 0.45f), 0.55f, typeof(BaseEnemy));

            // Now wire prefab references into EnemyData assets
            WirePrefabsToEnemyData();

            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] 15 Enemy prefabs created!");
        }

        [MenuItem("Pasta Defence/Create Hero Prefab")]
        public static void CreateHeroPrefab()
        {
            EnsureFolders();
            string path = HERO_PATH + "/Chef.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("Chef");

            // Sprite
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("Chef", new Color(1f, 1f, 1f));
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 5;
            go.transform.localScale = Vector3.one * 0.7f;

            // Collider for interactions
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = true;

            // Rigidbody
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Chef script
            go.AddComponent<Hero.ChefHero>();

            // Ability child objects
            var orderUp = new GameObject("Ability_OrderUp");
            orderUp.transform.SetParent(go.transform);
            orderUp.transform.localPosition = Vector3.zero;
            orderUp.AddComponent<Hero.ChefAbility>();

            var sauceBoss = new GameObject("Ability_SauceBoss");
            sauceBoss.transform.SetParent(go.transform);
            sauceBoss.transform.localPosition = Vector3.zero;
            sauceBoss.AddComponent<Hero.ChefAbility>();

            var chefsKiss = new GameObject("Ability_ChefsKiss");
            chefsKiss.transform.SetParent(go.transform);
            chefsKiss.transform.localPosition = Vector3.zero;
            chefsKiss.AddComponent<Hero.ChefAbility>();

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);

            Debug.Log("[Pasta Defence] Chef Hero prefab created!");
        }

        public static void CreateUIButtonPrefab()
        {
            EnsureFolders();
            string path = UI_PATH + "/TowerButton.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("TowerButton");

            // RectTransform
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 80);

            // Image background
            var img = go.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0.2f, 0.18f, 0.15f, 0.9f);

            // Button
            go.AddComponent<UnityEngine.UI.Button>();

            // Icon child
            var iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(go.transform, false);
            var iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.35f);
            iconRect.anchorMax = new Vector2(0.35f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            var iconImg = iconObj.AddComponent<UnityEngine.UI.Image>();
            iconImg.color = Color.white;

            // Text child
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(go.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.05f, 0.05f);
            textRect.anchorMax = new Vector2(0.95f, 0.95f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = "Tower\n50 Dough";
            tmp.fontSize = 10;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.95f, 0.8f);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);

            Debug.Log("[Pasta Defence] UI Tower Button prefab created!");
        }

        // --- Helpers ---

        private static void CreateTowerPrefab(string name, Color color, System.Type towerScript)
        {
            string path = TOWER_PATH + $"/Tower_{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject($"Tower_{name}");

            // Sprite
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite(name, color);
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 3;
            go.transform.localScale = Vector3.one * 0.5f;

            // Tower script
            go.AddComponent(towerScript);

            // Range indicator child
            var rangeObj = new GameObject("RangeIndicator");
            rangeObj.transform.SetParent(go.transform);
            rangeObj.transform.localPosition = Vector3.zero;
            var rangeSr = rangeObj.AddComponent<SpriteRenderer>();
            rangeSr.color = new Color(0f, 1f, 0f, 0.1f);
            rangeSr.sortingOrder = 1;
            rangeSr.enabled = false;

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);

            // Wire prefab into TowerData
            WirePrefabToTowerData(name, path);
        }

        private static void CreateEnemyPrefab(string name, Color color, float scale, System.Type enemyScript)
        {
            string path = ENEMY_PATH + $"/Enemy_{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject($"Enemy_{name}");
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Default");

            // Sprite
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite(name, color);
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 4;
            go.transform.localScale = Vector3.one * scale;

            // Collider
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            col.isTrigger = true;

            // Rigidbody
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Enemy script
            go.AddComponent(enemyScript);

            // Health bar child
            var hpBar = new GameObject("HealthBar");
            hpBar.transform.SetParent(go.transform);
            hpBar.transform.localPosition = new Vector3(0, 0.6f, 0);

            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(hpBar.transform);
            bgObj.transform.localPosition = Vector3.zero;
            bgObj.transform.localScale = new Vector3(1f, 0.15f, 1f);
            var bgSr = bgObj.AddComponent<SpriteRenderer>();
            bgSr.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            bgSr.sortingOrder = 10;

            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(hpBar.transform);
            fillObj.transform.localPosition = Vector3.zero;
            fillObj.transform.localScale = new Vector3(1f, 0.12f, 1f);
            var fillSr = fillObj.AddComponent<SpriteRenderer>();
            fillSr.color = Color.green;
            fillSr.sortingOrder = 11;

            var hpScript = hpBar.AddComponent<EnemyHealthBar>();

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void WirePrefabToTowerData(string name, string prefabPath)
        {
            string dataPath = $"Assets/ScriptableObjects/Towers/TowerData_{name}.asset";
            var data = AssetDatabase.LoadAssetAtPath<TowerData>(dataPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (data != null && prefab != null)
            {
                data.prefab = prefab;
                EditorUtility.SetDirty(data);
            }
        }

        private static void WirePrefabsToEnemyData()
        {
            string[] names = {
                "Penne", "Spaghetti", "Rigatoni", "Farfalle", "Ravioli",
                "Lasagna", "Fusilli", "Orzo", "Macaroni", "AngelHair",
                "Tortellini", "Cannelloni", "Gnocchi", "Linguine", "StuffedShell"
            };

            foreach (var name in names)
            {
                string dataPath = $"Assets/ScriptableObjects/Enemies/EnemyData_{name}.asset";
                string prefabPath = $"Assets/Prefabs/Enemies/Enemy_{name}.prefab";

                var data = AssetDatabase.LoadAssetAtPath<EnemyData>(dataPath);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (data != null && prefab != null)
                {
                    data.prefab = prefab;
                    EditorUtility.SetDirty(data);
                }
            }
        }

        private static Sprite CreatePlaceholderSprite(string name, Color color)
        {
            // Create a simple colored square placeholder sprite
            string spritePath = $"Assets/Art/Placeholders/Sprite_{name}.png";
            EnsureArtFolders();

            if (AssetDatabase.LoadAssetAtPath<Sprite>(spritePath) != null)
                return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Simple shape: rounded rectangle with border
                    bool isBorder = x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
                    bool isCorner = (x < 6 && y < 6) || (x < 6 && y >= size - 6) ||
                                    (x >= size - 6 && y < 6) || (x >= size - 6 && y >= size - 6);

                    if (isCorner)
                        pixels[y * size + x] = Color.clear;
                    else if (isBorder)
                        pixels[y * size + x] = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 1f);
                    else
                        pixels[y * size + x] = color;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(
                System.IO.Path.Combine(Application.dataPath, spritePath.Replace("Assets/", "")),
                pngData
            );
            Object.DestroyImmediate(tex);

            AssetDatabase.Refresh();

            // Set texture import settings to Sprite
            var importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 64;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "Prefabs");
            EnsureFolder(PREFAB_ROOT, "Towers");
            EnsureFolder(PREFAB_ROOT, "Enemies");
            EnsureFolder(PREFAB_ROOT, "Hero");
            EnsureFolder(PREFAB_ROOT, "UI");
        }

        private static void EnsureArtFolders()
        {
            EnsureFolder("Assets", "Art");
            EnsureFolder("Assets/Art", "Placeholders");
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{folderName}"))
                AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
