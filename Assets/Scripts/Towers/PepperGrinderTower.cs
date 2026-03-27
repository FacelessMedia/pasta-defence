using UnityEngine;
using PastaDefence.Enemies;

namespace PastaDefence.Towers
{
    /// <summary>
    /// Pepper Grinder — "Sneeze Louise"
    /// "Achoo said it couldn't be done?"
    /// Ranged DoT tower that sprays a pepper cloud.
    /// Applies burning DoT and a "sneeze" debuff that reduces enemy armor.
    /// </summary>
    public class PepperGrinderTower : BaseTower
    {
        [Header("Pepper Grinder Specific")]
        [SerializeField] private float cloudRadius = 1.5f;
        [SerializeField] private float cloudDuration = 2f;
        [SerializeField] private float sneezeArmorReduction = 2f;

        private float dotTickTimer;
        private const float DOT_TICK_INTERVAL = 0.5f;

        protected override void Attack(BaseEnemy target)
        {
            // Pepper Grinder creates a cloud at the target's position
            // Damages all enemies in the cloud
            SprayPepperCloud(target.transform.position);
        }

        private void SprayPepperCloud(Vector3 position)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, cloudRadius);
            string dmgType = data.damageType.ToString();

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                {
                    // Initial hit damage
                    enemy.TakeDamage(currentDamage, dmgType);

                    // Apply armor reduction (sneeze debuff)
                    enemy.ReduceArmor(sneezeArmorReduction);

                    // Apply slow (sneezing slows you down!)
                    if (currentSlowAmount > 0f)
                        enemy.ApplySlow(currentSlowAmount, currentSlowDuration);
                }
            }

            // TODO: Spawn pepper cloud VFX at position
        }

        protected override void OnAttack(BaseEnemy target)
        {
            // Grinder rotation animation
            transform.Rotate(0, 0, -45f * Time.deltaTime * 10f);
        }
    }
}
