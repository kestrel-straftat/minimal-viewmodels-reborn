using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace MinimalViewmodelsReborn;

// the following patches are largely to set all weapon vfx's layers to the HeldWeapon layer
// this makes the weapon camera render them instead of the main camera, transforming them with the gun
// i know it's not pretty code. sorry about that. but the way guns are set up make them *very* hard to work with
// if you want to do something to all of them.
public static class VfxPatches
{
    private static IEnumerable<Type> m_weaponTypes = AccessTools.AllTypes().Where(t => t != typeof(Weapon) && typeof(Weapon).IsAssignableFrom(t));
    private static LayerMask m_heldLayer = LayerMask.NameToLayer("HeldWeapon");
    private static LayerMask m_defaultLayer = LayerMask.NameToLayer("Default");
    
    // sets a gameobject and all its children to a specified layer (as layers are not inherited)
    private static void SetObjToLayer(GameObject obj, LayerMask layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            child.gameObject.layer = layer;
        }
    }
    
    // fix the bullet trail to visually finish at the right point as we transform it with the weapon cam
    // & fix layer of prefab of bullet trail for the duration of the call
    [HarmonyPatch]
    public static class SpawnBulletTrailPatch
    {
        [HarmonyTargetMethods]
        private static IEnumerable<MethodBase> TargetMethods() {
            foreach (var type in m_weaponTypes) {
                if (type.GetMethod("SpawnBulletTrail", BindingFlags.Instance | BindingFlags.NonPublic) is { } method)
                    yield return method;
            }
        }

        [HarmonyPrefix]
        private static void FixHitPoint(Weapon __instance, ref Vector3 hitPoint, Camera ___cam, LineRenderer ___bulletTrailLocal) {
            if (!__instance.IsOwner) return;
            // recalculate hit point (we know the original ray originated from ___cam.transform.position and ended at hitpoint)
            hitPoint = ViewmodelModifier.WeaponCam.transform.position + (hitPoint - ___cam.transform.position);
            
            if (___bulletTrailLocal) SetObjToLayer(___bulletTrailLocal.gameObject, m_heldLayer);
        }

        [HarmonyPostfix]
        private static void ResetVfxLayers(Weapon __instance, LineRenderer ___bulletTrailLocal) {
            if (!__instance.IsOwner || !___bulletTrailLocal) return;
            SetObjToLayer(___bulletTrailLocal.gameObject, m_defaultLayer);
        }
    }
    
    
    // fix layers of prefabs of eject case vfx
    [HarmonyPatch(typeof(Weapon), "OnShoot")]
    public static class WeaponPatch
    {
        [HarmonyPrefix]
        public static void SetVfxLayers(Weapon __instance, GameObject ___ejectCaseVfx) {
            if (!__instance.IsOwner || !___ejectCaseVfx) return;
            SetObjToLayer(___ejectCaseVfx, m_heldLayer);
        }

        [HarmonyPostfix]
        public static void ResetVfxLayers(Weapon __instance, GameObject ___ejectCaseVfx) {
            if (!__instance.IsOwner || !___ejectCaseVfx) return;
            SetObjToLayer(___ejectCaseVfx, m_defaultLayer);
        }
    }
    
    // fix layers of prefabs of muzzle flashes (for the duration of the call)
    [HarmonyPatch]
    public static class MuzzleFlashFix
    {
        [HarmonyTargetMethods]
        private static IEnumerable<MethodBase> TargetMethods() {
            foreach (var type in m_weaponTypes) {
                if (type.GetMethod("ShootObserversEffect", BindingFlags.Instance | BindingFlags.NonPublic) is { } method)
                    yield return method;
            }
        }

        [HarmonyPrefix]
        private static void SetVfxLayers(Weapon __instance, GameObject ___muzzleFlash) {
            if (!__instance.IsOwner || !___muzzleFlash) return;
            SetObjToLayer(___muzzleFlash, m_heldLayer);
        }

        [HarmonyPostfix]
        private static void ResetVfxLayers(Weapon __instance, GameObject ___muzzleFlash) {
            if (!__instance.IsOwner || !___muzzleFlash) return;
            SetObjToLayer(___muzzleFlash, m_defaultLayer);
        }
    }

    // various special cases of the above
    // because being consistent would kill sirius instantly
    
    [HarmonyPatch(typeof(DualLauncher), "Update")]
    public static class DualLauncherPatch
    {
        [HarmonyPrefix]
        public static void SetVfxLayers(DualLauncher __instance, bool ___grenadeOpen, ParticleSystem ___grenadeSmoke) {
            if (!___grenadeOpen || !__instance.IsOwner) return;
            SetObjToLayer(___grenadeSmoke.gameObject, m_heldLayer);
        }
        
        [HarmonyPostfix]
        public static void ResetVfxLayers(DualLauncher __instance, bool ___grenadeOpen, ParticleSystem ___grenadeSmoke) {
            if (!___grenadeOpen || !__instance.IsOwner) return;
            SetObjToLayer(___grenadeSmoke.gameObject, m_defaultLayer);
        }
    }

    [HarmonyPatch(typeof(BeamGun))]
    public static class WhatTheHellIsMuzzleFlash2
    {
        [HarmonyPatch("ShootObserversEffect")]
        [HarmonyPrefix]
        public static void SetVfxLayers(BeamGun __instance, GameObject ___muzzleFlash2) {
            if (!__instance.IsOwner || !___muzzleFlash2) return;
            SetObjToLayer(___muzzleFlash2, m_heldLayer);
        }
        
        [HarmonyPatch("ShootObserversEffect")]
        [HarmonyPostfix]
        public static void ResetVfxLayers(BeamGun __instance, GameObject ___muzzleFlash2) {
            if (!__instance.IsOwner || !___muzzleFlash2) return;
            SetObjToLayer(___muzzleFlash2, m_defaultLayer);
        }
        
        // ... why are the vfx for the beam load set up like this
        
        [HarmonyPatch("ShootObserversEffect2")]
        [HarmonyPrefix]
        public static void SetVfxLayers2(BeamGun __instance, GameObject ___muzzleFlash) {
            if (!__instance.IsOwner || !___muzzleFlash) return;
            SetObjToLayer(___muzzleFlash, m_heldLayer);
        }
        
        [HarmonyPatch("ShootObserversEffect2")]
        [HarmonyPostfix]
        public static void ResetVfxLayers2(BeamGun __instance, GameObject ___muzzleFlash) {
            if (!__instance.IsOwner || !___muzzleFlash) return;
            SetObjToLayer(___muzzleFlash, m_defaultLayer);
        }
    }
    
    // fix the placement hologram to render on the main camera
    [HarmonyPatch(typeof(WeaponHandSpawner))]
    public static class WeaponHandSpawnerPatch
    {
        [HarmonyPatch("HandlePlacement")]
        [HarmonyPostfix]
        private static void FixPlacementHologram(Transform ___previewObject) {
            ___previewObject.GetChild(0).gameObject.layer = m_defaultLayer;
        }
    }
}