using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PastaDefence.Enemies;

namespace PastaDefence.Core
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Header("Wave Data")]
        [SerializeField] private WaveData[] waves;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private WaypointPath waypointPath;

        [Header("Spawn Settings")]
        [SerializeField] private float timeBetweenSpawns = 0.5f;
        [SerializeField] private float timeBetweenGroups = 2f;

        public int ActiveEnemyCount { get; private set; }
        public int TotalWaves => waves != null ? waves.Length : 0;

        private bool isSpawning;

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
            EventBus.Subscribe(GameEvent.WaveStarted, OnWaveStarted);
            EventBus.Subscribe(GameEvent.EnemyKilled, OnEnemyDied);
            EventBus.Subscribe(GameEvent.EnemyReachedEnd, OnEnemyDied);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe(GameEvent.WaveStarted, OnWaveStarted);
            EventBus.Unsubscribe(GameEvent.EnemyKilled, OnEnemyDied);
            EventBus.Unsubscribe(GameEvent.EnemyReachedEnd, OnEnemyDied);
        }

        private void OnWaveStarted(object data)
        {
            if (data is int waveNumber)
            {
                int index = waveNumber - 1;
                if (index >= 0 && index < waves.Length)
                {
                    StartCoroutine(SpawnWave(waves[index]));
                }
            }
        }

        private IEnumerator SpawnWave(WaveData waveData)
        {
            isSpawning = true;

            foreach (var group in waveData.enemyGroups)
            {
                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemyData);
                    yield return new WaitForSeconds(
                        group.spawnInterval > 0 ? group.spawnInterval : timeBetweenSpawns
                    );
                }
                yield return new WaitForSeconds(timeBetweenGroups);
            }

            isSpawning = false;
        }

        private void SpawnEnemy(EnemyData enemyData)
        {
            if (enemyData == null || enemyData.prefab == null) return;

            GameObject enemyObj = ObjectPool.Instance.Get(
                enemyData.prefab,
                spawnPoint.position,
                Quaternion.identity
            );

            if (enemyObj.TryGetComponent<BaseEnemy>(out var enemy))
            {
                enemy.Initialize(enemyData, waypointPath);
                ActiveEnemyCount++;
            }
        }

        private void OnEnemyDied(object data)
        {
            ActiveEnemyCount--;
            if (ActiveEnemyCount <= 0 && !isSpawning)
            {
                ActiveEnemyCount = 0;
                GameManager.Instance.OnWaveComplete();
            }
        }
    }
}
