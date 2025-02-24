using HarmonyLib;
using Spacechase.Shared.Patching;
using StardewModdingAPI;
using StardewValley;

namespace StardewValleyBasicSetup
{
    public class Config
    {
        public int Somevalues { get; set; } = 5;
        public bool SomeOtherValues { get; set; } = false;

    }

    internal sealed class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public Config Config { get; private set; }

        
        public override void Entry(IModHelper helper)
        {
            Instance = this; 
            Config = helper.ReadConfig<Config>() ?? new Config(); // Sample to call --> int somevalue = Instance.Config.Somevalues; 

            //Sample to Apply a Patch from patches folder. Used this if you want to use a replace an existing stardew valley function some other other code.
            HarmonyPatcher.Apply(this,
            new PatchName(Config, Instance)
            );

            //use helper events examples that can be leveraged or removed.
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted; 
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding; 
            helper.Events.Input.ButtonPressed += Input_ButtonPressed; 
        }


        // Save config method for later use
        public void SaveConfig()
        {
            Helper.WriteConfig(Config);
        }

        private void GameLoop_DayEnding(object? sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            

        }

        private void Input_ButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            ;
        }

        private void GameLoop_DayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

        }
    }
}