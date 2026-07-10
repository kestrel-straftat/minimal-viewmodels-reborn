using UnityEngine;

namespace MinimalViewmodelsReborn.Utils;

internal static class GameObjectExtensions
{
    // sets a gameobject and all its children to a specified layer (as layers are not inherited)
    internal static void SetLayer(this GameObject obj, LayerMask layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            child.gameObject.layer = layer;
        }
    }
}