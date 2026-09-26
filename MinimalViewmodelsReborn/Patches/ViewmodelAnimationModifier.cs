using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Configuration;
using HarmonyLib;
using MinimalViewmodelsReborn.Utils;
using UnityEngine;

namespace MinimalViewmodelsReborn.Patches;

public static class ViewmodelAnimationModifier
{
    private static FPArms m_currentArms;
    private static Defaults m_defaults;
    
    public static void Apply() {
        m_currentArms.maxRotationAmount = m_defaults.maxRotationAmount * Plugin.ViewmodelSwayScale.Value;
        m_currentArms.jumpOffset = m_defaults.jumpOffset * Plugin.ViewmodelSwayScale.Value;
        m_currentArms.fallOffset = m_defaults.fallOffset * Plugin.ViewmodelSwayScale.Value;
        
        m_currentArms.bobScale = m_defaults.bobScale * Plugin.ViewmodelBobScale.Value;
        m_currentArms.crouchBobScale = m_defaults.crouchBobScale * Plugin.ViewmodelBobScale.Value;
        m_currentArms.sprintBobScale = m_defaults.sprintBobScale * Plugin.ViewmodelBobScale.Value;
        m_currentArms.aimBobScale = m_defaults.aimBobScale * Plugin.ViewmodelBobScale.Value;
        m_currentArms.idleScale = m_defaults.idleScale * Plugin.ViewmodelBobScale.Value;
        
    }

    [HarmonyPatch(typeof(FPArms))]
    public static class FPArmsPatch 
    {
        [HarmonyPatch(nameof(FPArms.Start)), HarmonyPostfix]
        public static void CollectOriginalValues(FPArms __instance) {
            m_currentArms = __instance;
            m_defaults = new Defaults(__instance);
            Apply();
        }

        // ffs sirius
        [HarmonyPatch(nameof(FPArms.Update)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ScaleHardcodedValues(IEnumerable<CodeInstruction> instructions) {
            return new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Add)
                )
                .Repeat(matcher => matcher
                    .Insert(
                        new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Plugin), nameof(Plugin.ViewmodelBobScale))),
                        new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ConfigEntry<float>), nameof(ConfigEntry<>.Value))),
                        new CodeInstruction(OpCodes.Mul)
                    )
                )
                .InstructionEnumeration();
        }
    }
    
    private struct Defaults(FPArms arms)
    {
        public Vector3 jumpOffset = arms.jumpOffset;
        public Vector3 fallOffset = arms.fallOffset;

        public float bobScale = arms.bobScale;
        public float crouchBobScale = arms.crouchBobScale;
        public float sprintBobScale = arms.sprintBobScale;
        public float aimBobScale = arms.aimBobScale;
        public float idleScale = arms.idleScale;

        public float maxRotationAmount = arms.maxRotationAmount;
    }
}