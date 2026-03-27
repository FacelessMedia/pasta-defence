using System.Collections.Generic;
using UnityEngine;
using PastaDefence.Core;

namespace PastaDefence.Humor
{
    /// <summary>
    /// Central hub for all humor delivery. Listens to game events and
    /// serves up random quips, avoiding repeats within a session.
    /// "The real MVP — Most Valuable Punner."
    /// </summary>
    public class PunManager : MonoBehaviour
    {
        public static PunManager Instance { get; private set; }

        [SerializeField] private QuipDatabase quipDatabase;

        // Track recently used quips to avoid repeats
        private readonly Dictionary<QuipCategory, List<int>> recentlyUsed = new();
        private const int MAX_RECENT = 5;

        // Event for UI to display quips
        public delegate void QuipEvent(string quip, QuipCategory category);
        public event QuipEvent OnQuipTriggered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (quipDatabase == null)
            {
                Debug.LogWarning("[PunManager] No QuipDatabase assigned! The puns will not flow.");
                return;
            }

            EventBus.Subscribe(GameEvent.EnemyKilled, _ => TriggerQuip(QuipCategory.EnemyKill, 0.15f));
            EventBus.Subscribe(GameEvent.WaveStarted, _ => TriggerQuip(QuipCategory.WaveStart));
            EventBus.Subscribe(GameEvent.WaveCompleted, _ => TriggerQuip(QuipCategory.WaveComplete));
            EventBus.Subscribe(GameEvent.BossSpawned, _ => TriggerQuip(QuipCategory.BossSpawn));
            EventBus.Subscribe(GameEvent.TowerPlaced, _ => TriggerQuip(QuipCategory.TowerPlaced, 0.3f));
            EventBus.Subscribe(GameEvent.TowerSold, _ => TriggerQuip(QuipCategory.TowerSold));
            EventBus.Subscribe(GameEvent.Victory, _ => TriggerQuip(QuipCategory.Victory));
            EventBus.Subscribe(GameEvent.Defeat, _ => TriggerQuip(QuipCategory.GameOver));
            EventBus.Subscribe(GameEvent.ChefDowned, _ => TriggerQuip(QuipCategory.ChefDowned));
            EventBus.Subscribe(GameEvent.ChefAbilityUsed, _ => TriggerQuip(QuipCategory.ChefAbility, 0.4f));
        }

        /// <summary>
        /// Get a random non-repeating quip from a category.
        /// </summary>
        public string GetQuip(QuipCategory category)
        {
            if (quipDatabase == null) return "";
            return quipDatabase.GetRandomQuip(category);
        }

        /// <summary>
        /// Trigger a quip event. The probability parameter controls how often
        /// quips fire for frequent events (e.g., enemy kills).
        /// </summary>
        public void TriggerQuip(QuipCategory category, float probability = 1f)
        {
            if (quipDatabase == null) return;
            if (probability < 1f && Random.value > probability) return;

            string quip = quipDatabase.GetRandomQuip(category);
            if (!string.IsNullOrEmpty(quip))
            {
                OnQuipTriggered?.Invoke(quip, category);
            }
        }

        /// <summary>
        /// Get a loading screen tip.
        /// </summary>
        public string GetLoadingTip()
        {
            return GetQuip(QuipCategory.LoadingTip);
        }

        /// <summary>
        /// Get a pause menu quip.
        /// </summary>
        public string GetPauseQuip()
        {
            return GetQuip(QuipCategory.PauseMenu);
        }
    }
}
