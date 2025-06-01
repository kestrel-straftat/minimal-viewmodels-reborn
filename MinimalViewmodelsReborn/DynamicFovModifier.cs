using HarmonyLib;

namespace MinimalViewmodelsReborn;

public static class DynamicFovModifier
{
    public static void Apply() {
        var controller = Settings.Instance.localPlayer;
        if (!controller) return;

        controller.distToRunFov = Configs.RunFovIncrease.Value;
        controller.distToSlideFov = Configs.SlideFovIncrease.Value;
        controller.distToRunSlideFov = Configs.RunSlideFovIncrease.Value;
    }

    [HarmonyPatch(typeof(FirstPersonController))]
    public static class FirstPersonControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void ApplyOnStart() => Apply();
    }
}