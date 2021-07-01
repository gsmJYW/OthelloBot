using Discord.Commands;
using OthelloBot.src.db;
using OthelloBot.src.embed;
using System;
using System.Data;
using System.Threading.Tasks;

namespace OthelloBot.src.command
{
    public class LeaderboardCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("리더보드")]
        public async Task Leaderboard(params string[] args)
        {
            try
            {
                var leaderboardEmbed = new LeaderboardEmbed();
                await Context.Channel.SendMessageAsync(embed: leaderboardEmbed.Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync("리더보드를 불러오지 못했습니다.");
            }
        }
    }
}
