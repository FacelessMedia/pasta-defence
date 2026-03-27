using UnityEngine;

namespace PastaDefence.Enemies
{
    /// <summary>
    /// Rigatoni — Tank pasta. "Built like a truck. A pasta truck."
    /// High HP, high armor, slow speed. The big boy of the pasta army.
    /// </summary>
    public class RigatoniEnemy : BaseEnemy
    {
        [Header("Rigatoni Specific")]
        [SerializeField] private float armorRegenRate = 0.5f;
        [SerializeField] private float armorRegenDelay = 3f;

        private float timeSinceLastHit;
        private float baseArmor;

        public override void Initialize(EnemyData enemyData, Core.WaypointPath waypointPath)
        {
            base.Initialize(enemyData, waypointPath);
            baseArmor = data.armor;
            timeSinceLastHit = 0f;
        }

        protected override void Update()
        {
            base.Update();

            if (isDead) return;

            // Rigatoni slowly regenerates armor after not being hit for a while
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit >= armorRegenDelay && currentArmor < baseArmor)
            {
                currentArmor = Mathf.Min(baseArmor, currentArmor + armorRegenRate * Time.deltaTime);
            }
        }

        public override float TakeDamage(float amount, string damageType = "")
        {
            timeSinceLastHit = 0f;
            return base.TakeDamage(amount, damageType);
        }
    }
}
