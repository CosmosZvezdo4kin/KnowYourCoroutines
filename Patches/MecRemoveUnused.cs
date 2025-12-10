using System.Collections.Generic;
using HarmonyLib;
using MEC;

namespace KnowYourCoroutines.Patches;

[HarmonyPatch(typeof(Timing), nameof(Timing.RemoveUnused))]
public class MecRemoveUnused
{
    public static bool Prefix(Timing __instance) 
    {
        var handlesToRemove = new List<CoroutineHandle>();

        foreach (var kvp in __instance._waitingTriggers)
        {
            if (kvp.Value.Count == 0)
            {
                handlesToRemove.Add(kvp.Key);
            }
            else if (__instance._handleToIndex.TryGetValue(kvp.Key, out var index) && __instance.CoindexIsNull(index))
            {
                __instance.CloseWaitingProcess(kvp.Key);
                handlesToRemove.Add(kvp.Key);
            }
        }

        foreach (var handle in handlesToRemove)
            __instance._waitingTriggers.Remove(handle);
        
        CompactSegment(__instance, Segment.Update, ref __instance._nextUpdateProcessSlot, __instance.UpdateProcesses, __instance.UpdatePaused, __instance.UpdateHeld, ref __instance.UpdateCoroutines);
        CompactSegment(__instance, Segment.FixedUpdate, ref __instance._nextFixedUpdateProcessSlot, __instance.FixedUpdateProcesses, __instance.FixedUpdatePaused, __instance.FixedUpdateHeld, ref __instance.FixedUpdateCoroutines);
        CompactSegment(__instance, Segment.LateUpdate, ref __instance._nextLateUpdateProcessSlot, __instance.LateUpdateProcesses, __instance.LateUpdatePaused, __instance.LateUpdateHeld, ref __instance.LateUpdateCoroutines);
        CompactSegment(__instance, Segment.SlowUpdate, ref __instance._nextSlowUpdateProcessSlot, __instance.SlowUpdateProcesses, __instance.SlowUpdatePaused, __instance.SlowUpdateHeld, ref __instance.SlowUpdateCoroutines);
        CompactSegment(__instance, Segment.RealtimeUpdate, ref __instance._nextRealtimeUpdateProcessSlot, __instance.RealtimeUpdateProcesses, __instance.RealtimeUpdatePaused, __instance.RealtimeUpdateHeld, ref __instance.RealtimeUpdateCoroutines);
        CompactSegment(__instance, Segment.EndOfFrame, ref __instance._nextEndOfFrameProcessSlot, __instance.EndOfFrameProcesses, __instance.EndOfFramePaused, __instance.EndOfFrameHeld, ref __instance.EndOfFrameCoroutines);
        CompactSegment(__instance, Segment.ManualTimeframe, ref __instance._nextManualTimeframeProcessSlot, __instance.ManualTimeframeProcesses, __instance.ManualTimeframePaused, __instance.ManualTimeframeHeld, ref __instance.ManualTimeframeCoroutines);

        return false;
    }
    
    private static void CompactSegment(Timing instance, Segment segment, ref int nextSlot, IEnumerator<float>[] processes, bool[] paused, bool[] held, ref int coroutineCount) 
    { 
        var readIndex = 0;
        var writeIndex = 0;
        
        var readPI = new Timing.ProcessIndex { seg = segment, i = 0 };
        var writePI = new Timing.ProcessIndex { seg = segment, i = 0 };
        
        while (readIndex < nextSlot)
        {
            if (processes[readIndex] != null)
            {
                if (readIndex != writeIndex)
                {
                    processes[writeIndex] = processes[readIndex];
                    paused[writeIndex] = paused[readIndex];
                    held[writeIndex] = held[readIndex];
                    
                    writePI.i = writeIndex;
                    
                    if (instance._indexToHandle.TryGetValue(writePI, out CoroutineHandle oldHandle))
                    {
                        instance.RemoveGraffiti(oldHandle);
                        instance._handleToIndex.Remove(oldHandle);
                        instance._indexToHandle.Remove(writePI);
                    }
                    
                    readPI.i = readIndex;
                    
                    if (instance._indexToHandle.TryGetValue(readPI, out CoroutineHandle handle))
                    {
                        instance._indexToHandle.Remove(readPI);
                        
                        writePI.i = writeIndex;
                        
                        instance._indexToHandle[writePI] = handle;
                        instance._handleToIndex[handle] = writePI;
                    }
                }
                writeIndex++;
            }
            readIndex++;
        }
        
        while (writeIndex < nextSlot)
        {
            processes[writeIndex] = null;
            paused[writeIndex] = false;
            held[writeIndex] = false;

            writePI.i = writeIndex;
            
            if (instance._indexToHandle.TryGetValue(writePI, out CoroutineHandle handle))
            {
                instance.RemoveGraffiti(handle);
                
                instance._handleToIndex.Remove(handle);
                instance._indexToHandle.Remove(writePI);
            }
            
            writeIndex++;
        }
        
        nextSlot = writeIndex;
        coroutineCount = writeIndex;
    }
}