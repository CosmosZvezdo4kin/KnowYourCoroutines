using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MEC;
using UnityEngine;
using static KnowYourCoroutines.Core;

namespace KnowYourCoroutines.Patches;

[HarmonyPatch(typeof(Timing))]
public class MecUpdate
{
    [HarmonyPatch(nameof(Timing.Update))]
    [HarmonyPrefix]
    public static bool Update(Timing __instance)
    {
        var eventField = typeof(Timing).GetField(nameof(Timing.OnPreExecute), BindingFlags.Static | BindingFlags.NonPublic);
        
        var handler = eventField?.GetValue(null) as Action;
        
        handler?.Invoke();
        
        if (__instance._nextSlowUpdateProcessSlot > 0 &&
            __instance._lastSlowUpdateTime + __instance.TimeBetweenSlowUpdateCalls < Time.realtimeSinceStartup)
        {
            if (__instance.UpdateTimeValues(Segment.SlowUpdate))
            {
                __instance._lastSlowUpdateProcessSlot = __instance._nextSlowUpdateProcessSlot;
            }
            RunSegment(__instance, Segment.SlowUpdate, __instance._lastSlowUpdateProcessSlot, __instance.SlowUpdateProcesses, __instance.SlowUpdatePaused, __instance.SlowUpdateHeld);
        }
        
        if (__instance._nextRealtimeUpdateProcessSlot > 0)
        {
            if (__instance.UpdateTimeValues(Segment.RealtimeUpdate))
            {
                __instance._lastRealtimeUpdateProcessSlot = __instance._nextRealtimeUpdateProcessSlot;
            }
            
            RunSegment(__instance, Segment.RealtimeUpdate, __instance._lastRealtimeUpdateProcessSlot, __instance.RealtimeUpdateProcesses, __instance.RealtimeUpdatePaused, __instance.RealtimeUpdateHeld);
        }
        
        if (__instance._nextUpdateProcessSlot > 0)
        {
            if (__instance.UpdateTimeValues(Segment.Update))
            {
                __instance._lastUpdateProcessSlot = __instance._nextUpdateProcessSlot;
            }
            
            RunSegment(__instance, Segment.Update, __instance._lastUpdateProcessSlot, __instance.UpdateProcesses, __instance.UpdatePaused, __instance.UpdateHeld);
        }
        
        if (__instance.AutoTriggerManualTimeframe)
        {
            __instance.TriggerManualTimeframeUpdate();
        }
        else
        {
            __instance._framesSinceUpdate++;
            
            if (__instance._framesSinceUpdate > 64)
            {
                __instance._framesSinceUpdate = 0;
                __instance.RemoveUnused();
            }
        }

        __instance.currentCoroutine = default(CoroutineHandle);

        return false;
    }
    
    [HarmonyPatch(nameof(Timing.FixedUpdate))]
    [HarmonyPrefix]
    public static bool FixedUpdate(Timing __instance)
    {
        var eventField = typeof(Timing).GetField(nameof(Timing.OnPreExecute), BindingFlags.Static | BindingFlags.NonPublic);
        
        var handler = eventField?.GetValue(null) as Action;
        
        handler?.Invoke();

        if (__instance._nextFixedUpdateProcessSlot > 0)
        {
            if (__instance.UpdateTimeValues(Segment.FixedUpdate))
            {
                __instance._lastFixedUpdateProcessSlot = __instance._nextFixedUpdateProcessSlot;
            }
            
            RunSegment(__instance, Segment.FixedUpdate, __instance._lastFixedUpdateProcessSlot, __instance.FixedUpdateProcesses, __instance.FixedUpdatePaused, __instance.FixedUpdateHeld);
        }

        __instance.currentCoroutine = default(CoroutineHandle);

        return false;
    }
    
