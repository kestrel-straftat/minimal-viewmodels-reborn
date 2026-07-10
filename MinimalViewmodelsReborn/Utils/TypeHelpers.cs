using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace MinimalViewmodelsReborn.Utils;

internal static class TypeHelpers
{
    internal static IEnumerable<Type> WeaponTypes { get; private set; } = 
        AccessTools.AllTypes().Where(t => t != typeof(Weapon) && typeof(Weapon).IsAssignableFrom(t));
}