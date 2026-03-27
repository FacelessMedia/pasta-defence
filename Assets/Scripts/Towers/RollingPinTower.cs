using UnityEngine;
using PastaDefence.Enemies;

namespace PastaDefence.Towers
{
    /// <summary>
    /// Rolling Pin — "The Roller Coaster"
    /// "Flattens your problems. And your pasta."
    /// Melee AoE tower that smashes down onto the path, dealing damage and slowing enemies.
    /// Chance to stun on crit.
    /// </summary>
    public class RollingPinTower : BaseTower
    {
        [Header("Rolling Pin Specific")]
        [SerializeField] private float smashAnimDuration = 0.3f;
        [SerializeField] private float critMultiplier = 2f;
        [SerializeField] private float critChance = 0.15f;

        private bool isSmashing;
        private float smashTimer;
        private Vector3 originalPosition;
        private Vector3 smashPosition;

        protected override void Awake()
        {
            base.Awake();
            originalPosition = transform.position;
        }

        protected override void Update()
        {
            base.Update();

            if (isSmashing)
            {
                smashTimer -= Time.deltaTime;
                float t = 1f - (smashTimer / smashAnimDuration);

                if (t < 0.5f)
                {
                    // Smash down
                    transform.position = Vector3.Lerp(originalPosition, smashPosition, t * 2f);
                }
                else
                {
                    // Return up
                    transform.position = Vector3.Lerp(smashPosition, originalPosition, (t - 0.5f) * 2f);
                }

                if (smashTimer <= 0f)
                {
                    isSmashing = false;
                    transform.position = originalPosition;
                }
            }
        }

        protected override void Attack(BaseEnemy target)
        {
            // Rolling Pin always attacks in AoE — it smashes everything below it
            bool isCrit = Random.value < critChance;
            float dmgMultiplier = isCrit ? critMultiplier : 1f;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentAoeRadius);
            string dmgType = data.damageType.ToString();

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                {
                    enemy.TakeDamage(currentDamage * dmgMultiplier, dmgType);

                    // Always slow
                    enemy.ApplySlow(currentSlowAmount, currentSlowDuration);

                    // Stun on crit
                    if (isCrit && currentStunChance > 0f)
                    {
                        enemy.ApplyStun(currentStunDuration);
                    }
                }
            }

            // Trigger smash animation
            StartSmashAnimation(target.transform.position);
        }

        private void StartSmashAnimation(Vector3 targetPos)
        {
            isSmashing = true;
            smashTimer = smashAnimDuration;
            originalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            smashPosition = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        }
    }
}
