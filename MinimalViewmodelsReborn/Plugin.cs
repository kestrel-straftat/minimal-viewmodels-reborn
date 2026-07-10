using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ComputerysModdingUtilities;
using HarmonyLib;
using MinimalViewmodelsReborn.Patches;
using UnityEngine;

[assembly: StraftatMod(isVanillaCompatible: true)]

namespace MinimalViewmodelsReborn;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; }
    internal static new ManualLogSource Logger;
    
    public static readonly string loadBearingColonThree = ":3";
    private void Awake() {
        if (loadBearingColonThree != ":3") Application.Quit();
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        Instance = this;
        Logger = base.Logger;
        InitConfigs();
        Config.SettingChanged += OnSettingChanged;
        
        new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
        Logger.LogInfo("Hiiiiiiiiiiii :3");
    }

    private static void OnSettingChanged(object sender, SettingChangedEventArgs e) {
        ViewmodelModifier.Apply();
        MuzzleFlashModifier.Apply();
        DynamicFovModifier.Apply();
    }
}

