using System.Collections.Generic;
using UnityEngine;
using PastaDefence.Core;
using PastaDefence.Enemies;

namespace PastaDefence.Towers
{
    public class BaseTower : MonoBehaviour
    {
        [Header("Tower Data")]
        [SerializeField] protected TowerData data;

        // Runtime stats (modified by upgrades)
        protected float currentDamage;
        protected float currentAttackSpeed;
        protected float currentRange;
        protected float currentAoeRadius;
        protected float currentSlowAmount;
        protected float currentSlowDuration;
        protected float currentStunChance;
        protected float currentStunDuration;
        protected float currentArmorReduction;
        protected float currentDotDamage;
        protected float currentDotDuration;
        protected float currentKnockbackForce;

        // State
        protected float attackTimer;
        protected BaseEnemy currentTarget;
        protected int upgradeLevel = 0;
        protected TargetingMode targetingMode;

        // Components
        protected SpriteRenderer spriteRenderer;
        protected CircleCollider2D rangeCollider;

        // Enemies in range
        protected List<BaseEnemy> enemiesInRange = new();

        public TowerData Data => data;
        public int UpgradeLevel => upgradeLevel;
        public float CurrentRange => currentRange;
        public int SellValue => Mathf.RoundToInt(GetTotalInvestment() * 0.7f);

        private int totalInvested;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Initialize(TowerData towerData)
        {
            data = towerData;
            ApplyBaseStats();
            totalInvested = data.cost;
            targetingMode = data.defaultTargeting;
        }

        protected void ApplyBaseStats()
        {
            currentDamage = data.damage;
            currentAttackSpeed = data.attackSpeed;
            currentRange = data.range;
            currentAoeRadius = data.aoeRadius;
            currentSlowAmount = data.slowAmount;
            currentSlowDuration = data.slowDuration;
            currentStunChance = data.stunChance;
            currentStunDuration = data.stunDuration;
            currentArmorReduction = data.armorReduction;
            currentDotDamage = data.dotDamage;
            currentDotDuration = data.dotDuration;
            currentKnockbackForce = data.knockbackForce;
        }

        protected virtual void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.WaveActive) return;

            attackTimer -= Time.deltaTime;
            CleanEnemyList();
            AcquireTarget();

