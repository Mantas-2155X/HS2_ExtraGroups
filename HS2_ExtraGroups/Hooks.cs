using System.Linq;

using System.Reflection;
using System.Reflection.Emit;

using System.Collections.Generic;

using HarmonyLib;

using HS2;
using Config;
using Manager;

namespace HS2_ExtraGroups
{
    public static class Hooks
    {
        public static void PatchSpecial(Harmony harmony)
        {
            {
                var iteratorType = AccessTools.TypeByName("ADV.ADVMainScene");
                var iteratorMethod = AccessTools.Method(iteratorType, "CharacterDelete");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(ADVMainScene_CharacterDelete_IncreaseRoomsList));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
            
            {
                var iteratorType = typeof(GroupListUI).GetNestedType("<Start>d__13", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "MoveNext");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(GroupListUI_Start_CustomList));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
            
            {
                var iteratorType = typeof(CoordinateGroupListUI).GetNestedType("<Start>d__13", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "MoveNext");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(CoordinateGroupListUI_Start_CustomList));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
            
            {
                var iteratorType = typeof(GroupCharaSelectUI).GetNestedType("<Start>d__13", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "MoveNext");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(GroupCharaSelectUI_Start_CustomList));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
            
            {
                var iteratorType = typeof(MaleCharaSelectUI).GetNestedType("<Start>d__18", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "MoveNext");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(MaleCharaSelectUI_Start_CustomList));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
            
            {
                var iteratorType = typeof(ConfigCharaSelectUI).GetNestedType("<Start>d__12", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "MoveNext");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(ConfigCharaSelectUI_Start_CustomList));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(GroupListUI), "Start")]
        public static void GroupListUI_Start_CreateUI(GroupListUI __instance, GroupCharaSelectUI ___groupCharaSelectUI)
        {
            Tools.ExpandUI(__instance, ___groupCharaSelectUI);
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(HomeSceneManager), "Start")]
        public static void HomeSceneManager_Start_CustomNamesList(ref string[][] ___txHomeSelectGroupString)
        {
            ___txHomeSelectGroupString = new string[HS2_ExtraGroups.groupCount][];

            for (var i = 0; i < HS2_ExtraGroups.groupCount; i++)
            {
                ___txHomeSelectGroupString[i] = new[]
                {
                    "グループ" + (i + 1) + "が設定中",
                    i + 1 + " Selet a Group",
                    i + 1 + " Selet a Group",
                    i + 1 + " Selet a Group",
                    ""
                };
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SaveData), "Initialize")]
        public static void SaveData_Initialize_CustomRoomClothsLists(SaveData __instance)
        {
            __instance.roomList = new List<string>[HS2_ExtraGroups.groupCount];
            __instance.dicCloths = new Dictionary<string, ClothPngInfo>[HS2_ExtraGroups.groupCount];
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(SaveData), "Copy")]
        public static void SaveData_Copy_CustomRoomClothsLists(SaveData __instance, ref SaveData source)
        {
            __instance.roomList = new List<string>[HS2_ExtraGroups.groupCount];
            __instance.dicCloths = new Dictionary<string, ClothPngInfo>[HS2_ExtraGroups.groupCount];

            var oldList = source.roomList;
            source.roomList = new List<string>[HS2_ExtraGroups.groupCount];
            for (var i = 0; i < source.roomList.Length; i++)
            {
                if (i < oldList.Length)
                    source.roomList[i] = oldList[i];
                else
                    source.roomList[i] = new List<string>();
            }
            
            var oldDict = source.dicCloths;
            source.dicCloths = new Dictionary<string, ClothPngInfo>[HS2_ExtraGroups.groupCount];
            for (var i = 0; i < source.dicCloths.Length; i++)
            {
                if (i < oldDict.Length)
                    source.dicCloths[i] = oldDict[i];
                else
                    source.dicCloths[i] = new Dictionary<string, ClothPngInfo>();
            }
        }
        
        private static IEnumerable<CodeInstruction> ADVMainScene_CharacterDelete_IncreaseRoomsList(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
           
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_5);
            if (index <= 0)
            {
                HS2_ExtraGroups.Logger.LogMessage("Failed transpiling 'ADVMainScene_CharacterDelete_IncreaseRoomsList' '5' index not found!");
                HS2_ExtraGroups.Logger.LogWarning("Failed transpiling 'ADVMainScene_CharacterDelete_IncreaseRoomsList' '5' index not found!");
                return il;
            }
            
            il[index].operand = HS2_ExtraGroups.groupCount;

            return il;
        }
        
        private static IEnumerable<CodeInstruction> CustomList(IEnumerable<CodeInstruction> instructions, string name)
        {
            var il = instructions.ToList();
           
            var startindex = il.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld && (instruction.operand as FieldInfo)?.Name == "charaLists");
            if (startindex <= 0)
            {
                HS2_ExtraGroups.Logger.LogMessage("Failed transpiling '" + name + "' 'charaLists' index not found!");
                HS2_ExtraGroups.Logger.LogWarning("Failed transpiling '" + name + "' 'charaLists' index not found!");
                return il;
            }
            
            var endindex = il.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "Init");
            if (endindex <= 0)
            {
                HS2_ExtraGroups.Logger.LogMessage("Failed transpiling '" + name + "' 'Init' index not found!");
                HS2_ExtraGroups.Logger.LogWarning("Failed transpiling '" + name + "' 'Init' index not found!");
                return il;
            }

            for (var i = startindex + 1; i < endindex; i++)
                il[i].opcode = OpCodes.Nop;

            il[endindex - 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Tools), nameof(Tools.NewGroupsList)));

            return il;
        }

        [HarmonyTranspiler, HarmonyPatch(typeof(GroupListUI), "Create")]
        private static IEnumerable<CodeInstruction> GroupListUI_Create_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "GroupListUI_Create_CustomList");
        
        [HarmonyTranspiler, HarmonyPatch(typeof(CoordinateGroupListUI), "Create")]
        private static IEnumerable<CodeInstruction> CoordinateGroupListUI_Create_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "CoordinateGroupListUI_Create_CustomList");
        
        [HarmonyTranspiler, HarmonyPatch(typeof(ConfigCharaSelectUI), "ReDrawListView")]
        private static IEnumerable<CodeInstruction> ConfigCharaSelectUI_ReDrawListView_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "ConfigCharaSelectUI_ReDrawListView_CustomList");
        
        private static IEnumerable<CodeInstruction> GroupListUI_Start_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "GroupListUI_Start_CustomList");
        
        private static IEnumerable<CodeInstruction> CoordinateGroupListUI_Start_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "CoordinateGroupListUI_Start_CustomList");
        
        private static IEnumerable<CodeInstruction> GroupCharaSelectUI_Start_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "GroupCharaSelectUI_Start_CustomList");
        
        private static IEnumerable<CodeInstruction> MaleCharaSelectUI_Start_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "MaleCharaSelectUI_Start_CustomList");
        
        private static IEnumerable<CodeInstruction> ConfigCharaSelectUI_Start_CustomList(IEnumerable<CodeInstruction> instructions) => CustomList(instructions, "ConfigCharaSelectUI_Start_CustomList");
    }
}