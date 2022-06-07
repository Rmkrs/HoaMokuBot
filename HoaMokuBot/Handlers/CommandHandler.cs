namespace HoaMokuBot.Handlers
{
    using System.Reflection;
    using AppSettings;
    using Config.Contracts;
    using Contracts;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Options;
    using Victoria;
    using Victoria.Enums;
    using Victoria.EventArgs;

    public class CommandHandler : ICommandHandler
    {
        private readonly DiscordShardedClient client;
        private readonly CommandService commandService;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfig config;
        private readonly Settings settings;
        private readonly IMusicButtonHandler buttonHandler;
        private readonly IMusicActionHandler actionHandler;
        private readonly IMusicTrackHandler trackHandler;
        private readonly IMusicChannelHandler channelHandler;
        private readonly LavaNode lavaNode;

        public CommandHandler(
            DiscordShardedClient client, 
            CommandService commandService, 
            IServiceProvider serviceProvider, 
            IConfig config, 
            IOptions<Settings> settings, 
            IMusicButtonHandler buttonHandler,
            IMusicActionHandler actionHandler,
            IMusicTrackHandler trackHandler,
            IMusicChannelHandler channelHandler,
            LavaNode lavaNode)
        {
            this.client = client;
            this.commandService = commandService;
            this.serviceProvider = serviceProvider;
            this.config = config;
            this.settings = settings.Value;
            this.buttonHandler = buttonHandler;
            this.actionHandler = actionHandler;
            this.trackHandler = trackHandler;
            this.channelHandler = channelHandler;
            this.lavaNode = lavaNode;
        }

        public async Task InitializeAsync()
        {
            await this.commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), serviceProvider);
            this.client.MessageReceived += this.HandleCommandAsync;

            this.commandService.CommandExecuted += async (_, context, result) =>
            {
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync($"error: {result}");
                }
            };

            client.ShardReady += this.OnShardReady;
            await this.client.LoginAsync(TokenType.Bot, this.settings.Discord.BotToken);
            await this.client.StartAsync();
            await this.client.SetGameAsync(this.settings.Discord.ActivityName, null, this.settings.Discord.ActivityType);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage msg)
            {
                return;
            }

            if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot)
            {
                return;
            }

            var context = new ShardedCommandContext(client, msg);

            var markPos = 0;

            if (!msg.HasStringPrefix(this.config.GetBotPrefix(context.Guild.Id), ref markPos) && 
                !msg.HasMentionPrefix(this.client.CurrentUser, ref markPos))
            {
                return;
            }

            using (context.Channel.EnterTypingState())
            {
                try
                {
                    var result = await this.commandService.ExecuteAsync(context, markPos, this.serviceProvider);
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine($"{result.Error} - {result.ErrorReason} at {context.Guild.Name}");

                        switch (result.Error)
                        {
                            case CommandError.UnknownCommand:
                                await msg.AddReactionAsync(this.settings.Discord.ButtonEmojis.Parsed.UnknownCommand);
                                break;
                            case CommandError.BadArgCount:
                                await context.Channel.SendMessageAsync("You are supposed to pass in a parameter with this command. Type 'help [command name]' for help");
                                break;
                            case CommandError.UnmetPrecondition:
                                // await context.Channel.SendMessageAsync($"You can not use this command at the moment. {Environment.NewLine} Reason: {result.ErrorReason}");
                                break;
                            default:
                                await context.Channel.SendMessageAsync($"{result.Error} - {result.ErrorReason}");
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ResetColor();
                    throw;
                }
            }
        }

        private async Task OnShardReady(DiscordSocketClient arg)
        {
            if (this.lavaNode.IsConnected)
            {
                Console.WriteLine("OnShardReady while LavaNode already Connected...");
                return;
            }

            await this.lavaNode.ConnectAsync();
            if (!this.lavaNode.IsConnected)
            {
                Console.WriteLine("LavaNode connection failed!");
                return;
            }

            this.lavaNode.OnTrackEnded += this.OnTrackEnded;
            this.client.ButtonExecuted += this.OnButtonExecuted;
            this.client.UserVoiceStateUpdated += this.OnUserVoiceStateUpdated;

            Console.WriteLine("LavaNode Connected.");
        }

        private async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState previous, SocketVoiceState current)
        {
            if (user.IsBot || user.Id == client.CurrentUser.Id)
            {
                return;
            }

            if (previous.VoiceChannel != null)
            {
                await this.channelHandler.CheckAutoJoin(previous.VoiceChannel.Guild, previous);
            }

            if (current.VoiceChannel != null)
            {
                await this.channelHandler.CheckAutoJoin(current.VoiceChannel.Guild, current);
            }
        }

        private async Task OnButtonExecuted(SocketMessageComponent component)
        {
            await component.DeferAsync();

            var channel = component.Message.Channel;
            var guild = ((SocketGuildChannel)channel).Guild;
            await this.buttonHandler.OnButtonExecuted(guild, channel, component.Data.CustomId);
        }

        private async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            if (args.Reason is TrackEndReason.Stopped or TrackEndReason.Replaced)
            {
                return;
            }

            await this.actionHandler.VerifyAndExecuteAndUpdateStatus((SocketGuild)args.Player.VoiceChannel.Guild, args.Player.TextChannel, () => this.trackHandler.NextTrack(args.Player));
        }
    }
}
