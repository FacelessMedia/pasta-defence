using System.Collections.Generic;
using UnityEngine;

namespace PastaDefence.Meta
{
    [System.Serializable]
    public class Recipe
    {
        public string id;
        public string recipeName;
        [TextArea(1, 2)] public string description;
        [TextArea(1, 2)] public string flavorText;
        public Sprite icon;
        public bool isUnlocked;

        [Header("Effects")]
        public float bonusDamageVsHighHP;       // Al Dente: +dmg vs >50% HP
        public float economyBonusPercent;        // Extra Virgin: economy tower bonus
        public float economyCostPenaltyPercent;  // Extra Virgin: economy tower cost
        public float proximityDamageBonus;       // Parmesan Principle: +dmg per nearby tower
        public float bonusDropChance;            // Leftovers: chance for bonus dough
        public float globalDamageBonus;          // Gluten Fury: flat damage bonus
        public int servingPenalty;               // Gluten Fury: fewer starting servings
    }

    [CreateAssetMenu(fileName = "New Recipe Book", menuName = "Pasta Defence/Recipe Book")]
    public class RecipeBookData : ScriptableObject
    {
        public Recipe[] allRecipes;

        public Recipe GetRecipe(string id)
        {
            foreach (var recipe in allRecipes)
            {
                if (recipe.id == id) return recipe;
            }
            return null;
        }
    }

    public class RecipeBook : MonoBehaviour
    {
        public static RecipeBook Instance { get; private set; }

        [SerializeField] private RecipeBookData recipeData;
        [SerializeField] private int maxEquippedRecipes = 3;

        private List<string> equippedRecipeIds = new();

        public int MaxEquipped => maxEquippedRecipes;
        public List<string> EquippedRecipeIds => equippedRecipeIds;

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

        public Recipe[] GetUnlockedRecipes()
        {
            var result = new List<Recipe>();
            if (recipeData == null || recipeData.allRecipes == null) return result.ToArray();

            foreach (var recipe in recipeData.allRecipes)
            {
                if (recipe.isUnlocked) result.Add(recipe);
            }
            return result.ToArray();
        }

        public Recipe[] GetEquippedRecipes()
        {
            var result = new List<Recipe>();
            foreach (var id in equippedRecipeIds)
            {
                var recipe = recipeData.GetRecipe(id);
                if (recipe != null) result.Add(recipe);
            }
            return result.ToArray();
        }

        public bool EquipRecipe(string recipeId)
        {
            if (equippedRecipeIds.Count >= maxEquippedRecipes) return false;
            if (equippedRecipeIds.Contains(recipeId)) return false;

            var recipe = recipeData.GetRecipe(recipeId);
            if (recipe == null || !recipe.isUnlocked) return false;

            equippedRecipeIds.Add(recipeId);
            return true;
        }

        public void UnequipRecipe(string recipeId)
        {
            equippedRecipeIds.Remove(recipeId);
        }

        public void UnlockRecipe(string recipeId)
        {
            var recipe = recipeData.GetRecipe(recipeId);
            if (recipe != null) recipe.isUnlocked = true;
        }

        public void IncreaseMaxEquipped()
        {
            maxEquippedRecipes++;
        }

        // Get combined effects of all equipped recipes
        public float GetBonusDamageVsHighHP()
        {
            float total = 0f;
            foreach (var recipe in GetEquippedRecipes())
                total += recipe.bonusDamageVsHighHP;
            return total;
        }

        public float GetBonusDropChance()
        {
            float total = 0f;
            foreach (var recipe in GetEquippedRecipes())
                total += recipe.bonusDropChance;
            return total;
        }

        public float GetGlobalDamageBonus()
        {
            float total = 0f;
            foreach (var recipe in GetEquippedRecipes())
                total += recipe.globalDamageBonus;
            return total;
        }

        public int GetServingPenalty()
        {
            int total = 0;
            foreach (var recipe in GetEquippedRecipes())
                total += recipe.servingPenalty;
            return total;
        }
    }
}
