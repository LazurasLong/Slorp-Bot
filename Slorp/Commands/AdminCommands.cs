using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Slorp.Commands {
    [Group("admin")]
    [Description("Administrative commands.")]
    [RequirePermissions(Permissions.Administrator)]
    [Hidden]
    public class AdminCommands {
        // Commands in this class need to be executed as <prefix>admin <command> <arguments>

        [Command("sudo")]
        [Description("Executes a command as another user.")]
        public async Task Sudo(CommandContext ctx,
            [Description("Member to execute as.")] DiscordMember member,
            [RemainingText, Description("Command text to execute.")] string command
            ) {
            // the [RemainingText] attribute in the argument captures all text passed to the command
            await ctx.TriggerTypingAsync();

            // Gets the command service, needed for sudo purposes
            var cmds = ctx.CommandsNext;

            // Performs the sudo
            await cmds.SudoAsync(member, ctx.Channel, command);
        }
    }
}
