using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MinimalViewmodelsReborn.Utils;
using UnityEngine;

namespace MinimalViewmodelsReborn.Patches;

// the following patches are largely to set all weapon vfx's layers to the HeldWeapon layer
// this makes the weapon camera render them instead of the main camera, transforming them with the gun
// i know it's not pretty code. sorry about that. but the way guns are set up make them *very* hard to work with
// if you want to do something to all of them.
public static class VfxPatches
{
    private static LayerMask m_heldLayer = LayerMask.NameToLayer("HeldWeapon");
    private static LayerMask m_defaultLayer = LayerMask.NameToLayer("Default");
    
    // fix the bullet trail to visually finish at the right point as we transform it with the weapon cam
    // & fix layer of prefab of bullet trail for the duration of the call
    [HarmonyPatch]
    public static class SpawnBulletTrailPatch
    {
        // note to future self. remember the underscore when patching rpclogic shit like this silly
        [HarmonyTargetMethods]
        private static IEnumerable<MethodBase> TargetMethods() =>
            TypeHelpers.WeaponTypes
                .GetRpcLogicMethods("SpawnBulletTrail");
        
        [HarmonyPrefix]
        private static bool FixHitPoint(Weapon __instance, ref Vector3 hitPoint, Camera ___cam, LineRenderer ___bulletTrailLocal) {
            if (!__instance.IsOwner) return true;
            if (Plugin.HideBulletTrails.Value) return false;
            // recalculate hit point (we know the original ray originated from ___cam.transform.position and ended at hitpoint)
            hitPoint = ViewmodelModifier.WeaponCam.transform.position + (hitPoint - ___cam.transform.position);
            if (___bulletTrailLocal) ___bulletTrailLocal.gameObject.SetLayer(m_heldLayer);

            return true;
        }

        [HarmonyPostfix]
        private static void ResetVfxLayers(Weapon __instance, LineRenderer ___bulletTrailLocal) {
            if (!___bulletTrailLocal || !__instance.IsOwner) return;
            ___bulletTrailLocal.gameObject.SetLayer(m_defaultLayer);
        }
    }
    
    // fix layers of prefabs of eject case vfx
    [HarmonyPatch(typeof(Weapon), nameof(Weapon.OnShoot))]
    public static class WeaponPatch
    {
        [HarmonyPrefix]
        public static void SetVfxLayers(Weapon __instance, GameObject ___ejectCaseVfx) {
            if (!___ejectCaseVfx || !__instance.IsOwner) return;
            ___ejectCaseVfx.SetLayer(m_heldLayer);
        }

        [HarmonyPostfix]
        public static void ResetVfxLayers(Weapon __instance, GameObject ___ejectCaseVfx) {
            if (!___ejectCaseVfx || !__instance.IsOwner) return;
            ___ejectCaseVfx.SetLayer(m_defaultLayer);
        }
    }
    
    // fix layers of prefabs of muzzle flashes (for the duration of the call)
    [HarmonyPatch]
    public static class MuzzleFlashFix
    {
        [HarmonyTargetMethods]
        private static IEnumerable<MethodBase> TargetMethods() =>
            TypeHelpers.WeaponTypes
                .Except([typeof(BeamGun)]) // beam gun does its own thing (?????). see below
                .GetRpcLogicMethods("ShootObserversEffect");

        [HarmonyPrefix]
        private static void SetVfxLayers(Weapon __instance, GameObject ___muzzleFlash) {
            if (!___muzzleFlash || !__instance.IsOwner) return;
            ___muzzleFlash.SetLayer(m_heldLayer);
        }

        [HarmonyPostfix]
        private static void ResetVfxLayers(Weapon __instance, GameObject ___muzzleFlash) {
            if (!___muzzleFlash || !__instance.IsOwner) return;
            ___muzzleFlash.SetLayer(m_defaultLayer);
        }
    }

    // various special cases of the above
    // because being consistent would kill sirius instantly
    
    [HarmonyPatch(typeof(DualLauncher), nameof(DualLauncher.Update))]
    public static class DualLauncherPatch
    {
        [HarmonyPrefix]
        public static void SetVfxLayers(DualLauncher __instance, bool ___grenadeOpen, ParticleSystem ___grenadeSmoke) {
            if (!___grenadeOpen || !__instance.IsOwner) return;
            ___grenadeSmoke.gameObject.SetLayer(m_heldLayer);
        }
        
        [HarmonyPostfix]
        public static void ResetVfxLayers(DualLauncher __instance, bool ___grenadeOpen, ParticleSystem ___grenadeSmoke) {
            if (!___grenadeOpen || !__instance.IsOwner) return;
            ___grenadeSmoke.gameObject.SetLayer(m_defaultLayer);
        }
    }

    // what the hell is muzzle flash 2
    
    [HarmonyPatch(typeof(BeamGun))]
    public static class BeamGunMuzzleFlash2Patch
    {
        [HarmonyTargetMethod]
        private static MethodBase TargetMethod() => 
            typeof(BeamGun).GetRpcLogicMethod(nameof(BeamGun.ShootObserversEffect));
        
        [HarmonyPrefix]
        public static void SetVfxLayers(BeamGun __instance, GameObject ___muzzleFlash2) {
            if (!__instance.IsOwner || !___muzzleFlash2) return;
            ___muzzleFlash2.SetLayer(m_heldLayer);
        }
        
        [HarmonyPostfix]
        public static void ResetVfxLayers(BeamGun __instance, GameObject ___muzzleFlash2) {
            if (!__instance.IsOwner || !___muzzleFlash2) return;
            ___muzzleFlash2.SetLayer(m_defaultLayer);
        }
    }
    
    [HarmonyPatch(typeof(BeamGun))]
    public static class BeamGunMuzzleFlashPatch
    {
        [HarmonyTargetMethod]
        private static MethodBase TargetMethod() =>
            typeof(BeamGun).GetRpcLogicMethod(nameof(BeamGun.ShootObserversEffect2));
        
        [HarmonyPrefix]
        public static void SetVfxLayers(BeamGun __instance, GameObject ___muzzleFlash) {
            if (!__instance.IsOwner || !___muzzleFlash) return;
            ___muzzleFlash.SetLayer(m_heldLayer);
        }
        
        [HarmonyPostfix]
        public static void ResetVfxLayers(BeamGun __instance, GameObject ___muzzleFlash) {
            if (!__instance.IsOwner || !___muzzleFlash) return;
            ___muzzleFlash.SetLayer(m_defaultLayer);
        }
    }
    
    // fix the placement hologram to render on the main camera
    [HarmonyPatch(typeof(WeaponHandSpawner))]
    public static class WeaponHandSpawnerPatch
    {
        [HarmonyPatch(nameof(WeaponHandSpawner.HandlePlacement)), HarmonyPostfix]
        private static void FixPlacementHologram(Transform ___previewObject) {
            ___previewObject.GetChild(0).gameObject.layer = m_defaultLayer;
        }
    }
}