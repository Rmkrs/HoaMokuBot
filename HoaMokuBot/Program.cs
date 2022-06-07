using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HoaMokuBot.AppSettings;
using HoaMokuBot.Config;
using HoaMokuBot.Config.Contracts;
using HoaMokuBot.Handlers;
using HoaMokuBot.Handlers.Contracts;
using HoaMokuBot.Parsers;
using HoaMokuBot.Parsers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Victoria;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        services.Configure<Settings>(config.GetSection("Settings"));
        services.AddSingleton(new DiscordShardedClient());
        services.AddSingleton(new CommandService(new CommandServiceConfig { LogLevel = LogSeverity.Info, CaseSensitiveCommands = false }));
        services.AddSingleton<ICommandHandler, CommandHandler>();
        services.AddSingleton<IMusicActionHandler, MusicActionHandler>();
        services.AddSingleton<IMusicButtonHandler, MusicButtonHandler>();
        services.AddSingleton<IMusicChannelHandler, MusicChannelHandler>();
        services.AddSingleton<IMusicEmbedHandler, MusicEmbedHandler>();
        services.AddSingleton<IMusicLavaPlayerHandler, MusicLavaPlayerHandler>();
        services.AddSingleton<IMusicMessageComponentHandler, MusicMessageComponentHandler>();
        services.AddSingleton<IMusicPlaylistHandler, MusicPlaylistHandler>();
        services.AddSingleton<IMusicSearchHandler, MusicSearchHandler>();
        services.AddSingleton<IMusicStatusHandler, MusicStatusHandler>();
        services.AddSingleton<IMusicTrackHandler, MusicTrackHandler>();

        services.AddSingleton<IConfig, Config>();
        services.AddSingleton<IPlaylist, Playlist>();
        services.AddSingleton<ISpotifyParser, SpotifyParser>();
        services.AddSingleton<IYoutubeParser, YoutubeParser>();

        var settings = new Settings();
        config.GetSection("Settings").Bind(settings);
        services.AddLavaNode(x =>
        {
            x.SelfDeaf = settings.LavaNode.SelfDeaf;
            x.Hostname = settings.LavaNode.HostName;
            x.Port = settings.LavaNode.Port;
            x.Authorization = settings.LavaNode.Authorization;
            x.IsSsl = settings.LavaNode.IsSsl;
        });

    }).Build();

var commandHandler = host.Services.GetRequiredService<ICommandHandler>();
await commandHandler.InitializeAsync();
Console.WriteLine("Started.");
Console.ReadKey();