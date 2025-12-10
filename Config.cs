using System.ComponentModel;

namespace KnowYourCoroutines;

public class Config
{
    [Description("Patches some MEC methods to fix bugs and improve error tracking. Without enabling this config, the configs below will not work.")]
    public bool PatchMethods { get; set; } = false;
    
    [Description("Enables logging coroutine errors (shows the name of the coroutine and its fields)")]
    public bool LogCoroutineError { get; set; } = true;
    
    [Description("Enables logging coroutine exceptions (the default exception logging system in Unity)")]
    public bool LogCoroutineException { get; set; } = true;
    
    [Description("The name speaks for itself")]
    public bool KillCoroutineOnException { get; set; } = true;
}