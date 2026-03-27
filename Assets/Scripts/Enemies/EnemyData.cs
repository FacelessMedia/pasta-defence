using UnityEngine;

namespace PastaDefence.Enemies
{
    public enum EnemyType
    {
        Penne,
        Spaghetti,
        Rigatoni,
        Farfalle,
        Ravioli,
        Lasagna,
        Fusilli,
        Orzo,
        Macaroni,
        AngelHair,
        Tortellini,
        Cannelloni,
        Gnocchi,
        Linguine,
        StuffedShell
    }

    public enum EnemyAbility
    {
        None,
        Flying,
        Shielded,
        Splitting,
        Healing,
        Stealth,
        Dashing,
        Siege,
        Regenerating,
        SpeedAura,
        Evasive
    }

    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Pasta Defence/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string enemyName = "Penne";
        public string punName = "Not the sharpest noodle in the box.";
        public EnemyType enemyType = EnemyType.Penne;
        public Sprite icon;
        public GameObject prefab;

        [Header("Base Stats")]
        public float maxHP = 100f;
        public float speed = 2f;
        public float armor = 0f;
        public int doughReward = 10;

        [Header("Abilities")]
        public EnemyAbility primaryAbility = EnemyAbility.None;
        public EnemyAbility secondaryAbility = EnemyAbility.None;

        [Header("Damage Resistances (-1 to 1: negative = weak, positive = resistant)")]
        [Range(-1f, 1f)] public float heatResistance = 0f;
        [Range(-1f, 1f)] public float impactResistance = 0f;
        [Range(-1f, 1f)] public float sharpResistance = 0f;
        [Range(-1f, 1f)] public float seasoningResistance = 0f;

        [Header("Special Stats")]
        public int servingsLostOnLeak = 1;
        public float shieldAmount = 0f;
        public int splitCount = 0;
        public EnemyData splitInto;
        public float healAmount = 0f;
        public float healInterval = 0f;
        public float dashDistance = 0f;
        public float dashCooldown = 0f;
        public float regenPerSecond = 0f;
        public float speedAuraMultiplier = 1f;
        public float speedAuraRange = 0f;
        public float evasionChance = 0f;

        [Header("Flavor Text")]
        [TextArea(2, 4)] public string flavorText;
        public string[] deathQuips;

        public float GetDamageMultiplier(string damageType)
        {
            float resistance = damageType switch
            {
                "Heat" => heatResistance,
                "Impact" => impactResistance,
                "Sharp" => sharpResistance,
                "Seasoning" => seasoningResistance,
                _ => 0f
            };
            return 1f - resistance;
        }
    }
}
