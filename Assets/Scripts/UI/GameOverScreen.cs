using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PastaDefence.Core;
using PastaDefence.Humor;

namespace PastaDefence.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;

        [Header("Victory")]
        [SerializeField] private GameObject victoryContent;
        [SerializeField] private TextMeshProUGUI victoryTitle;
        [SerializeField] private TextMeshProUGUI victoryQuip;
        [SerializeField] private TextMeshProUGUI starRatingText;
        [SerializeField] private Image[] starImages;

        [Header("Defeat")]
        [SerializeField] private GameObject defeatContent;
        [SerializeField] private TextMeshProUGUI defeatTitle;
        [SerializeField] private TextMeshProUGUI defeatQuip;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI wavesCompletedText;
        [SerializeField] private TextMeshProUGUI doughEarnedText;

        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button nextStageButton;

        private void Start()
        {
            panel.SetActive(false);

            EventBus.Subscribe(GameEvent.Victory, OnVictory);
            EventBus.Subscribe(GameEvent.Defeat, OnDefeat);

            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe(GameEvent.Victory, OnVictory);
            EventBus.Unsubscribe(GameEvent.Defeat, OnDefeat);
        }

        private void OnVictory(object data)
        {
            panel.SetActive(true);

            if (victoryContent != null) victoryContent.SetActive(true);
            if (defeatContent != null) defeatContent.SetActive(false);

            // Pick random victory title and quip
            if (victoryTitle != null)
            {
                int idx = Random.Range(0, FlavorTextProvider.VictoryTitles.Length);
                victoryTitle.text = FlavorTextProvider.VictoryTitles[idx];
            }

            if (victoryQuip != null && PunManager.Instance != null)
            {
                victoryQuip.text = PunManager.Instance.GetQuip(QuipCategory.Victory);
            }

            // Calculate star rating
            int stars = CalculateStars();
            ShowStars(stars);

            if (starRatingText != null)
                starRatingText.text = FlavorTextProvider.GetStarRatingMessage(stars);

            UpdateStats();

            if (nextStageButton != null)
                nextStageButton.gameObject.SetActive(true);
        }

        private void OnDefeat(object data)
        {
            panel.SetActive(true);

            if (victoryContent != null) victoryContent.SetActive(false);
            if (defeatContent != null) defeatContent.SetActive(true);

            // Pick random defeat title and quip
            if (defeatTitle != null)
            {
                int idx = Random.Range(0, FlavorTextProvider.GameOverTitles.Length);
                defeatTitle.text = FlavorTextProvider.GameOverTitles[idx];
            }

            if (defeatQuip != null && PunManager.Instance != null)
            {
                defeatQuip.text = PunManager.Instance.GetQuip(QuipCategory.GameOver);
            }

            UpdateStats();

            if (nextStageButton != null)
                nextStageButton.gameObject.SetActive(false);
        }

        private int CalculateStars()
        {
            int stars = 1; // Beat the stage

            // 2 stars: no leaks
            if (GameManager.Instance.CurrentServings >= 20) // TODO: compare to max
                stars = 2;

            // 3 stars: under budget (have more than 50% of peak dough)
            // Simplified for now
            if (stars == 2 && EconomyManager.Instance.CurrentDough > 0)
                stars = 3;

            return stars;
        }

        private void ShowStars(int count)
        {
            if (starImages == null) return;
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                    starImages[i].enabled = i < count;
            }
        }

        private void UpdateStats()
        {
            if (wavesCompletedText != null)
                wavesCompletedText.text = $"Waves: {GameManager.Instance.CurrentWave}";

            if (doughEarnedText != null)
                doughEarnedText.text = $"Dough Earned: {EconomyManager.Instance.CurrentDough}";
        }

        private void OnRetryClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        private void OnMenuClicked()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
