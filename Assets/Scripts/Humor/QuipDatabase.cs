using UnityEngine;

namespace PastaDefence.Humor
{
    public enum QuipCategory
    {
        EnemyKill,
        WaveStart,
        WaveComplete,
        BossSpawn,
        TowerPlaced,
        TowerSold,
        GameOver,
        Victory,
        ChefPlaced,
        ChefDowned,
        ChefAbility,
        LoadingTip,
        PauseMenu
    }

    [CreateAssetMenu(fileName = "New Quip Database", menuName = "Pasta Defence/Quip Database")]
    public class QuipDatabase : ScriptableObject
    {
        [Header("Enemy Kill Quips")]
        [TextArea(1, 2)] public string[] enemyKillQuips =
        {
            "Pasta la vista, baby.",
            "That's a-more damage than they expected!",
            "You got served!",
            "Order up! One defeated pasta!",
            "Sent to the sauce graveyard.",
            "Al gone-te!",
            "That noodle got nooked.",
            "Another one bites the crust.",
            "Reduced to sauce.",
            "Fork-got to dodge!",
            "That was im-pasta-ble to survive!",
            "Looks like they pasta-way.",
            "Well, that was un-fork-tunate.",
            "Noodled into oblivion.",
            "Carb-onized!"
        };

        [Header("Wave Start Quips")]
        [TextArea(1, 2)] public string[] waveStartQuips =
        {
            "Here they come! Al dente and dangerous!",
            "Incoming! Look alive, Chef!",
            "More pasta? This kitchen never closes!",
            "They're sending noodles! The audacity!",
            "Brace yourself — carbs are coming.",
            "Wave incoming! Hope you're not gluten intolerant!",
            "Lock and load the pepper grinder!",
            "Ready your utensils — it's showtime!",
            "The pasta uprising continues!",
            "Here comes the next course!"
        };

        [Header("Wave Complete Quips")]
        [TextArea(1, 2)] public string[] waveCompleteQuips =
        {
            "Another course completed. What's next on the menu?",
            "Clean plate club!",
            "That wave was a piece of cake. Wait, wrong food game.",
            "Served and handled!",
            "Kitchen: 1. Pasta: 0.",
            "Wave cleared! Time to upgrade your mise en place.",
            "Not bad, Chef. Not bad at all.",
            "That was grate! Get it? ...I'll see myself out.",
            "Dinner is NOT served. To them.",
            "Wave demolished! You're on a roll. A rolling pin, specifically."
        };

        [Header("Boss Spawn Quips")]
        [TextArea(1, 2)] public string[] bossSpawnQuips =
        {
            "Oh pasta... it's a big one!",
            "BOSS INCOMING! This one's got layers!",
            "That is one THICC noodle.",
            "The main course has arrived!",
            "This isn't your average Tuesday night dinner..."
        };

        [Header("Tower Placed Quips")]
        [TextArea(1, 2)] public string[] towerPlacedQuips =
        {
            "Utensil deployed! Let 'em cook!",
            "Placed and ready to pasta-pound!",
            "Another tool in the arsenal. Literally.",
            "Strategic kitchen equipment placement — very professional.",
            "That's using your noodle!"
        };

        [Header("Tower Sold Quips")]
        [TextArea(1, 2)] public string[] towerSoldQuips =
        {
            "Back in the drawer you go.",
            "Don't worry, you'll get your chance to shine.",
            "Return to sender. I mean drawer.",
            "Sometimes you gotta let go. *sniff*",
            "Sold! No refunds on emotional attachment."
        };

        [Header("Game Over Quips")]
        [TextArea(1, 2)] public string[] gameOverQuips =
        {
            "You've been served.",
            "The pasta has won this round. Fork.",
            "Game over, man! Game over! ...Want some pasta?",
            "Your kitchen has been overrun by carbs.",
            "Defeat tastes like cold spaghetti.",
            "The noodles prevail. This time.",
            "You tried your best. Your best wasn't al dente enough.",
            "Kitchen: closed. Permanently."
        };

