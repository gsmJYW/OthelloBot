using Discord.Commands;
using OthelloBot.src.embed;
using System.Threading.Tasks;

namespace OthelloBot.src.command
{
    public class HelpCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("도움말")]
        public async Task Help(params string[] args)
        {
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var helpEmbed = new HelpEmbed(avatarUrl);

            await Context.Channel.SendMessageAsync(embed: helpEmbed.Build());
        }
    }
}
