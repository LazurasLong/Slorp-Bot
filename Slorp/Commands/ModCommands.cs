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
        public async Task Clear(CommandContext ctx,
            [Description("Number of messages to delete")]int number,
            [Description("[Optional] Mention a user to only delete their messages")] DiscordMember user = null,
            [Description("[Optional] Give an optional reason for deleting these messages"), RemainingText] string reason = null) {

            List<DiscordMessage> messageList = new List<DiscordMessage>();

            if (user == null) {
                // If no user is specified, retrieves the last <number> messages and stores in a list
                messageList.AddRange(await ctx.Channel.GetMessagesAsync(number, ctx.Message.Id));
            }
            else {
                // Grabs the message that triggered the command
                DiscordMessage message = ctx.Message;

                for (int i = 0; i < number;) {
                    // Retrieves an IEnumerable<DiscordMessage> with the message directly before the previously checked message.
                    // The first time this loop runs, <message> is the message that triggered the clear command.
                    var testMessage = await ctx.Channel.GetMessagesAsync(1, message.Id);
                    message = testMessage[0];

                    // Adds a message to the list if the author matches the user specified in the command
                    if (message.Author == user) {
                        messageList.Add(message);
                        i++;
                    }
                }
            }
            // Adds the message that invoked the command to the list of messages to be deleted, then deletes all messages in the list.
            messageList.Add(ctx.Message);
            await ctx.Channel.DeleteMessagesAsync(messageList as IEnumerable<DiscordMessage>, reason);
        }
    }
}
