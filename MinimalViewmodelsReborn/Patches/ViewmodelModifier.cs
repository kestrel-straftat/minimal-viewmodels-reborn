using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MinimalViewmodelsReborn.Patches;

public static class ViewmodelModifier
{
    public static Camera WeaponCam { get; private set; }
    
    private static SkinnedMeshRenderer[] m_armRenderers;
    
    public static void Apply() {
        if (!WeaponCam) {
            return;
        }

        bool hideArms = Plugin.InvisibleArms.Value;
        foreach (var obj in m_armRenderers) {
            obj.enabled = !hideArms;
        }
        
        WeaponCam.enabled = !Plugin.InvisibleViewmodels.Value;
        if (Plugin.InvisibleViewmodels.Value) {
            return;
        }
        
        WeaponCam.transform.localPosition = -Plugin.ViewmodelOffset;
        
        WeaponCam.projectionMatrix = ConstructCameraProjectionMatrix();
        
        Camera.onPreRender -= OnPreRender;
        Camera.onPostRender -= OnPostRender;
        if (Plugin.MirrorViewmodel.Value) {
            Camera.onPreRender += OnPreRender;
            Camera.onPostRender += OnPostRender;
        }
    }
    
    private static void OnPreRender(Camera cam) {
        if (cam == WeaponCam) {
            GL.invertCulling = true;
        }
    }
    private static void OnPostRender(Camera cam) {
        if (cam == WeaponCam) {
            GL.invertCulling = false;
        }
    }
    
    // technically this should also be called whenever the screen aspect changes
    // but unity doesn't let you do that easily. sooooo
    private static Matrix4x4 ConstructCameraProjectionMatrix() {
        float aspect = (float)Screen.width / Screen.height;
        
        // far plane is 100.0 to prevent bullet trails from being cut off when being rendered by the weapon cam
        var mat = Matrix4x4.Perspective(Plugin.ViewmodelFOV.Value, aspect, 0.1f, 100.0f);
        if (Plugin.MirrorViewmodel.Value) {
            mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        }
        return mat;
    }
    
    // ok. that was the easy bit. now it's time to fix everything that broke!
    
    // needed to make the viewmodel customisation work on medium/low graphics
    // as weapons are rendered by the main cam on those settings
    [HarmonyPatch(typeof(PlayerSetup))]
    public static class PlayerSetupPatch
    {
        [HarmonyPatch(nameof(PlayerSetup.OnStartClient)), HarmonyPostfix]
        private static void FixCameras(PlayerSetup __instance, Camera[] ___cameras, LayerMask ___highMask, GameObject[] ___fpArms) {
            if (!__instance.IsOwner) return;
            ___cameras[0].cullingMask = ___highMask;
            WeaponCam = ___cameras[1];
            //m_originalProjectionMatrix = WeaponCam.projectionMatrix;
            m_armRenderers = ___fpArms.Select(obj => obj.GetComponent<SkinnedMeshRenderer>()).ToArray();
            
            // force the weapon camera to be enabled
            WeaponCam.enabled = true;
            
            // if on medium/low graphics
            if (Settings.Instance.qualitySetting < 2) {
                // disable unwanted prost processing (adds fxaa)
                WeaponCam.GetComponent<PostProcessLayer>().enabled = false;
            }
            
            Apply();
        }
    }
}