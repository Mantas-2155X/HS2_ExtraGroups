using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;

using HarmonyLib;

namespace HS2_ExtraGroups
{
    [BepInProcess("HoneySelect2")]
    [BepInPlugin(nameof(HS2_ExtraGroups), nameof(HS2_ExtraGroups), VERSION)]
    public class HS2_ExtraGroups : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";

        public new static ManualLogSource Logger;
        
        public static int groupCount = 5;
        public static int girlCount = 20;
        private static ConfigEntry<int> GroupCount { get; set; }
        private static ConfigEntry<int> GirlCount { get; set; }
        
        private void Awake()
        {
            Logger = base.Logger;
            
            GroupCount = Config.Bind("Requires restart! Modifies save!", "Groups Count", 5, new ConfigDescription("Requires a restart to apply.", new AcceptableValueRange<int>(5, 99)));
            groupCount = GroupCount.Value;
            
            GirlCount = Config.Bind("Requires restart! Modifies save!", "Girls Count", 20, new ConfigDescription("Requires a restart to apply.", new AcceptableValueRange<int>(20, 99)));
            girlCount = GirlCount.Value;
            
            var harmony = new Harmony(nameof(HS2_ExtraGroups));
            harmony.PatchAll(typeof(Hooks));
            
            Hooks.PatchSpecial(harmony);
        }
    }
}