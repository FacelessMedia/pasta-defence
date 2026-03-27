using UnityEngine;
using PastaDefence.Enemies;

namespace PastaDefence.UI
{
    /// <summary>
    /// Attach to enemy prefab. Shows a floating HP bar above the pasta.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Transform fillBar;
        [SerializeField] private SpriteRenderer fillRenderer;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);
        [SerializeField] private Color fullColor = Color.green;
        [SerializeField] private Color halfColor = Color.yellow;
        [SerializeField] private Color lowColor = Color.red;
        [SerializeField] private Color shieldColor = new Color(0.3f, 0.5f, 1f, 0.8f);

        private BaseEnemy enemy;
        private Transform cachedTransform;
        private bool hasShield;

        private void Awake()
        {
            enemy = GetComponentInParent<BaseEnemy>();
            cachedTransform = transform;
        }

        private void LateUpdate()
        {
            if (enemy == null || enemy.IsDead)
            {
                SetVisible(false);
                return;
            }

            // Position above enemy
            if (enemy.transform != null)
                cachedTransform.position = enemy.transform.position + offset;

            // Always face camera (for future 3D support)
            cachedTransform.rotation = Quaternion.identity;

            float hpPercent = enemy.HPPercent;

            // Update fill scale
            if (fillBar != null)
            {
                Vector3 scale = fillBar.localScale;
                scale.x = Mathf.Clamp01(hpPercent);
                fillBar.localScale = scale;
            }

            // Update color
            if (fillRenderer != null)
            {
                if (hpPercent > 0.6f)
                    fillRenderer.color = fullColor;
                else if (hpPercent > 0.3f)
                    fillRenderer.color = halfColor;
                else
                    fillRenderer.color = lowColor;
            }

            // Show/hide based on HP
            SetVisible(hpPercent < 1f);
        }

        private void SetVisible(bool visible)
        {
            if (fillRenderer != null) fillRenderer.enabled = visible;
            if (backgroundRenderer != null) backgroundRenderer.enabled = visible;
        }
    }
}
