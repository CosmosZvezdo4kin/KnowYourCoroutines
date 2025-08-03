using System;
using LabApi.Loader.Features.Plugins;

namespace KnowYourCoroutines;

public class Core : Plugin
{
    public override string Name => "KnowYourCoroutines";

    public override string Description => "Useful commands for debugging coroutines";

    public override string Author => "CosmosZvezdochkin";

    public override Version Version => new(1, 0, 4);

    public override Version RequiredApiVersion => new(1, 1, 1);
    
    public override void Enable()
    {
    }

    public override void Disable()
    {
    }
}