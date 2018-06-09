using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using Slorp.Commands;
using Slorp.Modules;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Slorp.Core {
    public class Program {
        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }
        ConfigJson cfgjson;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync() {
            // Load Configuration File
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            // Load the values from config file
            cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            var cfg = new DiscordConfiguration {
                Token = cfgjson.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            // Instantiate the client
            Client = new DiscordClient(cfg);

            // hook events
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientError;

            LoadCommands();

            // Connect and log in
            await Client.ConnectAsync();

            // Infinite wait
            await Task.Delay(-1);
        }

        private void LoadCommands() {
            // Set up command options
            var cmdcfg = new CommandsNextConfiguration {
                // Sets the prefix from the loaded file
                StringPrefix = cfgjson.CommandPrefix,

                // Enable DM responses
                EnableDms = true,

                // Lets users mention the bot instead of using a prefix
                EnableMentionPrefix = true,
                
                // Stops commands from being case sensitive
                CaseSensitive = false
            };

            // Load commands from command class
            Commands = Client.UseCommandsNext(cmdcfg);

            // Hook command events, so bot can read commands from users
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            // Register commands
            Commands.RegisterCommands<CoreCommands>();
            Commands.RegisterCommands<ModCommands>();
            Commands.RegisterCommands<AdminCommands>();

            // Link help formatter
            Commands.SetHelpFormatter<HelpFormatter>();
        }

        private Task Client_Ready(ReadyEventArgs e) {
            // Log a message when client is ready to process events
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Slorp", "Client is ready to process events.", DateTime.Now);

            // This method isn't asynchronous.
            // This means we need to quit the 
            // method manually so nothing else is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e) {
            // Log the name of the guild that was sent to the client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Slorp", $"Guild available: {e.Guild.Name}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e) {
            // Log the details of the error that occurred in the client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Slorp", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e) {
            // Log name of the command and user
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "Slorp", $"{e.Context.User.Username} Successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e) {
            // Log error details
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Slorp", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // Does the user have the right permissions?
            if (e.Exception is ChecksFailedException ex) {
                // No, they don't have the right permissions. Let them know.
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                var embed = new DiscordEmbedBuilder {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = DiscordColor.DarkRed
                };
                await e.Context.RespondAsync(embed: embed.Build());
            }
        }

    }

    public struct ConfigJson {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }
    }
}
