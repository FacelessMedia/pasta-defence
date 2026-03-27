#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using PastaDefence.Towers;
using PastaDefence.Enemies;
using PastaDefence.Humor;
using PastaDefence.Map;

namespace PastaDefence.EditorTools
{
    public static class ScriptableObjectFactory
    {
        private const string DATA_ROOT = "Assets/ScriptableObjects";
        private const string TOWER_PATH = DATA_ROOT + "/Towers";
        private const string ENEMY_PATH = DATA_ROOT + "/Enemies";
        private const string WAVE_PATH = DATA_ROOT + "/Waves";
        private const string MAP_PATH = DATA_ROOT + "/Maps";
        private const string HUMOR_PATH = DATA_ROOT + "/Humor";

        [MenuItem("Pasta Defence/Create All Data Assets")]
        public static void CreateAll()
        {
            EnsureFolders();
            CreateTowerData();
            CreateEnemyData();
            CreateWaveData();
            CreateQuipDatabase();
            CreateMapData();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Pasta Defence] All ScriptableObject assets created! That was grate!");
        }

        [MenuItem("Pasta Defence/Create Tower Data")]
        public static void CreateTowerData()
        {
            EnsureFolders();

            // --- Rolling Pin ---
            var rollingPin = CreateAsset<TowerData>(TOWER_PATH, "TowerData_RollingPin");
            rollingPin.towerName = "Rolling Pin";
            rollingPin.punName = "The Roller Coaster";
            rollingPin.towerType = TowerType.RollingPin;
            rollingPin.damage = 15f;
            rollingPin.attackSpeed = 0.8f;
            rollingPin.range = 2f;
            rollingPin.cost = 60;
            rollingPin.damageType = DamageType.Impact;
            rollingPin.isAreaOfEffect = true;
            rollingPin.aoeRadius = 1.2f;
            rollingPin.slowAmount = 0.3f;
            rollingPin.slowDuration = 1.5f;
            rollingPin.stunChance = 0.15f;
            rollingPin.stunDuration = 0.5f;
            rollingPin.defaultTargeting = TargetingMode.First;
            rollingPin.flavorText = "Flattens your problems. And your pasta.";
            rollingPin.placementQuip = "Time to roll out the welcome mat... of pain.";
            rollingPin.sellQuip = "Back in the drawer. Rolling away...";
            rollingPin.upgradePath = new TowerUpgradeData[]
            {
                new TowerUpgradeData
                {
                    upgradeName = "Warmed Up", punName = "Warmed Up",
                    description = "+Damage, +Speed", cost = 40,
                    damageBonus = 5f, attackSpeedBonus = 0.2f
                },
                new TowerUpgradeData
                {
                    upgradeName = "On a Roll", punName = "On a Roll",
                    description = "+AoE radius, +Slow duration", cost = 80,
                    aoeRadiusBonus = 0.5f, slowDurationBonus = 0.5f
                },
                new TowerUpgradeData
                {
                    upgradeName = "Steamroller", punName = "Steamroller",
                    description = "Massive AoE, perma-slow, lower damage", cost = 150,
                    aoeRadiusBonus = 1f, slowAmountBonus = 0.3f, slowDurationBonus = 2f, damageBonus = -3f
                }
            };
            EditorUtility.SetDirty(rollingPin);

            // --- Chef's Knife ---
            var knife = CreateAsset<TowerData>(TOWER_PATH, "TowerData_ChefsKnife");
            knife.towerName = "Chef's Knife";
            knife.punName = "Knife to Meet You";
            knife.towerType = TowerType.ChefsKnife;
            knife.damage = 25f;
            knife.attackSpeed = 1.5f;
            knife.range = 2.5f;
            knife.cost = 75;
            knife.damageType = DamageType.Sharp;
            knife.isAreaOfEffect = false;
            knife.defaultTargeting = TargetingMode.Strongest;
            knife.flavorText = "Cuts through problems like butter. And pasta.";
            knife.placementQuip = "Let's cut to the chase.";
            knife.sellQuip = "Sheathed and shelved.";
            knife.upgradePath = new TowerUpgradeData[]
            {
                new TowerUpgradeData
                {
                    upgradeName = "Sharpened", punName = "Sharpened",
                    description = "+Damage", cost = 50,
                    damageBonus = 10f
                },
                new TowerUpgradeData
                {
                    upgradeName = "Honed Edge", punName = "Honed Edge",
                    description = "+Speed, +Range", cost = 90,
                    attackSpeedBonus = 0.5f, rangeBonus = 0.5f
                },
                new TowerUpgradeData
                {
                    upgradeName = "Cleaver Mode", punName = "Cleaver Mode",
                    description = "Massive damage, slight AoE", cost = 175,
                    damageBonus = 20f, aoeRadiusBonus = 0.8f
                }
            };
            EditorUtility.SetDirty(knife);

            // --- Pepper Grinder ---
            var pepper = CreateAsset<TowerData>(TOWER_PATH, "TowerData_PepperGrinder");
            pepper.towerName = "Pepper Grinder";
            pepper.punName = "Sneeze Louise";
            pepper.towerType = TowerType.PepperGrinder;
            pepper.damage = 8f;
            pepper.attackSpeed = 1.2f;
            pepper.range = 3.5f;
            pepper.cost = 50;
            pepper.damageType = DamageType.Seasoning;
            pepper.isAreaOfEffect = true;
            pepper.aoeRadius = 1.5f;
            pepper.slowAmount = 0.15f;
            pepper.slowDuration = 1f;
            pepper.armorReduction = 2f;
            pepper.dotDamage = 3f;
            pepper.dotDuration = 2f;
            pepper.defaultTargeting = TargetingMode.First;
            pepper.flavorText = "Achoo said it couldn't be done?";
            pepper.placementQuip = "Bless you. In advance.";
            pepper.sellQuip = "The pepper has left the grinder.";
            pepper.upgradePath = new TowerUpgradeData[]
            {
                new TowerUpgradeData
                {
                    upgradeName = "Freshly Ground", punName = "Freshly Ground",
                    description = "+DoT damage", cost = 35,
                    dotDamageBonus = 3f
                },
                new TowerUpgradeData
                {
                    upgradeName = "Extra Coarse", punName = "Extra Coarse",
                    description = "+Range, stronger armor shred", cost = 70,
                    rangeBonus = 0.8f, damageBonus = 4f
                },
                new TowerUpgradeData
                {
                    upgradeName = "Ghost Pepper", punName = "Ghost Pepper",
                    description = "Invisible DoT cloud, ignores shields", cost = 140,
                    dotDamageBonus = 8f, aoeRadiusBonus = 0.5f, damageBonus = 5f
                }
            };
            EditorUtility.SetDirty(pepper);

            // --- Whisk ---
            var whisk = CreateAsset<TowerData>(TOWER_PATH, "TowerData_Whisk");
            whisk.towerName = "Whisk";
            whisk.punName = "Whisk Taker";
            whisk.towerType = TowerType.Whisk;
            whisk.damage = 6f;
            whisk.attackSpeed = 2f;
            whisk.range = 2f;
            whisk.cost = 45;
            whisk.damageType = DamageType.Sharp;
            whisk.isAreaOfEffect = true;
            whisk.aoeRadius = 1.8f;
            whisk.defaultTargeting = TargetingMode.First;
            whisk.flavorText = "Takes risks. And whisks.";
            whisk.placementQuip = "Whisk it for a biscuit!";
            whisk.sellQuip = "Whisked away.";
            EditorUtility.SetDirty(whisk);

            // --- Colander ---
            var colander = CreateAsset<TowerData>(TOWER_PATH, "TowerData_Colander");
            colander.towerName = "Colander";
            colander.punName = "The Strainer";
            colander.towerType = TowerType.Colander;
            colander.damage = 3f;
            colander.attackSpeed = 0.5f;
            colander.range = 2.5f;
            colander.cost = 55;
            colander.damageType = DamageType.Impact;
            colander.stunChance = 1f;
            colander.stunDuration = 1.5f;
            colander.defaultTargeting = TargetingMode.First;
            colander.flavorText = "Strains out the riff-raff.";
            colander.placementQuip = "Time to strain some noodles!";
            colander.sellQuip = "Drained and discarded.";
            EditorUtility.SetDirty(colander);

            // --- Blowtorch ---
            var blowtorch = CreateAsset<TowerData>(TOWER_PATH, "TowerData_Blowtorch");
            blowtorch.towerName = "Blowtorch";
            blowtorch.punName = "Creme Bruh-lee";
            blowtorch.towerType = TowerType.Blowtorch;
            blowtorch.damage = 20f;
            blowtorch.attackSpeed = 0.7f;
            blowtorch.range = 2.5f;
            blowtorch.cost = 90;
            blowtorch.damageType = DamageType.Heat;
            blowtorch.isAreaOfEffect = true;
            blowtorch.aoeRadius = 1f;
            blowtorch.dotDamage = 5f;
            blowtorch.dotDuration = 2f;
            blowtorch.defaultTargeting = TargetingMode.Strongest;
            blowtorch.flavorText = "Some like it hot. Pasta doesn't.";
            blowtorch.placementQuip = "Fire in the hole! ...kitchen!";
            blowtorch.sellQuip = "Flame off.";
            EditorUtility.SetDirty(blowtorch);

            // --- Meat Tenderizer ---
            var tenderizer = CreateAsset<TowerData>(TOWER_PATH, "TowerData_MeatTenderizer");
            tenderizer.towerName = "Meat Tenderizer";
            tenderizer.punName = "The Tenderoni";
            tenderizer.towerType = TowerType.MeatTenderizer;
            tenderizer.damage = 18f;
            tenderizer.attackSpeed = 0.6f;
            tenderizer.range = 1.8f;
            tenderizer.cost = 70;
            tenderizer.damageType = DamageType.Impact;
            tenderizer.armorReduction = 5f;
            tenderizer.slowAmount = 0.2f;
            tenderizer.slowDuration = 0.5f;
            tenderizer.defaultTargeting = TargetingMode.Strongest;
            tenderizer.flavorText = "Makes everything softer. Especially your enemies.";
            tenderizer.placementQuip = "Tenderizing time!";
            tenderizer.sellQuip = "This meat is done.";
            EditorUtility.SetDirty(tenderizer);

            // --- Frying Pan ---
            var pan = CreateAsset<TowerData>(TOWER_PATH, "TowerData_FryingPan");
            pan.towerName = "Frying Pan";
            pan.punName = "Pan-demonium";
            pan.towerType = TowerType.FryingPan;
            pan.damage = 12f;
            pan.attackSpeed = 0.8f;
            pan.range = 2f;
            pan.cost = 65;
            pan.damageType = DamageType.Heat;
            pan.isAreaOfEffect = true;
            pan.aoeRadius = 1f;
            pan.knockbackForce = 1.5f;
            pan.defaultTargeting = TargetingMode.First;
            pan.flavorText = "Flipping out? So are your enemies.";
            pan.placementQuip = "Pan-tastic placement!";
            pan.sellQuip = "Out of the frying pan...";
            EditorUtility.SetDirty(pan);

            // --- Spice Rack ---
            var spice = CreateAsset<TowerData>(TOWER_PATH, "TowerData_SpiceRack");
            spice.towerName = "Spice Rack";
            spice.punName = "The Flavor Saver";
            spice.towerType = TowerType.SpiceRack;
            spice.damage = 0f;
            spice.attackSpeed = 0f;
            spice.range = 3f;
            spice.cost = 80;
            spice.damageType = DamageType.Seasoning;
            spice.buffDamagePercent = 15f;
            spice.buffSpeedPercent = 10f;
            spice.buffRange = 3f;
            spice.defaultTargeting = TargetingMode.First;
            spice.flavorText = "Makes everything around it better. Like garlic.";
            spice.placementQuip = "Spicing things up!";
            spice.sellQuip = "The flavor has faded.";
            EditorUtility.SetDirty(spice);

            // --- Tip Jar ---
            var tipJar = CreateAsset<TowerData>(TOWER_PATH, "TowerData_TipJar");
            tipJar.towerName = "Tip Jar";
            tipJar.punName = "Penne Pincher";
            tipJar.towerType = TowerType.TipJar;
            tipJar.damage = 0f;
            tipJar.attackSpeed = 0f;
            tipJar.range = 0f;
            tipJar.cost = 100;
            tipJar.damageType = DamageType.Seasoning;
            tipJar.doughPerWave = 25;
            tipJar.defaultTargeting = TargetingMode.First;
            tipJar.flavorText = "Every penne counts.";
            tipJar.placementQuip = "Invest in your future!";
            tipJar.sellQuip = "Cashing out.";
            EditorUtility.SetDirty(tipJar);

            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] 10 Tower Data assets created! Knife work!");
        }

        [MenuItem("Pasta Defence/Create Enemy Data")]
        public static void CreateEnemyData()
        {
            EnsureFolders();

            CreateSingleEnemy("Penne", "Not the sharpest noodle in the box.",
                EnemyType.Penne, 100f, 2f, 0f, 10, EnemyAbility.None);

            CreateSingleEnemy("Spaghetti", "They're coming in hot! And tangled.",
                EnemyType.Spaghetti, 40f, 3.5f, 0f, 5, EnemyAbility.None);

            CreateSingleEnemy("Rigatoni", "Built like a truck. A pasta truck.",
                EnemyType.Rigatoni, 300f, 1f, 8f, 25, EnemyAbility.None,
                impactRes: -0.3f, sharpRes: 0.3f);

            CreateSingleEnemy("Farfalle", "Thinks it's a butterfly. It's not.",
                EnemyType.Farfalle, 80f, 2.5f, 0f, 15, EnemyAbility.Flying);

            var ravioli = CreateSingleEnemy("Ravioli", "Stuffed with confidence. And cheese.",
                EnemyType.Ravioli, 120f, 1.8f, 2f, 15, EnemyAbility.Shielded);
            ravioli.shieldAmount = 50f;
            EditorUtility.SetDirty(ravioli);

            var lasagna = CreateSingleEnemy("Lasagna", "The final boss of every Italian dinner.",
                EnemyType.Lasagna, 1000f, 0.8f, 10f, 100, EnemyAbility.Splitting);
            lasagna.splitCount = 3;
            lasagna.servingsLostOnLeak = 5;
            EditorUtility.SetDirty(lasagna);

            var fusilli = CreateSingleEnemy("Fusilli", "A real twist villain.",
                EnemyType.Fusilli, 90f, 2.2f, 1f, 12, EnemyAbility.Evasive);
            fusilli.evasionChance = 0.3f;
            EditorUtility.SetDirty(fusilli);

            var orzo = CreateSingleEnemy("Orzo", "Is it rice? Is it pasta? It's annoying.",
                EnemyType.Orzo, 60f, 2f, 0f, 8, EnemyAbility.Splitting);
            orzo.splitCount = 3;
            EditorUtility.SetDirty(orzo);

            var mac = CreateSingleEnemy("Macaroni", "The Mac Daddy of support.",
                EnemyType.Macaroni, 80f, 1.5f, 2f, 20, EnemyAbility.Healing);
            mac.healAmount = 5f;
            mac.healInterval = 2f;
            EditorUtility.SetDirty(mac);

            CreateSingleEnemy("Angel Hair", "So thin you can't even see it coming.",
                EnemyType.AngelHair, 50f, 3f, 0f, 12, EnemyAbility.Stealth);

            var tort = CreateSingleEnemy("Tortellini", "Ring ring! Special delivery of pain.",
                EnemyType.Tortellini, 100f, 2f, 3f, 15, EnemyAbility.Dashing);
            tort.dashDistance = 2f;
            tort.dashCooldown = 5f;
            EditorUtility.SetDirty(tort);

            CreateSingleEnemy("Cannelloni", "Loaded and dangerous.",
                EnemyType.Cannelloni, 200f, 1.2f, 5f, 30, EnemyAbility.Siege,
                servingsLost: 3);

            var gnocchi = CreateSingleEnemy("Gnocchi", "Keeps bouncing back. Like a potato.",
                EnemyType.Gnocchi, 150f, 1.5f, 3f, 18, EnemyAbility.Regenerating);
            gnocchi.regenPerSecond = 5f;
            EditorUtility.SetDirty(gnocchi);

            var ling = CreateSingleEnemy("Linguine", "Flat out fast.",
                EnemyType.Linguine, 70f, 2.5f, 1f, 12, EnemyAbility.SpeedAura);
            ling.speedAuraMultiplier = 1.3f;
            ling.speedAuraRange = 2f;
            EditorUtility.SetDirty(ling);

            CreateSingleEnemy("Stuffed Shell", "You never know what you're gonna get.",
                EnemyType.StuffedShell, 250f, 1.5f, 5f, 35, EnemyAbility.Shielded,
                secondary: EnemyAbility.Regenerating);

            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] 15 Enemy Data assets created! Pasta la vista!");
        }

        [MenuItem("Pasta Defence/Create Wave Data")]
        public static void CreateWaveData()
        {
            EnsureFolders();

            string[] waveNames = {
                "The Appetizer", "Noodle Nibble", "Carb Loading", "Penne Ante",
                "Holy Cannelloni!", "The Thickening", "Swarm Warning",
                "Tank Parade", "Mixed Platter", "The Spaghetti Western"
            };
            string[] waveQuips = {
                "Here they come! Al dente and dangerous!",
                "More noodles? The audacity!",
                "They're carbo-loading for battle!",
                "Penne for your thoughts... and your life!",
                "Holy Cannelloni! That's a lot of pasta!",
                "Things are getting thick. Like sauce.",
                "Swarm incoming! Hope you're not gluten intolerant!",
                "They're sending the big noodles now!",
                "A little bit of everything. Chef's surprise!",
                "FINAL WAVE: The Spaghetti Western"
            };

            // Load enemy data references
            var penne = AssetDatabase.LoadAssetAtPath<EnemyData>(ENEMY_PATH + "/EnemyData_Penne.asset");
            var spaghetti = AssetDatabase.LoadAssetAtPath<EnemyData>(ENEMY_PATH + "/EnemyData_Spaghetti.asset");
            var rigatoni = AssetDatabase.LoadAssetAtPath<EnemyData>(ENEMY_PATH + "/EnemyData_Rigatoni.asset");

            for (int i = 0; i < 10; i++)
            {
                var wave = CreateAsset<WaveData>(WAVE_PATH, $"WaveData_{i + 1:D2}_{waveNames[i].Replace(" ", "")}");
                wave.waveName = waveNames[i];
                wave.waveQuip = waveQuips[i];
                wave.waveNumber = i + 1;
                wave.bonusDough = (i + 1) * 5;

                // Scale enemy counts and mix
                switch (i)
                {
                    case 0: // Wave 1 — just penne
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = penne, count = 6, spawnInterval = 0.8f }
                        };
                        break;
                    case 1: // Wave 2 — more penne
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = penne, count = 10, spawnInterval = 0.6f }
                        };
                        break;
                    case 2: // Wave 3 — spaghetti swarm
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = spaghetti, count = 15, spawnInterval = 0.3f }
                        };
                        break;
                    case 3: // Wave 4 — penne + spaghetti
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = penne, count = 8, spawnInterval = 0.6f },
                            new EnemyGroup { enemyData = spaghetti, count = 10, spawnInterval = 0.3f }
                        };
                        break;
                    case 4: // Wave 5 — first rigatoni
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = penne, count = 5, spawnInterval = 0.6f },
                            new EnemyGroup { enemyData = rigatoni, count = 2, spawnInterval = 2f }
                        };
                        break;
                    case 5: // Wave 6
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = spaghetti, count = 20, spawnInterval = 0.25f },
                            new EnemyGroup { enemyData = penne, count = 8, spawnInterval = 0.5f }
                        };
                        break;
                    case 6: // Wave 7
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = spaghetti, count = 25, spawnInterval = 0.2f }
                        };
                        break;
                    case 7: // Wave 8 — tanks
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = rigatoni, count = 5, spawnInterval = 1.5f },
                            new EnemyGroup { enemyData = penne, count = 10, spawnInterval = 0.4f }
                        };
                        break;
                    case 8: // Wave 9 — mixed
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = penne, count = 10, spawnInterval = 0.5f },
                            new EnemyGroup { enemyData = spaghetti, count = 15, spawnInterval = 0.3f },
                            new EnemyGroup { enemyData = rigatoni, count = 3, spawnInterval = 1.5f }
                        };
                        break;
                    case 9: // Wave 10 — final
                        wave.enemyGroups = new EnemyGroup[]
                        {
                            new EnemyGroup { enemyData = rigatoni, count = 5, spawnInterval = 1f },
                            new EnemyGroup { enemyData = spaghetti, count = 20, spawnInterval = 0.2f },
                            new EnemyGroup { enemyData = penne, count = 15, spawnInterval = 0.3f },
                            new EnemyGroup { enemyData = rigatoni, count = 3, spawnInterval = 1.5f }
                        };
                        break;
                }

                EditorUtility.SetDirty(wave);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] 10 Wave Data assets created! Waves of flavor incoming!");
        }

        [MenuItem("Pasta Defence/Create Quip Database")]
        public static void CreateQuipDatabase()
        {
            EnsureFolders();
            var quips = CreateAsset<QuipDatabase>(HUMOR_PATH, "QuipDatabase_Main");
            // Uses the default values already defined in QuipDatabase.cs — 150+ quips
            EditorUtility.SetDirty(quips);
            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] Quip Database created! The puns will flow!");
        }

        [MenuItem("Pasta Defence/Create Map Data")]
        public static void CreateMapData()
        {
            EnsureFolders();

            // Load wave data
            var waves = new WaveData[10];
            for (int i = 0; i < 10; i++)
            {
                string[] names = {
                    "TheAppetizer", "NoodleNibble", "CarbLoading", "PenneAnte",
                    "HolyCannelloni!", "TheThickening", "SwarmWarning",
                    "TankParade", "MixedPlatter", "TheSpaghettiWestern"
                };
                waves[i] = AssetDatabase.LoadAssetAtPath<WaveData>(
                    WAVE_PATH + $"/WaveData_{i + 1:D2}_{names[i]}.asset");
            }

            var map = CreateAsset<MapData>(MAP_PATH, "MapData_CuttingBoard_01");
            map.mapName = "The Cutting Board";
            map.punSubtitle = "Where careers get started... and pasta gets ended.";
            map.theme = MapTheme.CuttingBoard;
            map.worldNumber = 1;
            map.stageNumber = 1;
            map.startingDough = 120;
            map.startingServings = 20;
            map.totalWaves = 10;
            map.waves = waves;
            map.enemyHPMultiplier = 1f;
            map.enemySpeedMultiplier = 1f;
            map.twoStarMaxLeaks = 0;
            map.threeStarBudgetPercent = 0.7f;
            EditorUtility.SetDirty(map);

            AssetDatabase.SaveAssets();
            Debug.Log("[Pasta Defence] Map Data created! The Cutting Board awaits!");
        }

        // --- Helpers ---

        private static T CreateAsset<T>(string folder, string name) where T : ScriptableObject
        {
            string path = $"{folder}/{name}.asset";

            // If it already exists, load it
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static EnemyData CreateSingleEnemy(string name, string flavorText,
            EnemyType type, float hp, float speed, float armor, int reward,
            EnemyAbility primary, EnemyAbility secondary = EnemyAbility.None,
            float heatRes = 0f, float impactRes = 0f, float sharpRes = 0f, float seasonRes = 0f,
            int servingsLost = 1)
        {
            var data = CreateAsset<EnemyData>(ENEMY_PATH, $"EnemyData_{name.Replace(" ", "")}");
            data.enemyName = name;
            data.punName = name;
            data.enemyType = type;
            data.maxHP = hp;
            data.speed = speed;
            data.armor = armor;
            data.doughReward = reward;
            data.primaryAbility = primary;
            data.secondaryAbility = secondary;
            data.heatResistance = heatRes;
            data.impactResistance = impactRes;
            data.sharpResistance = sharpRes;
            data.seasoningResistance = seasonRes;
            data.servingsLostOnLeak = servingsLost;
            data.flavorText = flavorText;
            data.deathQuips = new string[]
            {
                "Pasta la vista, baby.",
                "That's a-more damage than they expected!",
                "Reduced to sauce."
            };
            EditorUtility.SetDirty(data);
            return data;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "ScriptableObjects");
            EnsureFolder(DATA_ROOT, "Towers");
            EnsureFolder(DATA_ROOT, "Enemies");
            EnsureFolder(DATA_ROOT, "Waves");
            EnsureFolder(DATA_ROOT, "Maps");
            EnsureFolder(DATA_ROOT, "Humor");
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{folderName}"))
                AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
