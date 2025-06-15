using System;
using CommandSystem;
using LabApi.Features.Permissions;

namespace KnowYourCoroutines.Commands.GetCoroutines;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GetCoroutinesParent : ParentCommand
{
    public GetCoroutinesParent() => LoadGeneratedCommands();
    
    public override string Command => "getcoroutines";

    public override string[] Aliases => Array.Empty<string>();

    public override string Description => "GetCoroutines parent command";
    
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new All());
        RegisterCommand(new Running());
        RegisterCommand(new Paused());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var result = "Please enter a valid subcommand:";

        foreach (var command in AllCommands)
        {
            if (sender.HasAnyPermission($"kyc.{command.Command}"))
                result += $"\n<b>{command.Command}</b> ({command.Description})";
        }
        
        response = result;
        return false;
    }
}