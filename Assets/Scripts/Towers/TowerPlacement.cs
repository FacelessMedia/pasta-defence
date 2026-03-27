using UnityEngine;

namespace PastaDefence.Towers
{
    /// <summary>
    /// Attach to a placement spot on the map. Represents a valid location for tower building.
    /// </summary>
    public class TowerPlacement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer highlightRenderer;
        [SerializeField] private Color availableColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private Color occupiedColor = new Color(1f, 0f, 0f, 0.3f);
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 0f, 0.5f);

        public bool IsOccupied { get; private set; }
        public BaseTower CurrentTower { get; private set; }

        private void Start()
        {
            SetHighlight(false);
        }

        public bool PlaceTower(BaseTower tower)
        {
            if (IsOccupied) return false;

            CurrentTower = tower;
            tower.transform.position = transform.position;
            IsOccupied = true;
            SetHighlight(false);
            return true;
        }

        public void RemoveTower()
        {
            CurrentTower = null;
            IsOccupied = false;
        }

        public void SetHighlight(bool show)
        {
            if (highlightRenderer == null) return;
            highlightRenderer.enabled = show;

            if (show)
            {
                highlightRenderer.color = IsOccupied ? occupiedColor : availableColor;
            }
        }

        public void SetHoverHighlight(bool hover)
        {
            if (highlightRenderer == null) return;
            highlightRenderer.enabled = true;

            if (hover && !IsOccupied)
                highlightRenderer.color = hoverColor;
            else
                highlightRenderer.color = IsOccupied ? occupiedColor : availableColor;
        }

        private void OnMouseEnter()
        {
            if (TowerManager.Instance != null && TowerManager.Instance.IsPlacingTower)
            {
                SetHoverHighlight(true);
            }
        }

        private void OnMouseExit()
        {
            if (TowerManager.Instance != null && TowerManager.Instance.IsPlacingTower)
            {
                SetHighlight(TowerManager.Instance.IsPlacingTower);
            }
        }

        private void OnMouseDown()
        {
            if (TowerManager.Instance != null)
            {
                if (TowerManager.Instance.IsPlacingTower && !IsOccupied)
                {
                    TowerManager.Instance.PlaceTowerAt(this);
                }
                else if (IsOccupied && CurrentTower != null)
                {
                    TowerManager.Instance.SelectTower(CurrentTower);
                }
            }
        }
    }
}
