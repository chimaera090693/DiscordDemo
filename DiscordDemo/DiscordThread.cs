using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDemo
{
    public class DiscordThread
    {
        public DiscordClient Client { get; set; }

        public void InitialClient(DiscordConfig cfg, EventHandler<DebugLogMessageEventArgs> logHandler)
        {
            this.Client = new DiscordClient(cfg);
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientError += this.Client_ClientError;
            Client.DebugLogger.LogMessageReceived += logHandler;
        }

        public async Task StartMain()
        {
            //custom logger
            //Client.DebugLogger.LogMessageReceived??

            // register event
            if (Client == null) throw new NullReferenceException("Cliennt is null, please initial client first!");
            await this.Client.ConnectAsync();

            await Task.CompletedTask;
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
    }
}
