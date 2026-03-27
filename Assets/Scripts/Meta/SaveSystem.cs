using System.IO;
using UnityEngine;

namespace PastaDefence.Meta
{
    [System.Serializable]
    public class SaveData
    {
        // Progression
        public int highestStageCompleted;
        public int totalStarsEarned;
        public int[] stageStars; // Stars per stage index

        // Chef
        public int chefLevel;
        public int chefXP;

        // Economy
        public int totalDough;
        public int totalParmesan;

        // Skill tree (node IDs that are unlocked)
        public string[] unlockedSkillIds;

        // Recipes (unlocked recipe IDs)
        public string[] unlockedRecipeIds;
        public string[] equippedRecipeIds;

        // Kitchen upgrades
        public int sharpenStationLevel;
        public int spiceCabinetLevel;
        public int recipeBoxLevel;
        public int trophyShelfLevel;

        // Stats for Pastapedia
        public int totalPastaKilled;
        public int[] pastaKillCounts; // Per enemy type index

        // Settings
        public float musicVolume;
        public float sfxVolume;
    }

    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private const string SAVE_FILE = "pasta_defence_save.json";
        public SaveData CurrentSave { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Load();
        }

        public string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SAVE_FILE);
        }

        public void Save()
        {
            if (CurrentSave == null) CurrentSave = new SaveData();

            string json = JsonUtility.ToJson(CurrentSave, true);
            File.WriteAllText(GetSavePath(), json);
        }

        public void Load()
        {
            string path = GetSavePath();

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                CurrentSave = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                CurrentSave = CreateNewSave();
            }
        }

        public void ResetSave()
        {
            CurrentSave = CreateNewSave();
            Save();
        }

        private SaveData CreateNewSave()
        {
            return new SaveData
            {
                highestStageCompleted = 0,
                totalStarsEarned = 0,
                stageStars = new int[40],
                chefLevel = 1,
                chefXP = 0,
                totalDough = 0,
                totalParmesan = 0,
                unlockedSkillIds = new string[0],
                unlockedRecipeIds = new string[0],
                equippedRecipeIds = new string[0],
                sharpenStationLevel = 0,
                spiceCabinetLevel = 0,
                recipeBoxLevel = 0,
                trophyShelfLevel = 0,
                totalPastaKilled = 0,
                pastaKillCounts = new int[15],
                musicVolume = 0.5f,
                sfxVolume = 0.7f
            };
        }

        public void RecordStageComplete(int stageIndex, int stars)
        {
            if (CurrentSave == null) return;

            if (stageIndex > CurrentSave.highestStageCompleted)
                CurrentSave.highestStageCompleted = stageIndex;

            if (CurrentSave.stageStars != null && stageIndex < CurrentSave.stageStars.Length)
            {
                if (stars > CurrentSave.stageStars[stageIndex])
                {
                    int diff = stars - CurrentSave.stageStars[stageIndex];
                    CurrentSave.stageStars[stageIndex] = stars;
                    CurrentSave.totalStarsEarned += diff;
                }
            }

            Save();
        }

        public void RecordPastaKill(int enemyTypeIndex)
        {
            if (CurrentSave == null) return;

            CurrentSave.totalPastaKilled++;
            if (CurrentSave.pastaKillCounts != null && enemyTypeIndex < CurrentSave.pastaKillCounts.Length)
                CurrentSave.pastaKillCounts[enemyTypeIndex]++;
        }
    }
}