            if (currentTarget != null && attackTimer <= 0f)
            {
                Attack(currentTarget);
                attackTimer = 1f / currentAttackSpeed;
            }
        }

        protected void CleanEnemyList()
        {
            enemiesInRange.RemoveAll(e => e == null || e.IsDead || !e.gameObject.activeInHierarchy);

            if (currentTarget != null && (currentTarget.IsDead || !currentTarget.gameObject.activeInHierarchy))
                currentTarget = null;

            // Check range for remaining enemies
            enemiesInRange.RemoveAll(e =>
                Vector3.Distance(transform.position, e.transform.position) > currentRange
            );
        }

        protected virtual void AcquireTarget()
        {
            if (currentTarget != null && !currentTarget.IsDead &&
                Vector3.Distance(transform.position, currentTarget.transform.position) <= currentRange)
                return;

            currentTarget = null;

            if (enemiesInRange.Count == 0)
            {
                // Scan for enemies in range
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange);
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                    {
                        if (!data.canTargetFlying && enemy.data.primaryAbility == EnemyAbility.Flying)
                            continue;
                        if (!enemiesInRange.Contains(enemy))
                            enemiesInRange.Add(enemy);
                    }
                }
            }

            if (enemiesInRange.Count == 0) return;

            currentTarget = targetingMode switch
            {
                TargetingMode.First => GetFirstEnemy(),
                TargetingMode.Last => GetLastEnemy(),
                TargetingMode.Strongest => GetStrongestEnemy(),
                TargetingMode.Closest => GetClosestEnemy(),
                TargetingMode.Weakest => GetWeakestEnemy(),
                _ => GetFirstEnemy()
            };
        }

        protected virtual void Attack(BaseEnemy target)
        {
            if (data.isAreaOfEffect)
            {
                AttackAoE(target.transform.position);
            }
            else
            {
                DealDamage(target);
            }

            OnAttack(target);
        }

        protected virtual void DealDamage(BaseEnemy target)
        {
            string dmgType = data.damageType.ToString();
            target.TakeDamage(currentDamage, dmgType);

            ApplyEffects(target);
        }

        protected virtual void AttackAoE(Vector3 center)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, currentAoeRadius);
            string dmgType = data.damageType.ToString();

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                {
                    enemy.TakeDamage(currentDamage, dmgType);
                    ApplyEffects(enemy);
                }
            }
        }

        protected virtual void ApplyEffects(BaseEnemy target)
        {
            if (currentSlowAmount > 0f && currentSlowDuration > 0f)
                target.ApplySlow(currentSlowAmount, currentSlowDuration);

            if (currentStunChance > 0f && Random.value < currentStunChance)
                target.ApplyStun(currentStunDuration);

            if (currentArmorReduction > 0f)
                target.ReduceArmor(currentArmorReduction);

            if (currentKnockbackForce > 0f)
                target.Knockback(currentKnockbackForce);
        }

        protected virtual void OnAttack(BaseEnemy target)
        {
            // Override for custom attack visuals/sounds
        }

        public virtual bool CanUpgrade()
        {
            if (data.upgradePath == null || upgradeLevel >= data.upgradePath.Length)
                return false;

            return EconomyManager.Instance.CanAfford(data.upgradePath[upgradeLevel].cost);
        }

        public virtual bool Upgrade()
        {
            if (!CanUpgrade()) return false;

            var upgrade = data.upgradePath[upgradeLevel];

            if (!EconomyManager.Instance.SpendDough(upgrade.cost))
                return false;

            totalInvested += upgrade.cost;

            // Apply upgrade bonuses
            currentDamage += upgrade.damageBonus;
            currentAttackSpeed += upgrade.attackSpeedBonus;
            currentRange += upgrade.rangeBonus;
            currentAoeRadius += upgrade.aoeRadiusBonus;
            currentSlowAmount += upgrade.slowAmountBonus;
            currentSlowDuration += upgrade.slowDurationBonus;
            currentStunChance += upgrade.stunChanceBonus;
            currentDotDamage += upgrade.dotDamageBonus;
            currentKnockbackForce += upgrade.knockbackForceBonus;

            upgradeLevel++;

            EventBus.Trigger(GameEvent.TowerUpgraded, this);
            return true;
        }

        public void Sell()
        {
            EconomyManager.Instance.AddDough(SellValue);
            EventBus.Trigger(GameEvent.TowerSold, this);
            Destroy(gameObject);
        }

        public void SetTargetingMode(TargetingMode mode)
        {
            targetingMode = mode;
        }

        protected int GetTotalInvestment()
        {
            return totalInvested;
        }

        // Targeting helpers
        private BaseEnemy GetFirstEnemy()
        {
            BaseEnemy first = null;
            float maxDist = -1f;
            foreach (var e in enemiesInRange)
            {
                if (e.DistanceTraveled > maxDist)
                {
                    maxDist = e.DistanceTraveled;
                    first = e;
                }
            }
            return first;
        }

        private BaseEnemy GetLastEnemy()
        {
            BaseEnemy last = null;
            float minDist = float.MaxValue;
            foreach (var e in enemiesInRange)
            {
                if (e.DistanceTraveled < minDist)
                {
                    minDist = e.DistanceTraveled;
                    last = e;
                }
            }
            return last;
        }

        private BaseEnemy GetStrongestEnemy()
        {
            BaseEnemy strongest = null;
            float maxHP = -1f;
            foreach (var e in enemiesInRange)
            {
                if (e.HPPercent * e.data.maxHP > maxHP)
                {
                    maxHP = e.HPPercent * e.data.maxHP;
                    strongest = e;
                }
            }
            return strongest;
        }

        private BaseEnemy GetClosestEnemy()
        {
            BaseEnemy closest = null;
            float minDist = float.MaxValue;
            foreach (var e in enemiesInRange)
            {
                float dist = Vector3.Distance(transform.position, e.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = e;
                }
            }
            return closest;
        }

        private BaseEnemy GetWeakestEnemy()
        {
            BaseEnemy weakest = null;
            float minHP = float.MaxValue;
            foreach (var e in enemiesInRange)
            {
                if (e.HPPercent * e.data.maxHP < minHP)
                {
                    minHP = e.HPPercent * e.data.maxHP;
                    weakest = e;
                }
            }
            return weakest;
        }

        private void OnDrawGizmosSelected()
        {
            float range = data != null ? data.range : currentRange;
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
