using UnityEngine;

namespace PastaDefence.Core
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("Starting Dough")]
        [SerializeField] private int startingDough = 100;

        [Header("Wave Bonuses")]
        [SerializeField] private int waveCompleteBonus = 25;
        [SerializeField] private int noLeakBonus = 50;
        [SerializeField] private int earlyWaveBonus = 15;

        public int CurrentDough { get; private set; }
        private bool hadLeaksThisWave;

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
            CurrentDough = startingDough;
            EventBus.Trigger(GameEvent.DoughChanged, CurrentDough);

            EventBus.Subscribe(GameEvent.EnemyKilled, OnEnemyKilled);
            EventBus.Subscribe(GameEvent.EnemyReachedEnd, OnEnemyReachedEnd);
            EventBus.Subscribe(GameEvent.WaveStarted, OnWaveStarted);
            EventBus.Subscribe(GameEvent.WaveCompleted, OnWaveCompleted);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe(GameEvent.EnemyKilled, OnEnemyKilled);
            EventBus.Unsubscribe(GameEvent.EnemyReachedEnd, OnEnemyReachedEnd);
            EventBus.Unsubscribe(GameEvent.WaveStarted, OnWaveStarted);
            EventBus.Unsubscribe(GameEvent.WaveCompleted, OnWaveCompleted);
        }

        public bool CanAfford(int cost)
        {
            return CurrentDough >= cost;
        }

        public bool SpendDough(int amount)
        {
            if (amount > CurrentDough) return false;
            CurrentDough -= amount;
            EventBus.Trigger(GameEvent.DoughChanged, CurrentDough);
            return true;
        }

        public void AddDough(int amount)
        {
            CurrentDough += amount;
            EventBus.Trigger(GameEvent.DoughChanged, CurrentDough);
        }

        private void OnEnemyKilled(object data)
        {
            if (data is int reward)
            {
                AddDough(reward);
            }
        }

        private void OnEnemyReachedEnd(object data)
        {
            hadLeaksThisWave = true;
        }

        private void OnWaveStarted(object data)
        {
            hadLeaksThisWave = false;
        }

        private void OnWaveCompleted(object data)
        {
            AddDough(waveCompleteBonus);

            if (!hadLeaksThisWave)
            {
                AddDough(noLeakBonus);
            }
        }

        public void GrantEarlyWaveBonus()
        {
            AddDough(earlyWaveBonus);
        }
    }
}
