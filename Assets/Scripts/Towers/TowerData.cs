using UnityEngine;

namespace PastaDefence.Towers
{
    public enum TowerType
    {
        RollingPin,
        PepperGrinder,
        ChefsKnife,
        Whisk,
        Colander,
        Blowtorch,
        MeatTenderizer,
        FryingPan,
        SpiceRack,
        TipJar
    }

    public enum DamageType
    {
        Impact,
        Heat,
        Sharp,
        Seasoning
    }

    public enum TargetingMode
    {
        First,
        Last,
        Strongest,
        Closest,
        Weakest
    }

    [CreateAssetMenu(fileName = "New Tower Data", menuName = "Pasta Defence/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("Identity")]
        public string towerName = "Rolling Pin";
        public string punName = "The Roller Coaster";
        public TowerType towerType = TowerType.RollingPin;
        public Sprite icon;
        public GameObject prefab;

        [Header("Base Stats")]
        public float damage = 10f;
        public float attackSpeed = 1f;
        public float range = 3f;
        public int cost = 50;
        public DamageType damageType = DamageType.Impact;

        [Header("Targeting")]
        public TargetingMode defaultTargeting = TargetingMode.First;
        public bool canTargetFlying = false;
        public bool isAreaOfEffect = false;
        public float aoeRadius = 0f;

        [Header("Special Effects")]
        public float slowAmount = 0f;
        public float slowDuration = 0f;
        public float stunChance = 0f;
        public float stunDuration = 0f;
        public float armorReduction = 0f;
        public float dotDamage = 0f;
        public float dotDuration = 0f;
        public float knockbackForce = 0f;
        public float buffDamagePercent = 0f;
        public float buffSpeedPercent = 0f;
        public float buffRange = 0f;
        public int doughPerWave = 0;

        [Header("Upgrades")]
        public TowerUpgradeData[] upgradePath;

        [Header("Flavor Text")]
        [TextArea(2, 4)] public string flavorText;
        public string placementQuip;
        public string sellQuip;

        public float DPS => damage * attackSpeed;
    }

    [System.Serializable]
    public class TowerUpgradeData
    {
        public string upgradeName = "Upgrade";
        public string punName = "Getting Better";
        public string description;
        public int cost = 50;
        public Sprite icon;

        [Header("Stat Modifiers (additive)")]
        public float damageBonus = 0f;
        public float attackSpeedBonus = 0f;
        public float rangeBonus = 0f;
        public float aoeRadiusBonus = 0f;

        [Header("Special Effect Modifiers")]
        public float slowAmountBonus = 0f;
        public float slowDurationBonus = 0f;
        public float stunChanceBonus = 0f;
        public float dotDamageBonus = 0f;
        public float knockbackForceBonus = 0f;
        public float buffDamagePercentBonus = 0f;
        public float doughPerWaveBonus = 0;
    }
}
