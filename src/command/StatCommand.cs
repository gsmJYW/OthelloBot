using Discord.Commands;
using Discord.WebSocket;
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
            var user = Context.User;

            if (args.Length > 0)
            {
                try
                {
                    var userId = Convert.ToUInt64(args[0][3..21]);
                    user = Context.Guild.GetUser(userId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (user == null)
                {
                    await Context.Channel.SendMessageAsync("유저를 찾지 못했습니다.");
                }
            }

            var statEmbed = new StatEmbed(user);
            await Context.Channel.SendMessageAsync(embed: statEmbed.Build());
        }
    }
}
