using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DiscordDemo.Bus;

namespace DiscordDemo
{
    public class DiscordThread
    {
        public static Thread botThread;
        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }

        public void InitialClient(DiscordConfig cfg, EventHandler<DebugLogMessageEventArgs> logHandler)
        {
            this.Client = new DiscordClient(cfg);
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientError += this.Client_ClientError;
            Client.DebugLogger.LogMessageReceived += logHandler;

            //init command
            var ccfg = new CommandsNextConfiguration
            {
                StringPrefix = "#",
                EnableDms = true,
                EnableMentionPrefix = true
            };
            this.Commands = this.Client.UseCommandsNext(ccfg);
            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;
            // up next, let's register our commands
            this.Commands.RegisterCommands<CommonCommands>();
            this.Commands.RegisterCommands<ExampleExecutableGroup>();


            botThread = new Thread(new ThreadStart(this.StartMain));
            botThread.Start();
        }


        public void StartMain()
        {
            //custom logger
            //Client.DebugLogger.LogMessageReceived??

            // register event
            if (Client == null) throw new NullReferenceException("Client is null, please initial client first!");
            Client.ConnectAsync();
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", "Client is ready.", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "ExampleBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }


        private Task Commands_CommandExecuted(CommandExecutedEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
            // is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "ExampleBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);
            if (e.Exception is ChecksFailedException ex)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                var embed = new DiscordEmbed
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = 0xFF0000 // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }

    }
}
