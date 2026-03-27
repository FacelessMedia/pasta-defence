using UnityEngine;
using PastaDefence.Core;
using PastaDefence.Enemies;

namespace PastaDefence.Hero
{
    public class ChefHero : MonoBehaviour
    {
        public static ChefHero Instance { get; private set; }

        [Header("Base Stats")]
        [SerializeField] private float maxHP = 200f;
        [SerializeField] private float autoAttackDamage = 8f;
        [SerializeField] private float autoAttackSpeed = 1.5f;
        [SerializeField] private float autoAttackRange = 2f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float respawnTime = 10f;

        [Header("Leveling")]
        [SerializeField] private int xpPerWave = 100;
        [SerializeField] private int[] xpThresholds = { 100, 250, 500, 800, 1200, 1800, 2500, 3500, 5000, 7000, 9500, 12500, 16000, 20000, 25000 };
        [SerializeField] private float hpPerLevel = 15f;
        [SerializeField] private float damagePerLevel = 1.5f;

        [Header("Abilities")]
        [SerializeField] private ChefAbility orderUpAbility;
        [SerializeField] private ChefAbility sauceBossAbility;
        [SerializeField] private ChefAbility chefsKissAbility;

        // Runtime state
        public float CurrentHP { get; private set; }
        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; }
        public bool IsDowned { get; private set; }
        public bool IsMoving { get; private set; }
        public string Title => GetTitle();

        private float attackTimer;
        private float respawnTimer;
        private BaseEnemy autoAttackTarget;
        private Vector3 moveTarget;
        private bool hasValidMoveTarget;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            CurrentHP = maxHP;
            EventBus.Subscribe(GameEvent.WaveCompleted, OnWaveCompleted);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe(GameEvent.WaveCompleted, OnWaveCompleted);
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.WaveActive &&
                GameManager.Instance.CurrentState != GameState.PreWave)
                return;

            if (IsDowned)
            {
                HandleRespawn();
                return;
            }

            HandleMovement();
            HandleAutoAttack();
            HandleAbilityInput();
        }

        private void HandleMovement()
        {
            // Right-click to move during PreWave or WaveActive
            if (Input.GetMouseButtonDown(1) && !IsDowned)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0f;
                moveTarget = worldPos;
                hasValidMoveTarget = true;
                IsMoving = true;
            }

            if (hasValidMoveTarget && IsMoving)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, moveTarget, moveSpeed * Time.deltaTime
                );

                // Flip sprite
                if (spriteRenderer != null)
                {
                    Vector3 dir = moveTarget - transform.position;
                    if (dir.x != 0) spriteRenderer.flipX = dir.x < 0;
                }

                if (Vector3.Distance(transform.position, moveTarget) < 0.1f)
                {
                    IsMoving = false;
                    hasValidMoveTarget = false;
                }
            }
        }

        private void HandleAutoAttack()
        {
            if (GameManager.Instance.CurrentState != GameState.WaveActive) return;

            attackTimer -= Time.deltaTime;

            // Find nearest enemy in range
            if (autoAttackTarget == null || autoAttackTarget.IsDead ||
                Vector3.Distance(transform.position, autoAttackTarget.transform.position) > autoAttackRange)
            {
                autoAttackTarget = FindNearestEnemy();
            }

            if (autoAttackTarget != null && attackTimer <= 0f)
            {
                float effectiveDamage = autoAttackDamage + (damagePerLevel * (Level - 1));
                autoAttackTarget.TakeDamage(effectiveDamage, "Impact");
                attackTimer = 1f / autoAttackSpeed;
            }
        }

        private void HandleAbilityInput()
        {
            if (GameManager.Instance.CurrentState != GameState.WaveActive) return;

            // Q — Order Up!
            if (Input.GetKeyDown(KeyCode.Q) && orderUpAbility != null)
            {
                Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0f;
                orderUpAbility.Activate(targetPos);
            }

            // W — Sauce Boss
            if (Input.GetKeyDown(KeyCode.W) && sauceBossAbility != null)
            {
                sauceBossAbility.Activate(transform.position);
            }

            // E — Chef's Kiss
            if (Input.GetKeyDown(KeyCode.E) && chefsKissAbility != null)
            {
                chefsKissAbility.Activate(transform.position);
            }
        }

        public void TakeDamage(float amount)
        {
            if (IsDowned) return;

            CurrentHP -= amount;
            if (CurrentHP <= 0f)
            {
                CurrentHP = 0f;
                GoDown();
            }
        }

        private void GoDown()
        {
            IsDowned = true;
            respawnTimer = respawnTime;
            EventBus.Trigger(GameEvent.ChefDowned, null);

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
        }

        private void HandleRespawn()
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                IsDowned = false;
                CurrentHP = maxHP + (hpPerLevel * (Level - 1));

                if (spriteRenderer != null)
                    spriteRenderer.color = Color.white;
            }
        }

        private void OnWaveCompleted(object data)
        {
            CurrentXP += xpPerWave;
            CheckLevelUp();

            // Heal to full between waves
            CurrentHP = maxHP + (hpPerLevel * (Level - 1));
            IsDowned = false;

            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;
        }

        private void CheckLevelUp()
        {
            if (Level - 1 >= xpThresholds.Length) return;

            while (Level - 1 < xpThresholds.Length && CurrentXP >= xpThresholds[Level - 1])
            {
                Level++;
                maxHP += hpPerLevel;
                CurrentHP = maxHP;
            }
        }

        private BaseEnemy FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, autoAttackRange);
            BaseEnemy nearest = null;
            float minDist = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BaseEnemy>(out var enemy) && !enemy.IsDead)
                {
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = enemy;
                    }
                }
            }

            return nearest;
        }

        private string GetTitle()
        {
            if (Level >= 15) return "Head Chef";
            if (Level >= 10) return "Sous Chef";
            if (Level >= 5) return "Saucier";
            return "Line Cook";
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, autoAttackRange);
        }
    }
}
