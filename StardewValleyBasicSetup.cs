using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace StardewValleyBasicSetup
{
    public partial class ModConfig
    {
        public int Somevalues { get; set; } = 5;
        public bool SomeOtherValues { get; set; } = false;
    }

    public partial class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public ModConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            generateGMCM();
        }

        private void generateGMCM()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;

            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            gmcm.AddSectionTitle(ModManifest, () => "Sample Title");



        }

    }




}
