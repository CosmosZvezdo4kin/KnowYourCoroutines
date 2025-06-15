using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using MEC;

namespace KnowYourCoroutines.Commands.GetCoroutines;

public class All : ICommand
{
    public string Command => "all";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Get list of all coroutines";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission($"kyc.{Command}"))
        {
            response = $"You don't have permission to execute this command. Required permission: kyc.{Command}";
            return false;
        }

        var coroutinesList = Timing._instance._indexToHandle.Where(x => x.Value.IsValid).OrderByDescending(x => x.Value._id).ToList();

        var result = string.Join("\n", coroutinesList.Select(x => $"<color=red>{x.Value._id}</color> {x.Value}"));
        
        response = $"Coroutines count: <b>{coroutinesList.Count}</b>\n{result}";
        return true;
    }
}