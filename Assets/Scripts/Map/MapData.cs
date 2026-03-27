using UnityEngine;

namespace PastaDefence.Map
{
    public enum MapTheme
    {
        CuttingBoard,
        Stovetop,
        Sink,
        Pantry,
        Fridge,
        Oven
    }

    [CreateAssetMenu(fileName = "New Map Data", menuName = "Pasta Defence/Map Data")]
    public class MapData : ScriptableObject
    {
        [Header("Map Info")]
        public string mapName = "The Cutting Board";
        public string punSubtitle = "Where careers get started... and pasta gets ended.";
        public MapTheme theme = MapTheme.CuttingBoard;
        public int worldNumber = 1;
        public int stageNumber = 1;
        public Sprite mapPreview;

        [Header("Gameplay")]
        public int startingDough = 100;
        public int startingServings = 20;
        public int totalWaves = 10;

        [Header("Wave Data")]
        public Enemies.WaveData[] waves;

        [Header("Environmental Effects")]
        public bool hasBurnerHazards;
        public bool hasWaterZones;
        public bool hasDarkAreas;
        public bool hasColdZones;
        public bool hasHeatZones;

        [Header("Difficulty Multipliers")]
        [Tooltip("Applied to enemy HP at this stage")]
        public float enemyHPMultiplier = 1f;
        [Tooltip("Applied to enemy speed at this stage")]
        public float enemySpeedMultiplier = 1f;

        [Header("Unlock Requirements")]
        public int requiredStarsToUnlock = 0;
        public int requiredStageToUnlock = 0;

        [Header("Star Thresholds")]
        [Tooltip("Max servings lost for 2 stars")]
        public int twoStarMaxLeaks = 0;
        [Tooltip("Max dough spent percentage for 3 stars")]
        [Range(0f, 1f)] public float threeStarBudgetPercent = 0.8f;
    }
}
