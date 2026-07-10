using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MinimalViewmodelsReborn.Patches;

public static class MuzzleFlashModifier
{
    private static HashSet<GameObject> m_modifiedMuzzleFlashes = [];

    public static void Apply() {
        // this doesn't exactly fit the "Modifier" thing i'm going for here
        // as it does things a little more dynamically but whatever
        m_modifiedMuzzleFlashes.Clear();
    }

    [HarmonyPatch(typeof(Weapon))]
    public static class WeaponPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void ModifyBrightness(Weapon __instance, ref float ___lightIntensity, GameObject ___muzzleFlash) {
            // TODO fix muzzle flash lights
            //___lightIntensity *= Plugin.MuzzleFlashLightIntensity.Value;
        
            if (!___muzzleFlash || m_modifiedMuzzleFlashes.Contains(___muzzleFlash)) return;
            foreach (var system in ___muzzleFlash.GetComponentsInChildren<ParticleSystem>(true)) {
                var main = system.main;
                main.scalingMode = ParticleSystemScalingMode.Local;
                system.gameObject.transform.localScale = Vector3.one * Plugin.MuzzleFlashScale.Value;
            }
            m_modifiedMuzzleFlashes.Add(___muzzleFlash);
        }
    }
}