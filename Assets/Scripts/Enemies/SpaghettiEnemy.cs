using UnityEngine;

namespace PastaDefence.Enemies
{
    /// <summary>
    /// Spaghetti — Fast swarm pasta. "They're coming in hot! And tangled."
    /// Spawns in large groups, low HP, high speed.
    /// </summary>
    public class SpaghettiEnemy : BaseEnemy
    {
        [Header("Spaghetti Specific")]
        [SerializeField] private float tangleSlowRadius = 1f;
        [SerializeField] private float tangleSlowAmount = 0.05f;

        // Spaghetti is fast but fragile. When multiple spaghetti are close together,
        // they slightly slow each other (tangled!) — a subtle penalty for swarms.

        protected override void Update()
        {
            base.Update();

            if (isDead) return;

            // Spaghetti tangles — nearby spaghetti slow each other slightly
            ApplyTangleEffect();
        }

        private void ApplyTangleEffect()
        {
            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, tangleSlowRadius);
            int tangleCount = 0;

            foreach (var col in nearby)
            {
                if (col.gameObject != gameObject && col.TryGetComponent<SpaghettiEnemy>(out _))
                    tangleCount++;
            }

            // The more spaghetti nearby, the more tangled (slight self-slow)
            if (tangleCount > 3)
            {
                slowMultiplier = Mathf.Max(0.7f, 1f - (tangleCount * tangleSlowAmount));
            }
        }
    }
}
