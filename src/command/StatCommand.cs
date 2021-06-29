using Discord.Commands;
using OthelloBot.src.embed;
using System.Threading.Tasks;

namespace OthelloBot.src.command
{
    public class StatCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("전적")]
        public async Task Stat(params string[] args)
        {
            var statEmbed = new StatEmbed(Context.User);
            await Context.Channel.SendMessageAsync(embed: statEmbed.Build());
        }
    }
}