        [Header("Victory Quips")]
        [TextArea(1, 2)] public string[] victoryQuips =
        {
            "Nailed it! That was grate!",
            "Chef's kiss. Literally.",
            "Victory! The pasta menace has been contained!",
            "You did it! That was im-pasta-bly good!",
            "Kitchen: defended. Pasta: defeated. Chef: exhausted.",
            "The dinner plate survives another day!",
            "That was one spicy performance!",
            "Gordon would approve. Probably. Maybe."
        };

        [Header("Chef Placed Quips")]
        [TextArea(1, 2)] public string[] chefPlacedQuips =
        {
            "Chef's in the house! Mise en place, baby.",
            "Let's get this bread... I mean pasta.",
            "Time to cook up some defense!",
            "Alright noodles, who wants to get served?",
            "The Chef has entered the kitchen."
        };

        [Header("Chef Downed Quips")]
        [TextArea(1, 2)] public string[] chefDownedQuips =
        {
            "I... need a minute... in the walk-in...",
            "That's not al dente, that's al pain-te!",
            "Chef down! CHEF DOWN!",
            "Someone call the sous chef...",
            "I'll be back. After a short nap."
        };

        [Header("Chef Ability Quips")]
        [TextArea(1, 2)] public string[] chefAbilityQuips =
        {
            "Order up!",
            "Time to bring the heat!",
            "Sauce 'em!",
            "Chef's special, coming right up!",
            "BAM! As a famous chef once said."
        };

        [Header("Loading Screen Tips")]
        [TextArea(1, 3)] public string[] loadingTips =
        {
            "Did you know? Penne is the angriest pasta. It's always so penne-trating.",
            "Pro tip: Rolling Pins are great for flattening both dough AND your enemies.",
            "The Pepper Grinder's secret? It really grinds pasta's gears.",
            "Tip: Place towers near path curves for maximum pasta punishment.",
            "Fun fact: Rigatoni means 'ridged' in Italian. Also means 'hard to kill' in this game.",
            "Remember: Every penne counts. Literally. That's the currency name.",
            "Fusilli fact: Their spiral shape makes them 40% more annoying to hit.",
            "The Colander: straining out problems since... well, since you placed it.",
            "Did you know spaghetti means 'thin strings'? More like 'thin hit points'.",
            "Hot tip: The Blowtorch doesn't care about armor. Neither should you.",
            "Loading... just like waiting for water to boil.",
            "The Frying Pan: for when you need to flip the script. And the pasta.",
            "Farfalle thinks it can fly. Prove it wrong.",
            "Your Chef heals between waves. Unlike your ego after losing.",
            "Angel Hair pasta is invisible until close. Just like your dad at the store."
        };

        [Header("Pause Menu Quips")]
        [TextArea(1, 2)] public string[] pauseMenuQuips =
        {
            "Al Dente-tion: Game Paused",
            "Taking a breather? The pasta waits for no one. Oh wait, it does. You paused.",
            "Paused. The pasta is judging your bathroom break.",
            "Quick! Think about strategy! Or snacks. Both work.",
            "Game paused. Time to re-assess your noodle strategy."
        };

        public string GetRandomQuip(QuipCategory category)
        {
            string[] pool = GetPool(category);
            if (pool == null || pool.Length == 0) return "";
            return pool[Random.Range(0, pool.Length)];
        }

        private string[] GetPool(QuipCategory category)
        {
            return category switch
            {
                QuipCategory.EnemyKill => enemyKillQuips,
                QuipCategory.WaveStart => waveStartQuips,
                QuipCategory.WaveComplete => waveCompleteQuips,
                QuipCategory.BossSpawn => bossSpawnQuips,
                QuipCategory.TowerPlaced => towerPlacedQuips,
                QuipCategory.TowerSold => towerSoldQuips,
                QuipCategory.GameOver => gameOverQuips,
                QuipCategory.Victory => victoryQuips,
                QuipCategory.ChefPlaced => chefPlacedQuips,
                QuipCategory.ChefDowned => chefDownedQuips,
                QuipCategory.ChefAbility => chefAbilityQuips,
                QuipCategory.LoadingTip => loadingTips,
                QuipCategory.PauseMenu => pauseMenuQuips,
                _ => null
            };
        }
    }
}
