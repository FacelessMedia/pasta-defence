using UnityEngine;
using PastaDefence.Towers;
using PastaDefence.Enemies;

namespace PastaDefence.Humor
{
    /// <summary>
    /// Provides flavor text, descriptions, and pun names for all game elements.
    /// "Because every pasta deserves a good roast."
    /// </summary>
    public static class FlavorTextProvider
    {
        // Difficulty mode names
        public static string GetDifficultyName(int difficulty)
        {
            return difficulty switch
            {
                0 => "Appetizer",
                1 => "Entrée",
                2 => "Chef's Special",
                3 => "Gordon's Nightmare",
                _ => "Mystery Meat"
            };
        }

        // Star rating messages
        public static string GetStarRatingMessage(int stars)
        {
            return stars switch
            {
                1 => "You survived. Barely.",
                2 => "Clean plate club!",
                3 => "Thrifty AND deadly. Gordon would approve.",
                _ => "How did you get zero stars? That takes talent."
            };
        }

        // Sell button text
        public static string GetSellText(int sellValue)
        {
            return $"Return to Drawer ({sellValue} Dough)";
        }

        // Wave announcement format
        public static string GetWaveAnnouncement(int waveNumber, int totalWaves)
        {
            string label = waveNumber == totalWaves ? "FINAL WAVE" : $"Wave {waveNumber}";

            // Special wave name puns for milestone waves
            string subtitle = waveNumber switch
            {
                1 => "The Appetizer",
                5 => "Holy Cannelloni!",
                10 => "The Plot Thickens (Like Sauce)",
                12 => "Send Noods",
                15 => "Halfway to Heartburn",
                20 => "The Carb Crusade",
                25 => "Quarter-Life Pasta Crisis",
                30 => "The Gluten Gauntlet",
                35 => "Almost Done... Psych!",
                40 => "The Spaghetti Western",
                _ => ""
            };

            return string.IsNullOrEmpty(subtitle) ? label : $"{label}: {subtitle}";
        }

        // Game over screen messages
        public static readonly string[] GameOverTitles =
        {
            "YOU'VE BEEN SERVED.",
            "KITCHEN: CLOSED.",
            "GAME OVER-COOKED."
        };

        // Victory screen messages
        public static readonly string[] VictoryTitles =
        {
            "BON APPÉTIT!",
            "KITCHEN: DEFENDED!",
            "PASTA: DEFEATED!"
        };

        // Currency display
        public static string FormatDough(int amount) => $"{amount} Dough";
        public static string FormatParmesan(int amount) => $"{amount} Parmesan";

        // Servings display
        public static string FormatServings(int current, int max) =>
            $"Servings: {current}/{max}";

        // Targeting mode names (because even these get puns)
        public static string GetTargetingModeName(TargetingMode mode)
        {
            return mode switch
            {
                TargetingMode.First => "First In, First Served",
                TargetingMode.Last => "Save the Best for Last",
                TargetingMode.Strongest => "Go Big or Go Home",
                TargetingMode.Closest => "Up Close and Penne-sonal",
                TargetingMode.Weakest => "Pick on the Little Guy",
                _ => "Chef's Choice"
            };
        }
    }
}
