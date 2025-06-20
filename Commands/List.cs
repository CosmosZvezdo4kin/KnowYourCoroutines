using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using MEC;
using RemoteAdmin;

namespace KnowYourCoroutines.Commands;

public class List : ICommand, IUsageProvider
{
    public string Command => "list";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Get list of coroutines by category";
    
    public string[] Usage => new string[] { "all/valid/running/paused/invalid", "GetFields <true/false>" };
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission($"kyc.{Command}"))
        {
            response = $"You don't have permission to execute this command. Required permission: kyc.{Command}";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = $"You must provide at least one argument\nUsage: {Command} {this.DisplayCommandUsage()}";
            return false;
        }

        var shouldGetFields = false;

        if (arguments.Count > 1 && !bool.TryParse(arguments.At(1), out shouldGetFields))
        {
            response = $"Second argument must be boolean\nUsage: {Command} {this.DisplayCommandUsage()}";
            return false;
        }

        var coroutinesCategory = arguments.At(0).ToLowerInvariant();
        
        var coroutinesList = Timing._instance._indexToHandle.OrderByDescending(x => x.Value._id).ToList();

        switch (coroutinesCategory)
        {
            case "all": break;
            case "valid":
                coroutinesList = coroutinesList.Where(x => x.Value.IsValid).ToList();
                break;
            case "running":
                coroutinesList = coroutinesList.Where(x => x.Value.IsRunning).ToList();
                break;
            case "paused":
                coroutinesList = coroutinesList.Where(x => x.Value.IsAliveAndPaused).ToList();
                break;
            case "invalid":
                coroutinesList = coroutinesList.Where(x => !x.Value.IsValid).ToList();
                break;
            default:
                response = $"An unknown category of coroutines has been entered: {coroutinesCategory}\nUsage:{Command} {this.DisplayCommandUsage()}";
                return false;
        }
        
        var shouldUseRichText = sender is PlayerCommandSender;

        var coroutinesStringList = coroutinesList.Select(x =>
        {
            var processIndex = x.Key;
            var coroutine = x.Value;
            
            var fields = string.Empty;

            if (shouldGetFields)
            {
                var instance = Timing.GetInstance(coroutine.Key);
                
                var enumerator = instance?.CoindexPeek(processIndex);

                if (enumerator != null)
                {
                    var currentType = enumerator.GetType();
                    
                    fields = string.Join(string.Empty, currentType.GetFields().Select(x => $"\n- {x.Name} ({x.FieldType}): {x.GetValue(enumerator)}"));
                }
            }

            return shouldUseRichText ? $"<color=red>{coroutine._id}</color> <color=green>{processIndex.ToSafeString()}</color> {coroutine.ToSafeString()} {fields}" : $"{coroutine._id} {processIndex.ToSafeString()} {coroutine.ToSafeString()} {fields}";
        });

        var result = string.Join("\n", coroutinesStringList);
        
        response = shouldUseRichText ? $"Coroutines count: <b>{coroutinesList.Count}</b>\n{result}" : $"Coroutines count: {coroutinesList.Count}\n{result}";
        return true;
    }
}