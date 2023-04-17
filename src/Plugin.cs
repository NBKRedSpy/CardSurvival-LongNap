using System.Globalization;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CardSurvival_LongNap
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log { get; set; }

        public static LocalizationInfo LocalizationInfo { get; set; }

        public static string ModFolder { get; private set; }

        private void Awake()
        {
            Log = Logger;

            ModFolder = Path.GetDirectoryName(Info.Location);

            Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

        }

        public static void LogInfo(string text)
        {
            Plugin.Log.LogInfo(text);
        }

        public static string GetGameObjectPath(GameObject obj)
        {
            GameObject searchObject = obj;

            string path = "/" + searchObject.name;
            while (searchObject.transform.parent != null)
            {
                searchObject = searchObject.transform.parent.gameObject;
                path = "/" + searchObject.name + path;
            }
            return path;
        }

    }

    public static class PluginExtensions
    {
        public static void SetLocalization(this ref LocalizedString localizedString)
        {
            Plugin.LocalizationInfo.SetLocalization(ref localizedString);
        }
    }

}