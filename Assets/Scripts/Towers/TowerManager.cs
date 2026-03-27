using UnityEngine;
using PastaDefence.Core;

namespace PastaDefence.Towers
{
    public class TowerManager : MonoBehaviour
    {
        public static TowerManager Instance { get; private set; }

        [Header("Available Towers")]
        [SerializeField] private TowerData[] availableTowers;

        public bool IsPlacingTower { get; private set; }
        public TowerData SelectedTowerData { get; private set; }
        public BaseTower SelectedTower { get; private set; }

        private TowerPlacement[] allPlacements;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            allPlacements = FindObjectsByType<TowerPlacement>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            // Cancel placement with right-click
            if (IsPlacingTower && Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }

        public TowerData[] GetAvailableTowers()
        {
            return availableTowers;
        }

        public void StartPlacement(TowerData towerData)
        {
            if (!EconomyManager.Instance.CanAfford(towerData.cost))
                return;

            SelectedTowerData = towerData;
            IsPlacingTower = true;
            DeselectTower();

            // Show all placement spots
            foreach (var placement in allPlacements)
            {
                placement.SetHighlight(true);
            }
        }

        public void PlaceTowerAt(TowerPlacement placement)
        {
            if (!IsPlacingTower || SelectedTowerData == null) return;
            if (placement.IsOccupied) return;
            if (!EconomyManager.Instance.SpendDough(SelectedTowerData.cost)) return;

            // Instantiate tower
            GameObject towerObj = Instantiate(
                SelectedTowerData.prefab,
                placement.transform.position,
                Quaternion.identity
            );

            BaseTower tower = towerObj.GetComponent<BaseTower>();
            if (tower != null)
            {
                tower.Initialize(SelectedTowerData);
                placement.PlaceTower(tower);
                EventBus.Trigger(GameEvent.TowerPlaced, tower);
            }

            CancelPlacement();
        }

        public void CancelPlacement()
        {
            IsPlacingTower = false;
            SelectedTowerData = null;

            foreach (var placement in allPlacements)
            {
                placement.SetHighlight(false);
            }
        }

        public void SelectTower(BaseTower tower)
        {
            if (IsPlacingTower) return;

            SelectedTower = tower;
            // UI will read SelectedTower to show upgrade/sell panel
        }

        public void DeselectTower()
        {
            SelectedTower = null;
        }

        public void UpgradeSelectedTower()
        {
            if (SelectedTower == null) return;
            SelectedTower.Upgrade();
        }

        public void SellSelectedTower()
        {
            if (SelectedTower == null) return;

            // Find placement and clear it
            foreach (var placement in allPlacements)
            {
                if (placement.CurrentTower == SelectedTower)
                {
                    placement.RemoveTower();
                    break;
                }
            }

            SelectedTower.Sell();
            SelectedTower = null;
        }

        public void SetTargetingMode(TargetingMode mode)
        {
            if (SelectedTower != null)
            {
                SelectedTower.SetTargetingMode(mode);
            }
        }
    }
}
