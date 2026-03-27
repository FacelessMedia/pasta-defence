using UnityEngine;

namespace PastaDefence.Core
{
    public enum GameState
    {
        MainMenu,
        PreWave,
        WaveActive,
        WaveComplete,
        Victory,
        Defeat,
        Paused
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int maxServings = 20;
        [SerializeField] private int totalWaves = 10;

        public GameState CurrentState { get; private set; } = GameState.PreWave;
        public int CurrentServings { get; private set; }
        public int CurrentWave { get; private set; } = 0;
        public int TotalWaves => totalWaves;

        private GameState stateBeforePause;

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
            CurrentServings = maxServings;
            SetState(GameState.PreWave);
            EventBus.Trigger(GameEvent.GameStarted);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurrentState == GameState.Paused)
                    ResumeGame();
                else if (CurrentState == GameState.PreWave || CurrentState == GameState.WaveActive)
                    PauseGame();
            }
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            EventBus.Trigger(GameEvent.StateChanged, newState);
        }

        public void StartNextWave()
        {
            if (CurrentState != GameState.PreWave && CurrentState != GameState.WaveComplete)
                return;

            CurrentWave++;
            SetState(GameState.WaveActive);
            EventBus.Trigger(GameEvent.WaveStarted, CurrentWave);
        }

        public void OnWaveComplete()
        {
            if (CurrentWave >= totalWaves)
            {
                SetState(GameState.Victory);
                EventBus.Trigger(GameEvent.Victory);
            }
            else
            {
                SetState(GameState.WaveComplete);
                EventBus.Trigger(GameEvent.WaveCompleted, CurrentWave);
            }
        }

        public void LoseServing(int amount = 1)
        {
            CurrentServings = Mathf.Max(0, CurrentServings - amount);
            EventBus.Trigger(GameEvent.ServingsChanged, CurrentServings);

            if (CurrentServings <= 0)
            {
                SetState(GameState.Defeat);
                EventBus.Trigger(GameEvent.Defeat);
            }
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Paused) return;
            stateBeforePause = CurrentState;
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            Time.timeScale = 1f;
            SetState(stateBeforePause);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