    [HarmonyPatch(nameof(Timing.LateUpdate))]
    [HarmonyPrefix]
    public static bool LateUpdate(Timing __instance)
    {
        var eventField = typeof(Timing).GetField(nameof(Timing.OnPreExecute), BindingFlags.Static | BindingFlags.NonPublic);
        
        var handler = eventField?.GetValue(null) as Action;
        
        handler?.Invoke();

        if (__instance._nextLateUpdateProcessSlot > 0)
        {
            if (__instance.UpdateTimeValues(Segment.LateUpdate))
            {
                __instance._lastLateUpdateProcessSlot = __instance._nextLateUpdateProcessSlot;
            }
            
            RunSegment(__instance, Segment.LateUpdate, __instance._lastLateUpdateProcessSlot, __instance.LateUpdateProcesses, __instance.LateUpdatePaused, __instance.LateUpdateHeld);
        }

        __instance.currentCoroutine = default(CoroutineHandle);

        return false;
    }
    
    [HarmonyPatch(nameof(Timing.TriggerManualTimeframeUpdate))]
    [HarmonyPrefix]
    public static bool TriggerManualTimeframeUpdate(Timing __instance)
    {
        var eventField = typeof(Timing).GetField(nameof(Timing.OnPreExecute), BindingFlags.Static | BindingFlags.NonPublic);
        
        var handler = eventField?.GetValue(null) as Action;
        
        handler?.Invoke();

        if (__instance._nextManualTimeframeProcessSlot > 0)
        {
            if (__instance.UpdateTimeValues(Segment.ManualTimeframe))
            {
                __instance._lastManualTimeframeProcessSlot = __instance._nextManualTimeframeProcessSlot;
            }
            
            RunSegment(__instance, Segment.ManualTimeframe, __instance._lastManualTimeframeProcessSlot, __instance.ManualTimeframeProcesses, __instance.ManualTimeframePaused, __instance.ManualTimeframeHeld);
        }
        
        __instance._framesSinceUpdate++;
        
        if (__instance._framesSinceUpdate > 64)
        {
            __instance._framesSinceUpdate = 0;
            __instance.RemoveUnused();
        }

        __instance.currentCoroutine = default(CoroutineHandle);

        return false;
    }
    
    private static void RunSegment(Timing instance, Segment segment, int count, IEnumerator<float>[] processes, bool[] paused, bool[] held)
    {
        var processIndex = new Timing.ProcessIndex { seg = segment, i = 0 };

        for (int i = 0; i < count; i++)
        {
            processIndex.i = i;

            if (paused[i] || held[i] || processes[i] == null)
                continue;

            if (instance.localTime < processes[i].Current)
                continue;
            
            if (!instance._indexToHandle.TryGetValue(processIndex, out CoroutineHandle handle))
                continue;

            instance.currentCoroutine = handle;

            try
            {
                if (!processes[i].MoveNext())
                {
                    instance.KillCoroutinesOnInstance(handle);
                }
                else if (float.IsNaN(processes[i].Current))
                {
                    if (Timing.ReplacementFunction != null)
                    {
                        processes[i] = Timing.ReplacementFunction(processes[i], handle);
                        Timing.ReplacementFunction = null;
                    }
                    
                    i--;
                }
            }
            catch (Exception ex)
            {
                if (Singleton.Config.LogCoroutineError)
                {
                    var enumerator = processes[i];
                    
                    var currentType = enumerator.GetType();
                    
                    var fields = string.Join(string.Empty, currentType.GetFields().Select(field =>
                    {
                        var fieldValue = field.GetValue(enumerator);
                        
                        switch (fieldValue)
                        {
                            case Delegate del:
                                return $"\n- {field.Name} ({field.FieldType}): Name: {del.Method.DeclaringType?.FullName}.{del.Method.Name}, Target: {del.Target}";
                            case GameObject gameObject:
                                return $"\n- {field.Name} ({field.FieldType}): Name: {gameObject.name}, Tag: {gameObject.tag}";
                            default:
                                return $"\n- {field.Name} ({field.FieldType}): {fieldValue?.ToString() ?? "null"}";
                        }
                    }));
                    
                    LabApi.Features.Console.Logger.Error($"[MEC COROUTINE] Segment.{segment} {ex.Message} EXCEPTION CAUGHT ({enumerator}):{fields}");
                }
                
                if (Singleton.Config.LogCoroutineException)
                    Debug.LogException(ex);
                
                if (Singleton.Config.KillCoroutineOnException)
                    instance.KillCoroutinesOnInstance(handle);
            }
        }
    }
}