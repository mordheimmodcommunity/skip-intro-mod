using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace SkipIntro
{
    public class Main
    {
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            mod = modEntry;

            return true; // If false the mod will show an error.
        }
    }

    [HarmonyPatch]
    static class CopyrightManager_FadeToMainMenu_Patch
    {
        // This attribute tells Harmony to replace the body of the method with a call to the original method.
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CopyrightManager), "FadeToMainMenu")]
        public static void FadeToMainMenu(object instance)
        {
            // This method is only here so we can call it from our Postfix.
            // Its body is replaced by Harmony.
            // The 'instance' parameter represents the CopyrightManager instance
        }
    }

    [HarmonyPatch(typeof(CopyrightManager))]
    [HarmonyPatch("Start")]
    static class CopyrightManager_Start_Patch
    {
        static void Postfix(CopyrightManager __instance)
        {
            try
            {
                CopyrightManager_FadeToMainMenu_Patch.FadeToMainMenu(__instance);
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
            catch (Exception e)
            {
                Main.mod.Logger.Error(e.ToString());
            }
        }
    }
}
