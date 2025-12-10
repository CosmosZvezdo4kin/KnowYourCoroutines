using System;
using System.Linq;
using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using MEC;

namespace KnowYourCoroutines;

public class Core : Plugin
{
    public override string Name => "KnowYourCoroutines";
    public override string Description => "Useful commands for debugging coroutines";
    public override string Author => "CosmosZvezdochkin";
    public override Version Version => new(1, 1, 0);
    public override Version RequiredApiVersion => new(1, 1, 4);
    
    public static Core Singleton;

    public Config Config;
    
    private Harmony _harmony;
    
    public override void Enable()
    {
        Singleton = this;
        
        if (Config.PatchMethods) 
            PatchMethods();
    }

    public override void Disable()
    {
        UnpatchMethods();
        
        Singleton = null;
    }

    public override void LoadConfigs()
    {
        base.LoadConfigs();

        Config = this.LoadConfig<Config>("config.yml");
    }

    private void PatchMethods()
    {
        _harmony ??= new Harmony($"{Author}.{Name}.{DateTime.Now.Ticks}");

        try
        {
            _harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occured while patching methods:\n{ex}");
        }
        
        Logger.Info($"Successfully patched {_harmony.GetPatchedMethods()?.Count() ?? 0} method(-s)");
    }

    private void UnpatchMethods()
    {
        _harmony?.UnpatchAll();

        _harmony = null;
    }
}