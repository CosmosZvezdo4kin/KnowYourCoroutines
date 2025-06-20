﻿using System;
using LabApi.Loader.Features.Plugins;

namespace KnowYourCoroutines;

public class Core : Plugin
{
    public override string Name => "KnowYourCoroutines";

    public override string Description => "Useful commands for debugging coroutines";

    public override string Author => "CosmosZvezdochkin";

    public override Version Version => new(1, 0, 3);

    public override Version RequiredApiVersion => new(1, 0, 2);
    
    public override void Enable()
    {
    }

    public override void Disable()
    {
    }
}