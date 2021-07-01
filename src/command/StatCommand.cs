using Discord.Commands;
using Discord.WebSocket;
using OthelloBot.src.db;
using OthelloBot.src.embed;
using System;
using System.Threading.Tasks;

namespace OthelloBot.src.command
{
    public class StatCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("전적")]
        public async Task Stat(params string[] args)
        {
            var username = $"{Context.User.Username}#{Context.User.Discriminator}";

            if (args.Length > 0)
            {
                username = args[0];
            }

            var users = DB.GetUsers(username);

            if (users.Count == 0)
            {
                await Context.Channel.SendMessageAsync("사용자를 찾지 못했습니다.");
            }
            else if (users.Count == 1)
            {
                var statEmbed = new StatEmbed(users[0]);
                await Context.Channel.SendMessageAsync(embed: statEmbed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("여러 사용자가 검색 되었습니다.\n사용자명을 더 자세히 입력해주세요.");
            }
        }
    }
}
