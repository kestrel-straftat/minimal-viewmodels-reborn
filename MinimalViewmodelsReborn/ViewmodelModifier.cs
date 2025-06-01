using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MinimalViewmodelsReborn;

public static class ViewmodelModifier
{
    public static Camera WeaponCam { get; private set; }
    
    // apply transformations to the viewmodel camera
    public static void ApplyTransforms() {
        if (!WeaponCam) return;
        WeaponCam.enabled = !Configs.InvisibleViewmodels.Value;
        if (Configs.InvisibleViewmodels.Value) return;
        
        WeaponCam.fieldOfView = Configs.ViewmodelFOV.Value;

        WeaponCam.transform.localPosition = -Configs.ViewmodelOffset;
    }
    
    // ok. that was the easy bit. now it's time to fix everything that broke!
    
    // needed to make the viewmodel customisation work on medium/low graphics
    // as weapons are rendered by the main cam on those settings
    [HarmonyPatch(typeof(PlayerSetup))]
    public static class PlayerSetupPatch
    {
        [HarmonyPatch("OnStartClient")]
        [HarmonyPostfix]
        private static void FixCameras(PlayerSetup __instance, Camera[] ___cameras, LayerMask ___highMask) {
            if (!__instance.IsOwner) return;
            ___cameras[0].cullingMask = ___highMask;
            WeaponCam = ___cameras[1];
            
            // force the weapon camera to be enabled
            WeaponCam.enabled = true;
            
            // to prevent bullet trails from being cut off when being rendered by the weapon cam
            WeaponCam.farClipPlane = 100;
            
            // if on medium/low graphics
            if (Settings.Instance.qualitySetting < 2) {
                // disable unwanted prost processing (adds fxaa)
                WeaponCam.GetComponent<PostProcessLayer>().enabled = false;
            }
            
            ApplyTransforms();
        }
    }
}