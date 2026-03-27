using System.Collections.Generic;
using UnityEngine;

namespace PastaDefence.Meta
{
    public enum SkillBranch
    {
        SharpThinking,   // Offense
        DoughManagement, // Economy
        ThickSkin,       // Defense
        KitchenHacks     // Utility
    }

    [System.Serializable]
    public class SkillNode
    {
        public string id;
        public string skillName;
        public string punName;
        [TextArea(1, 2)] public string description;
        public SkillBranch branch;
        public int starCost = 1;
        public int requiredLevel = 1;
        public string[] prerequisiteIds;
        public string mutuallyExclusiveWith;
        public bool isUnlocked;

        [Header("Stat Bonuses")]
        public float globalDamagePercent;
        public float globalAttackSpeedPercent;
        public float critChanceBonus;
        public int startingDoughBonus;
        public float killRewardPercent;
        public int extraServings;
        public float towerHPPercent;
        public float placementSpeedPercent;
        public float upgradeCostReduction;
    }

    [CreateAssetMenu(fileName = "New Skill Tree", menuName = "Pasta Defence/Skill Tree")]
    public class SkillTreeData : ScriptableObject
    {
        public SkillNode[] nodes;

        public SkillNode GetNode(string id)
        {
            foreach (var node in nodes)
            {
                if (node.id == id) return node;
            }
            return null;
        }

        public SkillNode[] GetBranchNodes(SkillBranch branch)
        {
            var result = new List<SkillNode>();
            foreach (var node in nodes)
            {
                if (node.branch == branch) result.Add(node);
            }
            return result.ToArray();
        }
    }

    public class SkillTree : MonoBehaviour
    {
        public static SkillTree Instance { get; private set; }

        [SerializeField] private SkillTreeData skillTreeData;

        public int AvailableStars { get; private set; }

        // Accumulated bonuses from unlocked skills
        public float BonusDamagePercent { get; private set; }
        public float BonusAttackSpeedPercent { get; private set; }
        public float BonusCritChance { get; private set; }
        public int BonusStartingDough { get; private set; }
        public float BonusKillRewardPercent { get; private set; }
        public int BonusServings { get; private set; }
        public float BonusTowerHPPercent { get; private set; }
        public float BonusPlacementSpeedPercent { get; private set; }
        public float BonusUpgradeCostReduction { get; private set; }

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
            RecalculateBonuses();
        }

        public void AddStars(int amount)
        {
            AvailableStars += amount;
            SaveProgress();
        }

        public bool CanUnlock(string nodeId)
        {
            var node = skillTreeData.GetNode(nodeId);
            if (node == null || node.isUnlocked) return false;
            if (AvailableStars < node.starCost) return false;

            // Check prerequisites
            if (node.prerequisiteIds != null)
            {
                foreach (var prereqId in node.prerequisiteIds)
                {
                    var prereq = skillTreeData.GetNode(prereqId);
                    if (prereq == null || !prereq.isUnlocked) return false;
                }
            }

            // Check mutual exclusivity
            if (!string.IsNullOrEmpty(node.mutuallyExclusiveWith))
            {
                var exclusive = skillTreeData.GetNode(node.mutuallyExclusiveWith);
                if (exclusive != null && exclusive.isUnlocked) return false;
            }

            return true;
        }

        public bool UnlockSkill(string nodeId)
        {
            if (!CanUnlock(nodeId)) return false;

            var node = skillTreeData.GetNode(nodeId);
            AvailableStars -= node.starCost;
            node.isUnlocked = true;

            RecalculateBonuses();
            SaveProgress();
            return true;
        }

        private void RecalculateBonuses()
        {
            BonusDamagePercent = 0f;
            BonusAttackSpeedPercent = 0f;
            BonusCritChance = 0f;
            BonusStartingDough = 0;
            BonusKillRewardPercent = 0f;
            BonusServings = 0;
            BonusTowerHPPercent = 0f;
            BonusPlacementSpeedPercent = 0f;
            BonusUpgradeCostReduction = 0f;

            if (skillTreeData == null || skillTreeData.nodes == null) return;

            foreach (var node in skillTreeData.nodes)
            {
                if (!node.isUnlocked) continue;

                BonusDamagePercent += node.globalDamagePercent;
                BonusAttackSpeedPercent += node.globalAttackSpeedPercent;
                BonusCritChance += node.critChanceBonus;
                BonusStartingDough += node.startingDoughBonus;
                BonusKillRewardPercent += node.killRewardPercent;
                BonusServings += node.extraServings;
                BonusTowerHPPercent += node.towerHPPercent;
                BonusPlacementSpeedPercent += node.placementSpeedPercent;
                BonusUpgradeCostReduction += node.upgradeCostReduction;
            }
        }

        public void SaveProgress()
        {
            PlayerPrefs.SetInt("SkillTree_Stars", AvailableStars);

            if (skillTreeData != null && skillTreeData.nodes != null)
            {
                foreach (var node in skillTreeData.nodes)
                {
                    PlayerPrefs.SetInt($"SkillTree_{node.id}", node.isUnlocked ? 1 : 0);
                }
            }

            PlayerPrefs.Save();
        }

        public void LoadProgress()
        {
            AvailableStars = PlayerPrefs.GetInt("SkillTree_Stars", 0);

            if (skillTreeData != null && skillTreeData.nodes != null)
            {
                foreach (var node in skillTreeData.nodes)
                {
                    node.isUnlocked = PlayerPrefs.GetInt($"SkillTree_{node.id}", 0) == 1;
                }
            }

            RecalculateBonuses();
        }
    }
}
