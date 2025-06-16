using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using MEC;

namespace KnowYourCoroutines.Commands;

public class Kill : ICommand, IUsageProvider
{
    public string Command => "kill";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Kills coroutine by Id";
    
    public string[] Usage => new string[] { "Id" };
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission($"kyc.{Command}"))
        {
            response = $"You don't have permission to execute this command. Required permission: kyc.{Command}";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = $"You must provide coroutine id\nUsage: {Command} {this.DisplayCommandUsage()}";
            return false;
        }

        if (!int.TryParse(arguments.At(0), out var coroutineId))
        {
            response = $"Invalid coroutine id (must be integer)\nUsage: {Command} {this.DisplayCommandUsage()}";
            return false;
        }

        var coroutine = Timing._instance._indexToHandle.FirstOrDefault(x => x.Value._id == coroutineId && x.Value.IsValid).Value;

        if (!coroutine.IsValid)
        {
            response = "Invalid coroutine";
            return false;
        }

        Timing.KillCoroutines(coroutine);

        response = "Successfully killed coroutine!";
        return true;
    }
}