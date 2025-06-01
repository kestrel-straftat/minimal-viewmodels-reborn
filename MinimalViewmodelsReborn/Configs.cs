using BepInEx.Configuration;
using UnityEngine;

namespace MinimalViewmodelsReborn;

public static class Configs
{
    public static ConfigEntry<float> ViewmodelOffsetX { get; private set; }
    public static ConfigEntry<float> ViewmodelOffsetY { get; private set; }
    public static ConfigEntry<float> ViewmodelOffsetZ { get; private set; }
    public static ConfigEntry<float> ViewmodelFOV { get; private set; }
    public static ConfigEntry<bool> InvisibleViewmodels { get; private set; }
    
    public static ConfigEntry<float> MuzzleFlashLightIntensity { get; private set; }
    public static ConfigEntry<float> MuzzleFlashScale { get; private set; }
    
    public static ConfigEntry<float> RunFovIncrease { get; private set; }
    public static ConfigEntry<float> SlideFovIncrease { get; private set; }
    public static ConfigEntry<float> RunSlideFovIncrease { get; private set; }

    public static Vector3 ViewmodelOffset => new(ViewmodelOffsetX.Value, ViewmodelOffsetY.Value, ViewmodelOffsetZ.Value);

    internal static void Init(ConfigFile config) {
        InvisibleViewmodels = config.Bind(
            "Viewmodels.Visibility",
            "Invisible Viewmodels",
            false,
            "Hides your weapon completely."
        );
        ViewmodelFOV = config.Bind(
            "Viewmodels.Visibility",
            "Viewmodel FOV",
            75f,
            "Set the FOV of your viewmodel. A higher value will result in a smaller weapon."
        );
        ViewmodelOffsetX = config.Bind(
            "Viewmodels.Offset",
            "Viewmodel X Offset",
            0f,
            new ConfigDescription("Negative values will shift your held weapon left, Positive values will shift it right.", new AcceptableValueRange<float>(-2, 2))
        );
        ViewmodelOffsetY = config.Bind(
            "Viewmodels.Offset",
            "Viewmodel Y Offset",
            -0.1f,
            new ConfigDescription("Negative values will shift your held weapon down, Positive values will shift it up.", new AcceptableValueRange<float>(-2, 2))
        );
        ViewmodelOffsetZ = config.Bind(
            "Viewmodels.Offset",
            "Viewmodel Z Offset",
            0f,
            new ConfigDescription("Negative values will shift your held weapon back, Positive values will shift it forward.", new AcceptableValueRange<float>(-2, 2))
        );

        MuzzleFlashLightIntensity = config.Bind(
            "MuzzleFlashes.General",
            "Muzzle Flash Light Intensity",
            1f,
            "A multiplier applied to the light intensity of muzzle flashes. Requires a map restart to apply."
        );
        MuzzleFlashScale = config.Bind(
            "MuzzleFlashes.General",
            "Muzzle Flash Scale",
            1f,
            "A multiplier applied to the scale of muzzle flashes. Requires a map restart to apply."
        );

        RunFovIncrease = config.Bind(
            "DynamicFov.General",
            "Run Fov Increase",
            15f,
            "When sprinting, your FOV will be increased by this amount."
        );
        SlideFovIncrease = config.Bind(
            "DynamicFov.General",
            "Slide Fov Increase",
            12f,
            "When sliding, your FOV will be increased by this amount."
        );
        RunSlideFovIncrease = config.Bind(
            "DynamicFov.General",
            "Run Slide Fov Increase",
            15f,
            "When sprinting and sliding, your FOV will be increased by this amount."
        );
    }
}