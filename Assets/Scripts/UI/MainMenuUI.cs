using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PastaDefence.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI taglineText;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Loading Tip")]
        [SerializeField] private TextMeshProUGUI tipText;

        private readonly string[] taglines =
        {
            "Use your noodle.",
            "It's not delivery, it's destruction.",
            "They came. They saw. They carbo-loaded.",
            "This kitchen ain't big enough for the both of us.",
            "Defence never tasted so good.",
            "Al dente and dangerous.",
            "Where kitchen meets carnage.",
            "Pasta la vista, baby."
        };

        private readonly string[] tips =
        {
            "Did you know? Penne is the angriest pasta. It's always so penne-trating.",
            "Pro tip: Rolling Pins are great for flattening both dough AND your enemies.",
            "The Pepper Grinder's secret? It really grinds pasta's gears.",
            "Remember: Every penne counts. Literally. That's the currency name.",
            "Loading... just like waiting for water to boil.",
            "The Frying Pan: for when you need to flip the script. And the pasta."
        };

        private void Start()
        {
            // Set random tagline
            if (taglineText != null)
                taglineText.text = taglines[Random.Range(0, taglines.Length)];

            // Set random tip
            if (tipText != null)
                tipText.text = tips[Random.Range(0, tips.Length)];

            // Button listeners
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage_CuttingBoard_01");
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
