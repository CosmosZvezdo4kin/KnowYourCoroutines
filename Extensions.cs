using MEC;

namespace KnowYourCoroutines;

public static class Extensions
{
    public static string ToSafeString(this Timing.ProcessIndex processIndex)
    {
        return $"{processIndex.seg}-{processIndex.i}";
    }
    
    public static string ToSafeString(this CoroutineHandle coroutine)
    {
        var result = string.Empty;
        
        var tag = Timing.GetTag(coroutine);
        var layer = Timing.GetLayer(coroutine);
        var debugName = coroutine.GetSafeDebugName();
        
        result += debugName;
        
        if (tag != null)
            result += $" Tag: {tag}";
        
        if (layer != null)
            result += $" Layer: {layer}";

        return result;
    }

    public static string GetSafeDebugName(this CoroutineHandle coroutine)
    {
        if (!coroutine.IsValid)
            return "Uninitialized handle (key == 0)";
        
        var instance = Timing.GetInstance(coroutine.Key);
        
        if (instance == null)
            return "Invalid handle (instance is null)";
        
        if (!instance._handleToIndex.TryGetValue(coroutine, out var processIndex))
            return "Expired coroutine (undefined ProcessIndex)";

        var enumerator = instance.CoindexPeek(processIndex);

        return enumerator == null ? "Coroutine is probably broken (IEnumerator<float> is null)" : enumerator.ToString();
    }
}