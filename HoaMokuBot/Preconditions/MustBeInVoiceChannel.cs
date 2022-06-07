namespace HoaMokuBot.Preconditions
{
    using Discord;
    using Discord.Commands;

    public class MustBeInVoiceChannel : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult((context.User as IGuildUser)?.VoiceChannel is null 
                ? PreconditionResult.FromError("You must be in a voice channel to use music") 
                : PreconditionResult.FromSuccess());
        }
    }
}