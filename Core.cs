using System;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using MEC;

namespace KnowYourCoroutines;

public class Core : Plugin
{
    public override string Name => "KnowYourCoroutines";

    public override string Description => "Useful commands for debugging coroutines";

    public override string Author => "CosmosZvezdochkin";

    public override Version Version => new(1, 0, 5);

    public override Version RequiredApiVersion => new(1, 1, 4);
    
    public override void Enable()
    {
    }

    public override void Disable()
    {
    }
}