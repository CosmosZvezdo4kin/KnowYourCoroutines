using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using RemoteAdmin;

namespace KnowYourCoroutines.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CoroutineParent : ParentCommand, IUsageProvider
{
    public CoroutineParent() => LoadGeneratedCommands();
    
    public override string Command => "coroutine";

    public override string[] Aliases => Array.Empty<string>();

    public override string Description => "Main coroutine parent command";
    
    public string[] Usage => new string[] { "list/kill/pause/resume" };
    
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new List());
        RegisterCommand(new Kill());
        RegisterCommand(new Pause());
        RegisterCommand(new Resume());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var result = "Please enter a valid subcommand:";

        var shouldUseRichText = sender is PlayerCommandSender;

        foreach (var command in AllCommands)
        {
            var args = string.Empty;

            if (command is IUsageProvider usageProvider)
                args = string.Join(" ", usageProvider.Usage.Select(x => $"[{x}]"));
            
            if (sender.HasAnyPermission($"kyc.{command.Command}"))
                result += shouldUseRichText ? $"\n- <b>{command.Command}</b> {args}\n({command.Description})" : $"\n- {command.Command} {args}\n({command.Description})";
        }
        
        response = result;
        return false;
    }
}