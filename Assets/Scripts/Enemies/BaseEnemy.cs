using UnityEngine;
using PastaDefence.Core;

namespace PastaDefence.Enemies
{
    public class BaseEnemy : MonoBehaviour
    {
        [Header("Runtime State")]
        public EnemyData data;

        protected float currentHP;
        protected float currentShield;
        protected float currentSpeed;
        protected float currentArmor;
        protected bool isDead;
        protected bool isStunned;
        protected float stunTimer;
        protected float slowMultiplier = 1f;
        protected float slowTimer;

        // Pathfinding
        protected WaypointPath path;
        protected int currentWaypointIndex;
        protected float distanceTraveled;

        // Components
        protected SpriteRenderer spriteRenderer;
        private Transform cachedTransform;

        public float HPPercent => data != null ? currentHP / data.maxHP : 0f;
        public bool IsDead => isDead;
        public float DistanceTraveled => distanceTraveled;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            cachedTransform = transform;
        }

        public virtual void Initialize(EnemyData enemyData, WaypointPath waypointPath)
        {
            data = enemyData;
            path = waypointPath;

            currentHP = data.maxHP;
            currentShield = data.shieldAmount;
            currentSpeed = data.speed;
            currentArmor = data.armor;
            currentWaypointIndex = 0;
            distanceTraveled = 0f;
            isDead = false;
            isStunned = false;
            stunTimer = 0f;
            slowMultiplier = 1f;
            slowTimer = 0f;

            if (path != null && path.WaypointCount > 0)
                cachedTransform.position = path.GetWaypointPosition(0);
        }

        protected virtual void Update()
        {
            if (isDead) return;

            UpdateStatusEffects();

            if (!isStunned)
            {
                Move();
            }
        }

        protected virtual void Move()
        {
            if (path == null || currentWaypointIndex >= path.WaypointCount) return;

            Vector3 target = path.GetWaypointPosition(currentWaypointIndex);
            Vector3 direction = (target - cachedTransform.position).normalized;
            float effectiveSpeed = currentSpeed * slowMultiplier;
            float step = effectiveSpeed * Time.deltaTime;

            cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, target, step);
            distanceTraveled += step;

            // Flip sprite based on movement direction
            if (spriteRenderer != null && direction.x != 0)
                spriteRenderer.flipX = direction.x < 0;

            if (Vector3.Distance(cachedTransform.position, target) < 0.05f)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= path.WaypointCount)
                {
                    ReachedEnd();
                }
            }
        }

        protected virtual void UpdateStatusEffects()
        {
            if (isStunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f) isStunned = false;
            }

            if (slowMultiplier < 1f)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0f) slowMultiplier = 1f;
            }
        }

        public virtual float TakeDamage(float amount, string damageType = "")
        {
            if (isDead) return 0f;

            // Apply evasion
            if (data.primaryAbility == EnemyAbility.Evasive || data.secondaryAbility == EnemyAbility.Evasive)
            {
                if (Random.value < data.evasionChance)
                    return 0f;
            }

            // Apply damage type multiplier
            float multiplier = 1f;
            if (!string.IsNullOrEmpty(damageType))
                multiplier = data.GetDamageMultiplier(damageType);

            float finalDamage = amount * multiplier;

            // Apply armor reduction
            finalDamage = Mathf.Max(1f, finalDamage - currentArmor);

            // Absorb with shield first
            if (currentShield > 0f)
            {
                float shieldAbsorb = Mathf.Min(currentShield, finalDamage);
                currentShield -= shieldAbsorb;
                finalDamage -= shieldAbsorb;
            }

            currentHP -= finalDamage;

            if (currentHP <= 0f)
            {
                Die();
            }

            return finalDamage;
        }

        public virtual void ApplySlow(float amount, float duration)
        {
            slowMultiplier = Mathf.Min(slowMultiplier, 1f - amount);
            slowTimer = Mathf.Max(slowTimer, duration);
        }

        public virtual void ApplyStun(float duration)
        {
            isStunned = true;
            stunTimer = Mathf.Max(stunTimer, duration);
        }

        public virtual void ReduceArmor(float amount)
        {
            currentArmor = Mathf.Max(0f, currentArmor - amount);
        }

        public virtual void Knockback(float distance)
        {
            if (currentWaypointIndex <= 0) return;

            // Move backward along the path
            Vector3 previousWaypoint = path.GetWaypointPosition(currentWaypointIndex - 1);
            Vector3 direction = (previousWaypoint - transform.position).normalized;
            transform.position += direction * distance;
            distanceTraveled -= distance;
        }

        protected virtual void Die()
        {
            if (isDead) return;
            isDead = true;

            EventBus.Trigger(GameEvent.EnemyKilled, data.doughReward);
            OnDeath();

            ObjectPool.Instance.Return(gameObject);
        }

        protected virtual void OnDeath()
        {
            // Override in subclasses for special death behavior (e.g., splitting)
        }

        protected virtual void ReachedEnd()
        {
            isDead = true;
            GameManager.Instance.LoseServing(data.servingsLostOnLeak);
            EventBus.Trigger(GameEvent.EnemyReachedEnd, data);

            ObjectPool.Instance.Return(gameObject);
        }
    }
}
