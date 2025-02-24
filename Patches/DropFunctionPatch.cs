using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Monsters;
using StardewValley;
using System.Collections.Generic;
using System;
using HarmonyLib;
using StardewValley.Locations;
using Spacechase.Shared.Patching;
using Netcode;
using static HarmonyLib.Code;
using System.Reflection;
using System.Text.Json.Nodes;
using System.IO;
using System.Xml.Linq;

using static MoreMonsterDrops.ModEntry;
using MoreMonsterDrops;
using StardewValley.Constants;
using StardewValley.Objects.Trinkets;

/// <summary>Applies Harmony patches to <see cref="GameLocation"/>.</summary>


internal class DropFunctionPatch : BasePatcher
{
    public Config Config { get; private set; }
    public static ModEntry Instance { get; private set; }

    public DropFunctionPatch(Config config, ModEntry _instance)
    {
        Config = config;
        Instance = _instance;
    }
    public override void Apply(Harmony harmony, IMonitor monitor)
    {

        harmony.Patch(
            original: this.RequireMethod<GameLocation>(nameof(GameLocation.monsterDrop)),
            postfix: this.GetHarmonyMethod(nameof(Before_MonsterDrop))

        );


    }


    private static void Before_MonsterDrop(Monster monster, int x, int y, Farmer who)
    {
        if (Instance.Config.TurnOnMoreMonsterDrops)
        {
            int number = (int)(Instance.Config.NumberOfTimesDropFunctionExecutesPerCombatLevel * who.CombatLevel) + Instance.Config.ExtraNumberOfTimesDropFunctionExecuteRegardlessOfCombatLevel;

            //runs drop function multiple times
            for (int p = 0; p < number; p++)
            {


                IList<string> objectsToDrop = monster.objectsToDrop;
                Vector2 vector = Utility.PointToVector2(who.StandingPixel);
                List<Item> extraDropItems = monster.getExtraDropItems();
                if (who.isWearingRing("526") && DataLoader.Monsters(Game1.content).TryGetValue(monster.Name, out var value))
                {
                    string[] array = ArgUtility.SplitBySpace(value.Split('/')[6]);
                    for (int i = 0; i < array.Length; i += 2)
                    {
                        if (Game1.random.NextDouble() < Convert.ToDouble(array[i + 1]))
                        {
                            objectsToDrop.Add(array[i]);
                        }
                    }
                }

                List<Debris> list = new List<Debris>();
                for (int j = 0; j < objectsToDrop.Count; j++)
                {
                    string text = objectsToDrop[j];
                    if (text != null && text.StartsWith('-') && int.TryParse(text, out var result))
                    {
                        list.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(result), Game1.random.Next(1, 4), new Vector2(x, y), vector)));
                    }
                    else
                    {
                        list.Add(monster.ModifyMonsterLoot(new Debris(text, new Vector2(x, y), vector)));
                    }
                }

                for (int k = 0; k < extraDropItems.Count; k++)
                {
                    list.Add(monster.ModifyMonsterLoot(new Debris(extraDropItems[k], new Vector2(x, y), vector)));
                }

                Trinket.TrySpawnTrinket(Game1.player.currentLocation, monster, monster.getStandingPosition());
                if (who.isWearingRing("526"))
                {
                    extraDropItems = monster.getExtraDropItems();
                    for (int l = 0; l < extraDropItems.Count; l++)
                    {
                        Item one = extraDropItems[l].getOne();
                        one.Stack = extraDropItems[l].Stack;
                        one.HasBeenInInventory = false;
                        list.Add(monster.ModifyMonsterLoot(new Debris(one, new Vector2(x, y), vector)));
                    }
                }

                foreach (Debris item2 in list)
                {
                    monster.currentLocation.debris.Add(item2);
                }

                if (who.stats.Get("Book_Void") != 0 && Game1.random.NextDouble() < 0.03 && list != null && monster != null)
                {
                    foreach (Debris item3 in list)
                    {
                        if (item3.item != null)
                        {
                            Item one2 = item3.item.getOne();
                            if (one2 != null)
                            {
                                one2.Stack = item3.item.Stack;
                                one2.HasBeenInInventory = false;
                                Game1.player.currentLocation.debris.Add(monster.ModifyMonsterLoot(new Debris(one2, new Vector2(x, y), vector)));
                            }
                        }
                        else if (item3.itemId.Value != null && item3.itemId.Value.Length > 0)
                        {
                            Item item = ItemRegistry.Create(item3.itemId.Value);
                            item.HasBeenInInventory = false;
                            monster.currentLocation.debris.Add(monster.ModifyMonsterLoot(new Debris(item, new Vector2(x, y), vector)));
                        }
                    }
                }

                if (Game1.currentLocation.HasUnlockedAreaSecretNotes(who) && Game1.random.NextDouble() < 0.033)
                {
                    StardewValley.Object @object = Game1.currentLocation.tryToCreateUnseenSecretNote(who);
                    if (@object != null)
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(@object, new Vector2(x, y), -1, Game1.player.currentLocation));
                    }
                }

                Utility.trySpawnRareObject(who, new Vector2(x, y), monster.currentLocation, 1.5);
                if (Utility.tryRollMysteryBox(0.01 + who.team.AverageDailyLuck() / 10.0 + (double)who.LuckLevel * 0.008))
                {
                    monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create((who.stats.Get(StatKeys.Mastery(2)) != 0) ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2(x, y), -1, Game1.player.currentLocation));
                }

                if (who.stats.MonstersKilled > 10 && Game1.random.NextDouble() < 0.0001 + ((!who.mailReceived.Contains("voidBookDropped")) ? ((double)who.stats.MonstersKilled * 1.5E-05) : 0.0004))
                {
                    monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)Book_Void"), new Vector2(x, y), -1, Game1.player.currentLocation));
                    who.mailReceived.Add("voidBookDropped");
                }

                if (Game1.currentLocation is Woods && Game1.random.NextDouble() < 0.1)
                {
                    monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)292"), new Vector2(x, y), -1, Game1.player.currentLocation));
                }

                if (Game1.netWorldState.Value.GoldenWalnutsFound >= 100)
                {
                    if (monster.isHardModeMonster.Value && Game1.stats.Get("hardModeMonstersKilled") > 50 && Game1.random.NextDouble() < 0.001 + (double)((float)who.LuckLevel * 0.0002f))
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)896"), new Vector2(x, y), -1, Game1.player.currentLocation));
                    }
                    else if (monster.isHardModeMonster.Value && Game1.random.NextDouble() < 0.008 + (double)((float)who.LuckLevel * 0.002f))
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)858"), new Vector2(x, y), -1, Game1.player.currentLocation));
                    }
                }
            }

        }


    }
}




