using BepInEx.Configuration;
using UnityEngine;

namespace MinimalViewmodelsReborn;

public partial class Plugin
{
    // Viewmodels.Visibility
    public static ConfigEntry<bool> InvisibleViewmodels { get; private set; }
    public static ConfigEntry<bool> InvisibleArms { get; private set; }
    public static ConfigEntry<float> ViewmodelFOV { get; private set; }
    public static ConfigEntry<bool> MirrorViewmodel { get; private set; }
    
    // Viewmodels.Offset
    public static ConfigEntry<float> ViewmodelOffsetX { get; private set; }
    public static ConfigEntry<float> ViewmodelOffsetY { get; private set; }
    public static ConfigEntry<float> ViewmodelOffsetZ { get; private set; }
    
    // Viewmodels.Animation
    public static ConfigEntry<float> ViewmodelSwayScale { get; private set; }
    public static ConfigEntry<float> ViewmodelBobScale { get; private set; }

    // VFX.General
    public static ConfigEntry<bool> HideBulletTrails { get; private set; }
    
    // VFX.MuzzleFlashes
    public static ConfigEntry<float> MuzzleFlashLightIntensity { get; private set; }
    public static ConfigEntry<float> MuzzleFlashScale { get; private set; }

    // DynamicFov.General
    public static ConfigEntry<float> RunFovIncrease { get; private set; }
    public static ConfigEntry<float> SlideFovIncrease { get; private set; }
    public static ConfigEntry<float> RunSlideFovIncrease { get; private set; }
    
    public static Vector3 ViewmodelOffset => new(ViewmodelOffsetX.Value, ViewmodelOffsetY.Value, ViewmodelOffsetZ.Value);

    internal void InitConfigs() {
        
        // Viewmodels.Visibility
        
        InvisibleViewmodels = Config.Bind(
            "Viewmodels.Visibility",
            "Invisible Viewmodels",
            false,
            "Hides your weapon completely."
        );
        InvisibleArms = Config.Bind(
            "Viewmodels.Visibility",
            "Invisible Arms",
            false,
            "Hides your arms."
        );
        ViewmodelFOV = Config.Bind(
            "Viewmodels.Visibility",
            "Viewmodel FOV",
            75f,
            "Set the FOV of the viewmodel camera."
        );
        MirrorViewmodel = Config.Bind(
            "Viewmodels.Visibility",
            "Mirror Viewmodel",
            false,
            "Mirrors your viewmodel."
        );
        
        // Viewmodels.Offset
        
        ViewmodelOffsetX = Config.Bind(
            "Viewmodels.Offset",
            "Viewmodel X Offset",
            0f,
            new ConfigDescription("Negative values will shift your held weapon left, Positive values will shift it right.", new AcceptableValueRange<float>(-5, 5))
        );
        ViewmodelOffsetY = Config.Bind(
            "Viewmodels.Offset",
            "Viewmodel Y Offset",
            -0.1f,
            new ConfigDescription("Negative values will shift your held weapon down, Positive values will shift it up.", new AcceptableValueRange<float>(-5, 5))
        );
        ViewmodelOffsetZ = Config.Bind(
            "Viewmodels.Offset",
            "Viewmodel Z Offset",
            0f,
            new ConfigDescription("Negative values will shift your held weapon back, Positive values will shift it forward.", new AcceptableValueRange<float>(-5, 5))
        );

        // Viewmodels.Animation

        ViewmodelSwayScale = Config.Bind(
            "Viewmodels.Animation",
            "Viewmodel Sway Scale",
            1.0f,
            new ConfigDescription("A multiplier applied to the viewmodel sway when moving the camera and jumping.", new AcceptableValueRange<float>(0, 2))
        );
        ViewmodelBobScale = Config.Bind(
            "Viewmodels.Animation",
            "Viewmodel Bob Scale",
            1.0f,
            new ConfigDescription("A multiplier applied to the viewmodel bobbing animation.", new AcceptableValueRange<float>(0, 2))
        );
        
        // VFX.General
        
        HideBulletTrails = Config.Bind(
            "VFX.General",
            "Hide Bullet Trails",
            false,
            "Hides bullet trails."
        );
        
        // VFX.MuzzleFlashes
        
        /*
        MuzzleFlashLightIntensity = Config.Bind(
            "VFX.MuzzleFlashes",
            "Muzzle Flash Light Intensity",
            1f,
            "A multiplier applied to the light intensity of muzzle flashes. Requires a map restart to apply."
        );
        */
        MuzzleFlashScale = Config.Bind(
            "VFX.MuzzleFlashes",
            "Muzzle Flash Scale",
            1f,
            "A multiplier applied to the scale of muzzle flashes. Requires a map restart to apply."
        );
        
        // DynamicFov.General

        RunFovIncrease = Config.Bind(
            "DynamicFov.General",
            "Run Fov Increase",
            15f,
            "When sprinting, your FOV will be increased by this amount."
        );
        SlideFovIncrease = Config.Bind(
            "DynamicFov.General",
            "Slide Fov Increase",
            12f,
            "When sliding, your FOV will be increased by this amount."
        );
        RunSlideFovIncrease = Config.Bind(
            "DynamicFov.General",
            "Run Slide Fov Increase",
            15f,
            "When sprinting and sliding, your FOV will be increased by this amount."
        );
    }
}