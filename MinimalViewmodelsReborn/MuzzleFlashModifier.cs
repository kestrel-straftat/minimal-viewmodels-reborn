using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace MinimalViewmodelsReborn;

public static class MuzzleFlashModifier
{
    private static HashSet<GameObject> m_modifiedMuzzleFlashes = [];

    public static void ClearModifiedSet() => m_modifiedMuzzleFlashes.Clear();
    
    [HarmonyPatch(typeof(Weapon))]
    public static class WeaponPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void ModifyBrightness(Weapon __instance, ref float ___lightIntensity, GameObject ___muzzleFlash) {
            string weaponName = __instance.GetComponent<ItemBehaviour>().weaponName;
            ___lightIntensity *= Configs.MuzzleFlashLightIntensity.Value;
        
            if (!___muzzleFlash || m_modifiedMuzzleFlashes.Contains(___muzzleFlash)) return;
            foreach (var system in ___muzzleFlash.GetComponentsInChildren<ParticleSystem>(true)) {
                var main = system.main;
                main.scalingMode = ParticleSystemScalingMode.Local;
                system.gameObject.transform.localScale = Vector3.one * Configs.MuzzleFlashScale.Value;
            }
            m_modifiedMuzzleFlashes.Add(___muzzleFlash);
        }
    }
}