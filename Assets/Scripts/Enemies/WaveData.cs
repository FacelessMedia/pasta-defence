using UnityEngine;

namespace PastaDefence.Enemies
{
    [System.Serializable]
    public class EnemyGroup
    {
        public EnemyData enemyData;
        public int count = 5;
        public float spawnInterval = 0.5f;
    }

    [CreateAssetMenu(fileName = "New Wave Data", menuName = "Pasta Defence/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Wave Info")]
        public string waveName = "Wave 1";
        public string waveQuip = "Here they come!";
        public int waveNumber = 1;

        [Header("Enemy Groups")]
        public EnemyGroup[] enemyGroups;

        [Header("Rewards")]
        public int bonusDough = 0;

        public int TotalEnemyCount
        {
            get
            {
                int total = 0;
                if (enemyGroups != null)
                {
                    foreach (var group in enemyGroups)
                        total += group.count;
                }
                return total;
            }
        }
    }
}
