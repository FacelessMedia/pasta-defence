using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PastaDefence.Core;
using PastaDefence.Humor;
using PastaDefence.Towers;

namespace PastaDefence.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Top Bar")]
        [SerializeField] private TextMeshProUGUI doughText;
        [SerializeField] private TextMeshProUGUI servingsText;
        [SerializeField] private TextMeshProUGUI waveText;

        [Header("Tower Selection Panel")]
        [SerializeField] private Transform towerButtonContainer;
        [SerializeField] private GameObject towerButtonPrefab;

        [Header("Tower Info Panel")]
        [SerializeField] private GameObject towerInfoPanel;
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI towerPunNameText;
        [SerializeField] private TextMeshProUGUI towerStatsText;
        [SerializeField] private TextMeshProUGUI towerFlavorText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI upgradeButtonText;
        [SerializeField] private Button sellButton;
        [SerializeField] private TextMeshProUGUI sellButtonText;
        [SerializeField] private Button[] targetingButtons;

        [Header("Wave Announcement")]
        [SerializeField] private GameObject waveAnnouncementPanel;
        [SerializeField] private TextMeshProUGUI waveAnnouncementText;
        [SerializeField] private float announcementDuration = 3f;

        [Header("Quip Display")]
        [SerializeField] private TextMeshProUGUI quipText;
        [SerializeField] private float quipDuration = 2.5f;

        [Header("Start Wave Button")]
        [SerializeField] private Button startWaveButton;
        [SerializeField] private TextMeshProUGUI startWaveButtonText;

        [Header("Speed Controls")]
        [SerializeField] private Button speedButton;
        [SerializeField] private TextMeshProUGUI speedButtonText;

        [Header("Chef Info")]
        [SerializeField] private TextMeshProUGUI chefTitleText;
        [SerializeField] private Slider chefHPBar;

        private float announcementTimer;
        private float quipTimer;
        private int currentSpeedIndex = 0;
        private readonly float[] speedOptions = { 1f, 1.5f, 2f, 3f };

        private void Start()
        {
            // Subscribe to events
            EventBus.Subscribe(GameEvent.DoughChanged, OnDoughChanged);
            EventBus.Subscribe(GameEvent.ServingsChanged, OnServingsChanged);
            EventBus.Subscribe(GameEvent.WaveStarted, OnWaveStarted);
            EventBus.Subscribe(GameEvent.WaveCompleted, OnWaveCompleted);
            EventBus.Subscribe(GameEvent.StateChanged, OnStateChanged);

            // Subscribe to pun manager
            if (PunManager.Instance != null)
                PunManager.Instance.OnQuipTriggered += ShowQuip;

            // Initialize UI
            UpdateDoughDisplay(EconomyManager.Instance.CurrentDough);
            UpdateServingsDisplay(GameManager.Instance.CurrentServings);
            UpdateWaveDisplay();
            PopulateTowerButtons();
            HideTowerInfo();
            HideWaveAnnouncement();
            HideQuip();

            // Button listeners
            if (startWaveButton != null)
                startWaveButton.onClick.AddListener(OnStartWaveClicked);
            if (speedButton != null)
                speedButton.onClick.AddListener(OnSpeedClicked);
            if (upgradeButton != null)
                upgradeButton.onClick.AddListener(OnUpgradeClicked);
            if (sellButton != null)
                sellButton.onClick.AddListener(OnSellClicked);

            UpdateStartWaveButton();
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe(GameEvent.DoughChanged, OnDoughChanged);
            EventBus.Unsubscribe(GameEvent.ServingsChanged, OnServingsChanged);
            EventBus.Unsubscribe(GameEvent.WaveStarted, OnWaveStarted);
            EventBus.Unsubscribe(GameEvent.WaveCompleted, OnWaveCompleted);
            EventBus.Unsubscribe(GameEvent.StateChanged, OnStateChanged);

            if (PunManager.Instance != null)
                PunManager.Instance.OnQuipTriggered -= ShowQuip;
        }

        private void Update()
        {
            // Announcement timer
            if (announcementTimer > 0f)
            {
                announcementTimer -= Time.deltaTime;
                if (announcementTimer <= 0f) HideWaveAnnouncement();
            }

            // Quip timer
            if (quipTimer > 0f)
            {
                quipTimer -= Time.deltaTime;
                if (quipTimer <= 0f) HideQuip();
            }

            // Update tower info panel if a tower is selected
            if (TowerManager.Instance != null && TowerManager.Instance.SelectedTower != null)
            {
                ShowTowerInfo(TowerManager.Instance.SelectedTower);
            }

            // Update chef info
            UpdateChefInfo();
        }

        // --- Event Handlers ---

        private void OnDoughChanged(object data)
        {
            if (data is int dough) UpdateDoughDisplay(dough);
        }

        private void OnServingsChanged(object data)
        {
            if (data is int servings) UpdateServingsDisplay(servings);
        }

        private void OnWaveStarted(object data)
        {
            if (data is int wave)
            {
                UpdateWaveDisplay();
                ShowWaveAnnouncement(wave);
            }
            UpdateStartWaveButton();
        }

        private void OnWaveCompleted(object data)
        {
            UpdateStartWaveButton();
        }

        private void OnStateChanged(object data)
        {
            UpdateStartWaveButton();
        }

        // --- Display Updates ---

        private void UpdateDoughDisplay(int dough)
        {
            if (doughText != null)
                doughText.text = FlavorTextProvider.FormatDough(dough);
        }

        private void UpdateServingsDisplay(int servings)
        {
            if (servingsText != null)
                servingsText.text = $"Servings: {servings}";
        }

        private void UpdateWaveDisplay()
        {
            if (waveText != null && GameManager.Instance != null)
            {
                int current = GameManager.Instance.CurrentWave;
                int total = GameManager.Instance.TotalWaves;
                waveText.text = $"Wave: {current}/{total}";
            }
        }

        private void ShowWaveAnnouncement(int waveNumber)
        {
            if (waveAnnouncementPanel == null || waveAnnouncementText == null) return;

            int total = GameManager.Instance.TotalWaves;
            string announcement = FlavorTextProvider.GetWaveAnnouncement(waveNumber, total);
            waveAnnouncementText.text = announcement;
            waveAnnouncementPanel.SetActive(true);
            announcementTimer = announcementDuration;
        }

        private void HideWaveAnnouncement()
        {
            if (waveAnnouncementPanel != null)
                waveAnnouncementPanel.SetActive(false);
        }

        private void ShowQuip(string quip, QuipCategory category)
        {
            if (quipText == null) return;
            quipText.text = quip;
            quipText.gameObject.SetActive(true);
            quipTimer = quipDuration;
        }

        private void HideQuip()
        {
            if (quipText != null)
                quipText.gameObject.SetActive(false);
        }

        // --- Tower Buttons ---

        private void PopulateTowerButtons()
        {
            if (TowerManager.Instance == null || towerButtonContainer == null || towerButtonPrefab == null)
                return;

            foreach (Transform child in towerButtonContainer)
                Destroy(child.gameObject);

            var towers = TowerManager.Instance.GetAvailableTowers();
            if (towers == null) return;

            foreach (var towerData in towers)
            {
                GameObject btnObj = Instantiate(towerButtonPrefab, towerButtonContainer);
                var btn = btnObj.GetComponent<Button>();
                var nameLabel = btnObj.GetComponentInChildren<TextMeshProUGUI>();

                if (nameLabel != null)
                    nameLabel.text = $"{towerData.punName}\n{towerData.cost} Dough";

                var image = btnObj.transform.Find("Icon")?.GetComponent<Image>();
                if (image != null && towerData.icon != null)
                    image.sprite = towerData.icon;

                var captured = towerData;
                btn.onClick.AddListener(() => TowerManager.Instance.StartPlacement(captured));
            }
        }

        // --- Tower Info Panel ---

        private void ShowTowerInfo(BaseTower tower)
        {
            if (towerInfoPanel == null) return;
            towerInfoPanel.SetActive(true);

            if (towerNameText != null) towerNameText.text = tower.Data.towerName;
            if (towerPunNameText != null) towerPunNameText.text = $"\"{tower.Data.punName}\"";
            if (towerFlavorText != null) towerFlavorText.text = tower.Data.flavorText;

            if (towerStatsText != null)
            {
                towerStatsText.text = $"DMG: {tower.Data.damage:F0}  SPD: {tower.Data.attackSpeed:F1}  RNG: {tower.Data.range:F1}";
            }

            if (upgradeButton != null)
            {
                bool canUpgrade = tower.CanUpgrade();
                upgradeButton.interactable = canUpgrade;
                if (upgradeButtonText != null)
                {
                    if (tower.Data.upgradePath != null && tower.UpgradeLevel < tower.Data.upgradePath.Length)
                    {
                        var next = tower.Data.upgradePath[tower.UpgradeLevel];
                        upgradeButtonText.text = $"{next.punName} ({next.cost} Dough)";
                    }
                    else
                    {
                        upgradeButtonText.text = "MAX LEVEL";
                    }
                }
            }

            if (sellButton != null && sellButtonText != null)
            {
                sellButtonText.text = FlavorTextProvider.GetSellText(tower.SellValue);
            }
        }

        private void HideTowerInfo()
        {
            if (towerInfoPanel != null)
                towerInfoPanel.SetActive(false);
        }

        // --- Chef Info ---

        private void UpdateChefInfo()
        {
            if (Hero.ChefHero.Instance == null) return;

            if (chefTitleText != null)
                chefTitleText.text = $"Chef ({Hero.ChefHero.Instance.Title}) Lv.{Hero.ChefHero.Instance.Level}";

            if (chefHPBar != null)
                chefHPBar.value = Hero.ChefHero.Instance.CurrentHP;
        }

        // --- Button Callbacks ---

        private void OnStartWaveClicked()
        {
            if (GameManager.Instance.CurrentState == GameState.PreWave ||
                GameManager.Instance.CurrentState == GameState.WaveComplete)
            {
                GameManager.Instance.StartNextWave();
            }
        }

        private void OnSpeedClicked()
        {
            currentSpeedIndex = (currentSpeedIndex + 1) % speedOptions.Length;
            Time.timeScale = speedOptions[currentSpeedIndex];

            if (speedButtonText != null)
                speedButtonText.text = $"{speedOptions[currentSpeedIndex]}x";
        }

        private void OnUpgradeClicked()
        {
            TowerManager.Instance?.UpgradeSelectedTower();
        }

        private void OnSellClicked()
        {
            TowerManager.Instance?.SellSelectedTower();
            HideTowerInfo();
        }

        private void UpdateStartWaveButton()
        {
            if (startWaveButton == null) return;

            var state = GameManager.Instance.CurrentState;
            bool canStart = state == GameState.PreWave || state == GameState.WaveComplete;
            startWaveButton.interactable = canStart;

            if (startWaveButtonText != null)
            {
                if (state == GameState.WaveActive)
                    startWaveButtonText.text = "Cooking...";
                else
                    startWaveButtonText.text = "Send Next Wave!";
            }
        }
    }
}
