using UnityEngine;
using PastaDefence.Enemies;

namespace PastaDefence.Towers
{
    /// <summary>
    /// Chef's Knife — "Knife to Meet You"
    /// "Cuts through problems like butter. And pasta."
    /// Single-target high DPS tower with fast chop attacks.
    /// Bonus damage vs large pasta, can "dice" splitting enemies (preventing splits).
    /// </summary>
    public class ChefsKnifeTower : BaseTower
    {
        [Header("Chef's Knife Specific")]
        [SerializeField] private float largePastaBonusDamage = 1.5f;
        [SerializeField] private bool canPreventSplit = true;
        [SerializeField] private float chopAnimSpeed = 0.15f;

        private bool isChopping;
        private float chopTimer;

        protected override void Attack(BaseEnemy target)
        {
            float finalDamage = currentDamage;
            string dmgType = data.damageType.ToString();

            // Bonus damage against large pasta types (Rigatoni, Lasagna, Cannelloni)
            if (IsLargePasta(target.data.enemyType))
            {
                finalDamage *= largePastaBonusDamage;
            }

            target.TakeDamage(finalDamage, dmgType);
            ApplyEffects(target);

            // Trigger chop animation
            TriggerChopAnimation();
        }

        private bool IsLargePasta(EnemyType type)
        {
            return type == EnemyType.Rigatoni ||
                   type == EnemyType.Lasagna ||
                   type == EnemyType.Cannelloni ||
                   type == EnemyType.StuffedShell;
        }

        protected override void Update()
        {
            base.Update();

            if (isChopping)
            {
                chopTimer -= Time.deltaTime;
                if (chopTimer <= 0f)
                {
                    isChopping = false;
                    // Reset rotation after chop
                    transform.rotation = Quaternion.identity;
                }
                else
                {
                    // Quick rotation for chop visual
                    float t = chopTimer / chopAnimSpeed;
                    float angle = Mathf.Sin(t * Mathf.PI) * 15f;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }

        private void TriggerChopAnimation()
        {
            isChopping = true;
            chopTimer = chopAnimSpeed;
        }
    }
}
