// ReSharper disable UnusedMember.Global
namespace HoaMokuBot.Modules
{
    using Config.Contracts;
    using Discord;
    using Discord.Commands;

    [Name("Admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    // ReSharper disable once UnusedType.Global
    public class Admin : ModuleBase<SocketCommandContext>
    {
        private readonly IConfig config;

        public Admin(IConfig config)
        {
            this.config = config;
        }

        [Command("prefix")]
        [Summary(("Change or view my prefix"))]
        public async Task PrefixTask(string newPrefix = "")
        {
            if (String.IsNullOrWhiteSpace(newPrefix))
            {
                await ReplyAsync($"The prefix I'm actively listening to is: {this.config.GetBotPrefix(Context.Guild.Id)}");
                return;
            }

            this.config.SetBotPrefix(Context.Guild.Id, newPrefix);
            await ReplyAsync($"My active listening prefix changed to : {newPrefix}");
        }
    }
}
