using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Slorp.Commands {
    [Group("admin")]
    [Description("Administrative spellbook")]
    [RequirePermissions(Permissions.Administrator)]
    [Hidden]
    public class AdminCommands {
        // Commands in this class need to be executed as <prefix>admin <command> <arguments>

        [Command("Sudo")]
        [Description("Speak or cast a spell as another user.")]
        public async Task Sudo(CommandContext ctx,
            [Description("Member to impersonate.")] DiscordMember member,
            [RemainingText, Description(".")] string command
            ) {
            await ctx.TriggerTypingAsync();

            // Gets the command service, needed to call sudo
            var cmds = ctx.CommandsNext;

            // Performs the sudo
            await cmds.SudoAsync(member, ctx.Channel, command);
        }
    }
}
