
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using Spacechase.Shared.Patching;
using StardewValley.Menus;
using StardewValleyBasicSetup;

/// <summary>Applies Harmony patches to <see cref="GameLocation"/>.</summary>

internal class PatchName : BasePatcher
{

    public readonly Config _config;
    public static ModEntry Instance;

    public PatchName(Config Config)
    {
        _config = Config;
    }


    public PatchName(Config Config, ModEntry _instance)
    {
        _config = Config;
        Instance = _instance;
    }




    public override void Apply(Harmony harmony, IMonitor monitor)
    {

        //Example of a postfix use case, code runes original function to get the profession name and text description on Miner (level 5), afterwards
        //Postfix will run our own version in this example is "ModifyProfessionDescription" and pass the ref string value so that we can replace the profession name and text description with our own description and profession.
        harmony.Patch(
            original: this.RequireMethod<LevelUpMenu>(
            nameof(LevelUpMenu.getProfessionDescription )
        ),
        postfix: this.GetHarmonyMethod(nameof(ModifyProfessionDescription)) 
        );


    }


    public static void ModifyProfessionDescription(int whichProfession, ref List<string> __result)
    {
        // Miner Profession ID is 18 (Miner)
        if (whichProfession == 18)
        {
            __result.Clear();
            __result.Add("Master Miner"); 
            __result.Add("Gain +" + Instance.Config.Somevalues + " ore per mined rock instead of +1!");
            
        }
    }


}