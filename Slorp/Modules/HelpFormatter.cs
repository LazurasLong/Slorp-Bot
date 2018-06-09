using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slorp.Modules {
    public class HelpFormatter : IHelpFormatter {
        //private StringBuilder MessageBuilder { get; }
        private DiscordEmbedBuilder HelpEmbed { get; }
        DiscordColor slorpColor = new DiscordColor("#ffd28b");

        public HelpFormatter() {
            //MessageBuilder = new StringBuilder();
            HelpEmbed = new DiscordEmbedBuilder {
                Color = slorpColor
            };
        }

        // This is called first when someone specifies a command using the help function
        // This adds the command's name to the message builder
        public IHelpFormatter WithCommandName(string name) {
            //MessageBuilder.Append("Command: ")
            //    .AppendLine(Formatter.Bold(name))
            //    .AppendLine();

            HelpEmbed.Title = $"Help: {name}";

            return this;
        }

        // This is called second when someone specifies a command using the help function
        // This adds the description of the command to the message builder
        public IHelpFormatter WithDescription(string description) {
            //MessageBuilder.Append("Description: ")
            //    .AppendLine(description)
            //    .AppendLine();

            HelpEmbed.Description = description;

            return this;
        }

        // This is called when a group that can be executed is specified with the help function
        public IHelpFormatter WithGroupExecutable() {
            //MessageBuilder.AppendLine("This group is a standalone command.")
            //    .AppendLine();

            HelpEmbed.Description += "\nThis group is a standalone command.";

            return this;
        }

        // This is called fourth, it adds the command's aliases to the message builder
        public IHelpFormatter WithAliases(IEnumerable<string> aliases) {
            //MessageBuilder.Append("Aliases: ")
            //    .AppendLine(string.Join(", ", aliases))
            //    .AppendLine();

            HelpEmbed.AddField("Aliases", "`" + string.Join(", ", aliases) + "`");

            return this;
        }

        // This is called fifth, it adds the command's arguments to the message builder
        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments) {
            //MessageBuilder.Append("Arguments: ")
            //    .AppendLine(string.Join(", ", arguments.Select(xarg => $"{xarg.Name} ({xarg.Type.ToUserFriendlyName()})")))
            //    .AppendLine();

            HelpEmbed.AddField("Arguments", string.Join("\n", arguments.Select(xarg => $"`{xarg.Name}`: {xarg.Description}")));

            return this;
        }

        // This is called sixth, it adds the group's subcommands if a group is specified with the help function
        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands) {
            //MessageBuilder.Append("Subcommands: ")
            //    .AppendLine(string.Join(", ", subcommands.Select(xc => xc.Name)))
            //    .AppendLine();

            HelpEmbed.AddField("Subcommands", string.Join("\n", subcommands.Select(xc => xc.Name)));

            return this;
        }

        // This is the last method called, it produces and returns the final message.
        public CommandHelpMessage Build() {
            // return new CommandHelpMessage(MessageBuilder.ToString().Replace("\r\n", "\n"));
            return new CommandHelpMessage(embed: HelpEmbed.Build());
        }
    }
}
