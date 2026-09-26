using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MinimalViewmodelsReborn.Utils;

internal static class PatchHelpers
{
    internal static MethodBase GetRpcLogicMethod(this Type type, string methodName) {
        return type
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(m => m.Name.StartsWith("RpcLogic___" + methodName + "_"));
    }

    internal static IEnumerable<MethodBase> GetRpcLogicMethods(this IEnumerable<Type> types, string methodName) {
        return types
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            .Where(m => m.Name.StartsWith("RpcLogic___" + methodName + "_"));
    }

    internal static CodeMatcher Dump(this CodeMatcher matcher) {
        foreach (var instr in matcher.InstructionEnumeration()) {
            Plugin.Logger.LogWarning(instr);
        }
        return matcher;
    }
}