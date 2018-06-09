using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Slorp.Modules;
using System.Threading.Tasks;


namespace Slorp.Commands {
    public class CoreCommands {
        DiscordColor slorpColor = new DiscordColor("#ffd28b");

        [Command("ping")]
        [Description("Measures Slorp's latency")]
        [Aliases("pong")]
        public async Task Ping(CommandContext ctx) // CommandContext is context the command came from (User, Channel, Server)
        {
            // triggers a typing indicator in the channel the command came from
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! {ctx.Client.Ping}ms");
        }

        [Command("roll")]
        [Description("Rolls sets of dice")]
        [Aliases("r")]
        public async Task Roll(CommandContext ctx, [RemainingText] string _dice) {
            await ctx.TriggerTypingAsync();

            Dice dice = new Dice();

            await ctx.RespondAsync(embed: dice.DRoll(_dice));
        }

        [Command("rip")]
        [Description("Pay your respects")]
        [Aliases("f")]
        public async Task Rip(
            CommandContext ctx,
            [Description("Member who died, can be a nickname or a @mention")] DiscordMember member,
            [RemainingText, Description("Leave an epitaph?")] string message = ""
            ) {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder {
                Title = $":skull: {member.DisplayName} died :skull:",
                Description = $"Here lies {member.Mention}",
                Color = slorpColor
            };

            if (message != "") embed.AddField("In Memoriam", message);

            await ctx.RespondAsync(embed: embed.Build());

            await ctx.Message.DeleteAsync(); // Deletes the message that triggered this command.
        }
    }
}
