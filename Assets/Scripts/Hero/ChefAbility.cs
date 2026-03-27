using UnityEngine;
using PastaDefence.Core;
using PastaDefence.Enemies;
using PastaDefence.Towers;

namespace PastaDefence.Hero
{
    public enum AbilityType
    {
        OrderUp,
        SauceBoss,
        ChefsKiss
    }

    [System.Serializable]
    public class ChefAbility : MonoBehaviour
    {
        [Header("Ability Info")]
        [SerializeField] private AbilityType abilityType;
        [SerializeField] private string abilityName = "Order Up!";
        [SerializeField] private string punDescription = "Throws a hot plate — AoE damage in target area";
        [SerializeField] private float cooldown = 15f;
        [SerializeField] private Sprite icon;

        [Header("Order Up! — AoE Damage")]
        [SerializeField] private float orderUpDamage = 50f;
        [SerializeField] private float orderUpRadius = 2f;

        [Header("Sauce Boss — Slow + DoT Cone")]
        [SerializeField] private float sauceDotDamage = 5f;
        [SerializeField] private float sauceDotDuration = 3f;
        [SerializeField] private float sauceSlowAmount = 0.4f;
        [SerializeField] private float sauceSlowDuration = 3f;
        [SerializeField] private float sauceRadius = 3f;

        [Header("Chef's Kiss — Buff Towers")]
        [SerializeField] private float kissBuffRange = 4f;
        [SerializeField] private float kissAttackSpeedBuff = 0.25f;
        [SerializeField] private float kissBuffDuration = 8f;

        private float cooldownTimer;

        public bool IsReady => cooldownTimer <= 0f;
        public float CooldownPercent => cooldown > 0 ? Mathf.Clamp01(cooldownTimer / cooldown) : 0f;
        public string AbilityName => abilityName;
        public Sprite Icon => icon;

        private void Update()
        {
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;
        }

        public bool Activate(Vector3 targetPosition)
        {
            if (!IsReady) return false;

            switch (abilityType)
            {
                case AbilityType.OrderUp:
                    ActivateOrderUp(targetPosition);
                    break;
                case AbilityType.SauceBoss:
                    ActivateSauceBoss(targetPosition);
                    break;
                case AbilityType.ChefsKiss:
                    ActivateChefsKiss(targetPosition);
                    break;
            }

            cooldownTimer = cooldown;
            EventBus.Trigger(GameEvent.ChefAbilityUsed, abilityType);
            return true;
        }

        private void ActivateOrderUp(Vector3 position)
        {
            // "Order Up!" — AoE damage at target location
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, orderUpRadius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                {
                    enemy.TakeDamage(orderUpDamage, "Heat");
                }
            }

            // TODO: Spawn hot plate VFX at position
        }

        private void ActivateSauceBoss(Vector3 position)
        {
            // "Sauce Boss" — Cone of marinara, slows + DoT
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, sauceRadius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                {
                    enemy.TakeDamage(sauceDotDamage, "Seasoning");
                    enemy.ApplySlow(sauceSlowAmount, sauceSlowDuration);
                }
            }

            // TODO: Spawn marinara splash VFX
        }

        private void ActivateChefsKiss(Vector3 position)
        {
            // "Chef's Kiss" — Buffs all towers in range
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, kissBuffRange);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseTower>(out var tower))
                {
                    // Apply temporary attack speed buff
                    // Towers will need a buff system — for now, directly boost
                    // TODO: Implement proper buff system on towers
                }
            }

            // TODO: Spawn chef's kiss VFX (sparkles, heart particles)
        }
    }
}
