
using StardewModdingAPI;

using Spacechase.Shared.Patching;


namespace MoreMonsterDrops
{


    public class Config
    {
        public bool TurnOnMoreMonsterDrops { get; set; } = true;
        public double NumberOfTimesDropFunctionExecutesPerCombatLevel { get; set; } = 0.5;
        public int ExtraNumberOfTimesDropFunctionExecuteRegardlessOfCombatLevel { get; set; } = 5;
    }


    internal sealed class ModEntry : Mod
    {

        public static ModEntry Instance { get; private set; }
        public Config Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<Config>() ?? new Config();

            //This Harmony Patcher turns on more monster drops
            HarmonyPatcher.Apply(this,
            new DropFunctionPatch(Config, Instance)
            );

        }






    }
}

