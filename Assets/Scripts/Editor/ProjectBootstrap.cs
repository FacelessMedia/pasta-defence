#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PastaDefence.EditorTools
{
    public static class ProjectBootstrap
    {
        [MenuItem("Pasta Defence/ONE-CLICK SETUP (Do Everything)", priority = 0)]
        public static void SetupEntireProject()
        {
            Debug.Log("=== PASTA DEFENCE: FULL PROJECT SETUP ===");
            Debug.Log("Step 1/5: Creating ScriptableObject data assets...");

            ScriptableObjectFactory.CreateAll();

            Debug.Log("Step 2/5: Creating prefabs...");

            PrefabFactory.CreateAll();

            Debug.Log("Step 3/5: Re-wiring prefab references to data assets...");

            // Re-run data creation to wire prefabs that were just created
            ScriptableObjectFactory.CreateTowerData();
            ScriptableObjectFactory.CreateEnemyData();
            ScriptableObjectFactory.CreateWaveData();
            ScriptableObjectFactory.CreateMapData();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Step 4/5: Setting up Main Menu scene...");

            SceneSetup.SetupMainMenuScene();

            Debug.Log("Step 5/5: Setting up Game scene...");

            SceneSetup.SetupGameScene();

            Debug.Log("=== PASTA DEFENCE: SETUP COMPLETE! ===");
            Debug.Log("Your project is ready to play! Hit the Play button in Unity!");
            Debug.Log("Pasta la vista, setup process!");

            EditorUtility.DisplayDialog(
                "Pasta Defence - Setup Complete!",
                "Your entire project has been set up!\n\n" +
                "Created:\n" +
                "- 10 Tower data assets\n" +
                "- 15 Enemy data assets\n" +
                "- 10 Wave data assets\n" +
                "- 1 Map data asset\n" +
                "- 1 Quip Database (150+ puns)\n" +
                "- 26 Prefabs (towers, enemies, hero, UI)\n" +
                "- 2 Scenes (MainMenu + Stage_CuttingBoard_01)\n\n" +
                "Hit Play to start cooking!",
                "Let's Cook!"
            );
        }

        [MenuItem("Pasta Defence/Add Scenes to Build Settings", priority = 100)]
        public static void AddScenesToBuild()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Stage_CuttingBoard_01.unity", true)
            };

            EditorBuildSettings.scenes = scenes;
            Debug.Log("[Pasta Defence] Build settings updated with MainMenu and Stage_CuttingBoard_01!");
        }
    }
}
#endif
