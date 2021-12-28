using System;

namespace Celeste.Mod.GreenylieHelper
{
    public class GreenylieHelperModule : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static GreenylieHelperModule Instance;

        public GreenylieHelperModule()
        {
            Instance = this;
        }

        public override Type SettingsType => typeof(GreenylieHelperSettings);
        public static GreenylieHelperSettings Settings => (GreenylieHelperSettings)Instance._Settings;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
        }

        // Optional, do anything requiring either the Celeste or mod content here.
        public override void LoadContent(bool firstLoad)
        {
        }

        // Unload the entirety of your mod's content. Free up any native resources.
        public override void Unload()
        {
        }
    }
}
