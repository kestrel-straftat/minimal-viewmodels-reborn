using HarmonyLib;

namespace MinimalViewmodelsReborn.Patches;

public static class DynamicFovModifier
{
    private static FirstPersonController m_controller; 
    
    public static void Apply() {
        if (!m_controller) return;

        m_controller.distToRunFov = Plugin.RunFovIncrease.Value;
        m_controller.distToSlideFov = Plugin.SlideFovIncrease.Value;
        m_controller.distToRunSlideFov = Plugin.RunSlideFovIncrease.Value;
    }

    [HarmonyPatch(typeof(FirstPersonController))]
    public static class FirstPersonControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void ApplyOnStart(FirstPersonController __instance) {
            m_controller = __instance;
            Apply();
        }
    }
}