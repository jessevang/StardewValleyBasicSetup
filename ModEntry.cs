using GenericModConfigMenu;
using LeFauxMods.Common.Integrations.IconicFramework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace StardewValleyBasicSetup
{
    public class ModConfig
    {
        public string Mode { get; set; } = "UnifiedExperience";
        public string AbilityUses { get; set; } = "Energy";
        public KeybindList AbilityHotkey { get; set; } = new(
                  new Keybind(SButton.Q),
                  new Keybind(SButton.ControllerA)
        );

        public float BaseEnergyDrain { get; set; } = 3.0f;
    }

    public partial class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public ModConfig Config { get; set; }
        private UnifiedExperienceSystem.IUnifiedExperienceAPI? uesApi;


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            registerGMCM();
            registerIconicFramework();
            registerUES();
        }

        private void registerGMCM()
        {
            // Uses Generic Mod Config Menu API to build a config UI.
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;

            // Register the mod.
            gmcm.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            ITranslationHelper i18n = Helper.Translation;


            gmcm.AddSectionTitle(
                  ModManifest,
                  text: () => "Section Title"
              );
            gmcm.AddParagraph(
                ModManifest,
                text: () => "This is a paragraph in mod config menu"
            );


            gmcm.AddTextOption(
                mod: ModManifest,
                name: () => i18n.Get("config.mode.name"),
                tooltip: () => i18n.Get("config.mode.tooltip"),
                getValue: () => Config.Mode,
                setValue: value => Config.Mode = value,
                allowedValues: new[] { "Standalone", "UnifiedExperience" }
            );

            gmcm.AddTextOption(
                mod: ModManifest,
                name: () => i18n.Get("config.AbilityUses.name"),
                tooltip: () => i18n.Get("config.AbilityUses.tooltip"),
                getValue: () => Config.AbilityUses,
                setValue: value => Config.AbilityUses = value,
                allowedValues: new[] { "Energy", "Stamina" }
            );





        }

        private void registerIconicFramework()
        {
            var iconicFramework = Helper.ModRegistry.GetApi<IIconicFrameworkApi>("furyx639.ToolbarIcons");
            if (iconicFramework is null)
            {
                Monitor.Log("Iconic Framework not found, skipping toolbar icon registration.", LogLevel.Info);
                return;
            }

            ITranslationHelper I18n = Helper.Translation;

            iconicFramework.AddToolbarIcon(
                id: $"{ModManifest.UniqueID}.Spin",
                texturePath: "Tilesheets/bobbers",
                    sourceRect: new Rectangle(50, 130, 11, 12),
                getTitle: () => I18n.Get("Icon.Spin.name"),
                getDescription: () => I18n.Get("Icon.Spin.tooltip"),
                onClick: () => doSomething()
            );




        }


        private void registerUES()
        {
            //Registers Mod,
            uesApi = Helper.ModRegistry.GetApi<UnifiedExperienceSystem.IUnifiedExperienceAPI>("Darkmushu.UnifiedExperienceSystem");

            if (uesApi == null)
            {
                return;
            }

            // Linear curve — XP is fixed per level, need 1k to reach level 1, if level was set to 10, then need 10k xp to reach level 10.
            uesApi.RegisterAbility(
                modUniqueId: this.ModManifest.UniqueID,
                abilityId: "FlyingWeaponMountIgnoresCollision",
                displayName: "Flying Weapon Mount Flys Over Everything",
                description: "Unlocks ability fly anywhere.",
                curveKind: "linear",
                curveData: new Dictionary<string, object>
                {
        { "xpPerLevel", 1000 }
                },
                maxLevel: 1
            );

            //Step Curve - base 200xp, next level is 400+200, next one is 600+200... at level 10 it's 400+(10-1)*200 = 2200, total experience need to reach 10 is 13k xp
            uesApi.RegisterAbility(
                modUniqueId: this.ModManifest.UniqueID,
                abilityId: "FlyingWeaponMountSpeed",
                displayName: "Flying Weapon Mount Speed",
                description: "Increase weapon mount Speed by .25 per Level.",
                curveKind: "step",
                curveData: new Dictionary<string, object>
                {
                { "base", 400 },
                { "step", 200 }
                },
                maxLevel: 10
            );

            //Table Curve - Level 1 needs 100 XP, level 2 needs 200xp, level 3 need 300 xp, level 10 needs 1500xp, total xp to reach level 10 is 7300XP.
            uesApi.RegisterAbility(
                modUniqueId: this.ModManifest.UniqueID,
                abilityId: "FlyingWeaponMountStaminaDrain",
                displayName: "Reduce Stamina Drain",
                description: "Increase weapon mount Speed by .25 per Level",
                curveKind: "table",
                curveData: new Dictionary<string, object>
                {
                { "levels", new int[] { 100, 200, 300, 500, 600, 800, 1000, 1100, 1200, 1500 } }
                },
                maxLevel: 10
            );
        }



        private void doSomething()
        {
            //Do Something
        }

    }

}





