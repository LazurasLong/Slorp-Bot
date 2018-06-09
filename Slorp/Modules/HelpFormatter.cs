using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Slorp.Modules {
    public class HelpFormatter : IHelpFormatter {
        private DiscordEmbedBuilder HelpEmbed { get; }
        DiscordColor slorpColor = new DiscordColor("#ffd28b");

        public HelpFormatter() {
            HelpEmbed = new DiscordEmbedBuilder {
                Color = slorpColor
            };
        }

        // This is called first when someone specifies a command using the help function
        // This adds the command's name to the message builder
        public IHelpFormatter WithCommandName(string name) {
            HelpEmbed.Title = $"Help: {name}";

            return this;
        }

        // This is called second when someone specifies a command using the help function
        // This adds the description of the command to the message builder
        public IHelpFormatter WithDescription(string description) {
            HelpEmbed.Description = description;

            return this;
        }

        // This is called when a group that can be executed is specified with the help function
        public IHelpFormatter WithGroupExecutable() {
            HelpEmbed.Description += "\nThis is a castable group of spells.";

            return this;
        }

        // This is called fourth, it adds the command's aliases to the message builder
        public IHelpFormatter WithAliases(IEnumerable<string> aliases) {
            HelpEmbed.AddField("Aliases", "`" + string.Join(", ", aliases) + "`");

            return this;
        }

        // This is called fifth, it adds the command's arguments to the message builder
        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments) {
            HelpEmbed.AddField("Components", string.Join("\n", arguments.Select(xarg => $"`{xarg.Name}`: {xarg.Description}")));

            return this;
        }

        // This is called sixth, it adds the group's subcommands if a group is specified with the help function
        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands) {
            HelpEmbed.AddField("Spells", "```" + string.Join("\n", subcommands.Select(xc => xc.Name)) + "```");

            return this;
        }

        // This is the last method called, it produces and returns the final message.
        public CommandHelpMessage Build() {
            return new CommandHelpMessage(embed: HelpEmbed.Build());
        }
    }
}
