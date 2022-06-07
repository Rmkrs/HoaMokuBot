// ReSharper disable UnusedMember.Global
namespace HoaMokuBot.Modules
{
    using System.Globalization;
    using Config.Contracts;
    using Discord;
    using Discord.Commands;

    [Name("Help")]
    // ReSharper disable once UnusedType.Global
    public class Help : ModuleBase
    {
        private readonly CommandService commandService;
        private readonly IConfig config;

        public Help(CommandService commandService, IConfig config)
        {
            this.commandService = commandService;
            this.config = config;
        }

        [Command("help", RunMode = RunMode.Async)]
        [Summary("Lists all the commands")]
        public async Task HelpAsync([Remainder]string commandOrModule = "")
        {
            if (!String.IsNullOrWhiteSpace(commandOrModule))
            {
                await DetailedHelpAsync(commandOrModule.ToLower());
                return;
            }

            var builder = new EmbedBuilder();
            builder.WithColor(new Color(87, 222, 127));
            builder.WithTitle($"Hey {Context.User.Username}, here is a list of all my commands");
            builder.WithFooter(f => f.WithText("Use 'help [command-name]' or 'help [module-name]' for more information"));

            foreach (var module in commandService.Modules.OrderBy(x => x.Name))
            {
                var moduleCommands = new List<string>();

                foreach (var command in module.Commands.OrderBy(x => x.Name))
                {
                    if (module.Name.ToLower().Equals("help"))
                    {
                        continue;
                    }

                    var result = await command.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        moduleCommands.Add($"{command.Aliases.First()} - {command.Summary}");
                    }
                }

                if (moduleCommands.Any())
                {
                    builder.AddField(x =>
                    {
                        x.Name = $"{Environment.NewLine}{module.Name}";
                        x.Value = String.Join(Environment.NewLine, moduleCommands);
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync(String.Empty, false, builder.Build());
        }

        private async Task DetailedHelpAsync(string commandOrModule)
        {
            var moduleFound = this.commandService.Modules.Select(m => m.Name.ToString().ToLower()).ToList().Contains(commandOrModule);
            if (moduleFound)
            {
                await DetailedModuleHelpAsync(commandOrModule);
                return;
            }

            var result = this.commandService.Search(Context, commandOrModule);
            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I could not find a command called *{commandOrModule}*");
                return;
            }

            var builder = new EmbedBuilder
            {
                Color = new Color(87, 22, 127)
            };

            var parametersFound = false;

            foreach (var command in result.Commands.Select(s => s.Command))
            {
                builder.AddField(x =>
                {
                    x.Name = $"Producing help on {command.Name}";
                    var temp = "None";
                    if (command.Aliases.Count != 1)
                    {
                        var formattedAliases = command.Aliases.Select(a => $"{this.config.GetBotPrefix(Context.Guild.Id)}{a}");
                        temp = string.Join(", ", formattedAliases);
                    }

                    x.Value = "**Aliases**: " + temp;
                    temp = "`" + this.config.GetBotPrefix(Context.Guild.Id) + commandOrModule;

                    if (command.Parameters.Count != 0)
                    {
                        parametersFound = true;
                        temp += " " + string.Join(
                            " ",
                            command.Parameters.Select(
                                p => p.IsOptional
                                    ? "<" + (p.Summary?.Length > 1 ? p.Summary : p.Name) + ">"
                                    : "[" + (p.Summary?.Length > 1 ? p.Summary : p.Name) + "]"));
                    }

                    temp += "`";
                    x.Value += $"\n**Usage**: {temp}\n**Summary**: {command.Summary}";

                    x.IsInline = false;
                });
            }

            if (parametersFound)
            {
                builder.WithFooter($"Note: {Environment.NewLine}Parameters inside '[]' are mandatory.{Environment.NewLine}Parameters inside '<>' are optional.");
            }

            await ReplyAsync(String.Empty, false, builder.Build());
        }

        private async Task DetailedModuleHelpAsync(string module)
        {
            var first = this.commandService.Modules.First(m => m.Name.ToLower() == module);
            var embed = new EmbedBuilder()
            {
                Title = $"List of commands under the {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(module)} module",
                Description = String.Empty,
                Color = new Color(87, 22, 127)
            };

            embed.WithFooter("Use 'help [command-name]' for more information on the command");
            embed.Description += String.Join(",", first.Commands.Select(s => $"{s.Name} - {s.Summary}"));
            await ReplyAsync(String.Empty, false, embed.Build());
        }
    }
}
