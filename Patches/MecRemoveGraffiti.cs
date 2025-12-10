using HarmonyLib;
using MEC;

namespace KnowYourCoroutines.Patches;

[HarmonyPatch(typeof(Timing), nameof(Timing.RemoveGraffiti))]
public class MecRemoveGraffiti
{
    public static bool Prefix(Timing __instance , CoroutineHandle handle)
    {
        if (__instance._processLayers.TryGetValue(handle, out var layerName))
        {
            if (__instance._layeredProcesses.TryGetValue(layerName, out var layerSet))
            {
                layerSet.Remove(handle);
                
                if (layerSet.Count == 0)
                {
                    __instance._layeredProcesses.Remove(layerName);
                }
            }

            __instance._processLayers.Remove(handle);
        }
        
        if (__instance._processTags.TryGetValue(handle, out var tagName))
        {
            if (__instance._taggedProcesses.TryGetValue(tagName, out var tagSet))
            {
                tagSet.Remove(handle);
                
                if (tagSet.Count == 0)
                {
                    __instance._taggedProcesses.Remove(tagName);
                }
            }

            __instance._processTags.Remove(handle);
        }

        return false;
    }
}