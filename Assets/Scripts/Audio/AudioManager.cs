using UnityEngine;
using PastaDefence.Core;

namespace PastaDefence.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music Tracks")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private AudioClip victoryMusic;
        [SerializeField] private AudioClip defeatMusic;

        [Header("SFX")]
        [SerializeField] private AudioClip towerPlaceSfx;
        [SerializeField] private AudioClip towerSellSfx;
        [SerializeField] private AudioClip towerUpgradeSfx;
        [SerializeField] private AudioClip enemyDeathSfx;
        [SerializeField] private AudioClip enemyLeakSfx;
        [SerializeField] private AudioClip waveStartSfx;
        [SerializeField] private AudioClip waveCompleteSfx;
        [SerializeField] private AudioClip buttonClickSfx;
        [SerializeField] private AudioClip chefAbilitySfx;
        [SerializeField] private AudioClip chefDownedSfx;

        [Header("Settings")]
        [Range(0f, 1f)] public float musicVolume = 0.5f;
        [Range(0f, 1f)] public float sfxVolume = 0.7f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            EventBus.Subscribe(GameEvent.TowerPlaced, _ => PlaySFX(towerPlaceSfx));
            EventBus.Subscribe(GameEvent.TowerSold, _ => PlaySFX(towerSellSfx));
            EventBus.Subscribe(GameEvent.TowerUpgraded, _ => PlaySFX(towerUpgradeSfx));
            EventBus.Subscribe(GameEvent.EnemyKilled, _ => PlaySFX(enemyDeathSfx));
            EventBus.Subscribe(GameEvent.EnemyReachedEnd, _ => PlaySFX(enemyLeakSfx));
            EventBus.Subscribe(GameEvent.WaveStarted, _ => PlaySFX(waveStartSfx));
            EventBus.Subscribe(GameEvent.WaveCompleted, _ => PlaySFX(waveCompleteSfx));
            EventBus.Subscribe(GameEvent.ChefAbilityUsed, _ => PlaySFX(chefAbilitySfx));
            EventBus.Subscribe(GameEvent.ChefDowned, _ => PlaySFX(chefDownedSfx));
            EventBus.Subscribe(GameEvent.Victory, _ => PlayMusic(victoryMusic));
            EventBus.Subscribe(GameEvent.Defeat, _ => PlayMusic(defeatMusic));
            EventBus.Subscribe(GameEvent.BossSpawned, _ => PlayMusic(bossMusic));
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickSfx);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }
    }
}
