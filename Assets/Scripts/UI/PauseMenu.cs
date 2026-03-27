using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PastaDefence.Core;
using PastaDefence.Humor;

namespace PastaDefence.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI pauseTitleText;
        [SerializeField] private TextMeshProUGUI pauseQuipText;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void Start()
        {
            panel.SetActive(false);

            EventBus.Subscribe(GameEvent.StateChanged, OnStateChanged);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
            if (restartButton != null)
                restartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
            if (menuButton != null)
                menuButton.onClick.AddListener(GoToMenu);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe(GameEvent.StateChanged, OnStateChanged);
        }

        private void OnStateChanged(object data)
        {
            if (data is GameState state)
            {
                bool isPaused = state == GameState.Paused;
                panel.SetActive(isPaused);

                if (isPaused)
                {
                    // Show a random pause quip
                    if (pauseTitleText != null)
                        pauseTitleText.text = "Al Dente-tion: Game Paused";

                    if (pauseQuipText != null && PunManager.Instance != null)
                        pauseQuipText.text = PunManager.Instance.GetPauseQuip();
                }
            }
        }

        private void GoToMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
