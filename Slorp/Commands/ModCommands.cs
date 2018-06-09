using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slorp.Commands {
    [Group("mod")]
    [Description("Moderator commands")]
    [RequirePermissions(Permissions.ManageMessages)]
    [Hidden]
    public class ModCommands {
        [Command("clear")]
        [Description("Clears messages from the chat")]
        public async Task Clear(CommandContext ctx, int number, DiscordMember user = null) {
            if (user == null) {
                // If no user is specified, deletes the last <number> messages
                IEnumerable<DiscordMessage> messageList = await ctx.Channel.GetMessagesAsync(number, ctx.Message.Id) as IEnumerable<DiscordMessage>;
                await ctx.Channel.DeleteMessagesAsync(messageList);
            }
            else {
                // Grabs the message that triggered the command
                DiscordMessage message = ctx.Message;
                // Creates an empty list to store messages to be deleted
                List<DiscordMessage> messageList = new List<DiscordMessage>();

                for (int i = 0; i < number;) {
                    // Grabs the message immediately before <message>, stored in an array
                    var testMessage = await ctx.Channel.GetMessagesAsync(1, message.Id);
                    message = testMessage[0];

                    // Adds a message to the list if the author matches the user specified in the command
                    if (message.Author == user) {
                        messageList.Add(message);
                        i++;
                    }
                }

                await ctx.Channel.DeleteMessagesAsync(messageList as IEnumerable<DiscordMessage>);
            }

            // Deletes the message that triggered this command
            await ctx.Message.DeleteAsync("Clearing command clutter");
        }
    }
}
