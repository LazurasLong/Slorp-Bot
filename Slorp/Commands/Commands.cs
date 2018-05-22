using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Slorpbot
{
    public class Commands
    {
        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! {ctx.Client.Ping}ms");
        }

        // Command that rolls dice
        [Command("roll"), Description("Rolls any number of any-sided dice, for example 3d8 will roll an 8 sided die 3 times."), Aliases("r")]
        public async Task Roll(CommandContext ctx, [RemainingText] string _dice)
        {
            await ctx.TriggerTypingAsync();

            Dice dice = new Dice();
            var embed = new DiscordEmbedBuilder(dice.DRoll(_dice, ctx));

            await ctx.RespondAsync(embed: embed.Build());
        }

        [Command("rip"), Description("Pay your respects"), Aliases("F")]
        public async Task Roll(CommandContext ctx, DiscordMember member, [RemainingText] string message)
        {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "F",
                Description = $""
            };
        }
    }
}
