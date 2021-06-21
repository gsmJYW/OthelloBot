using Discord;
using Discord.Commands;
using OthelloBot.src.embed;
using System.Data;
using System.Threading.Tasks;

namespace OthelloBot.src.command
{
    public class GameCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("게임")]
        public async Task Game(params string[] args)
        {
            try
            {
                
            }
            catch
            {
                await Context.Channel.SendMessageAsync("한 채널에 한 게임만 할 수 있습니다.");
                return;
            }

            var embed = new GameRoomEmbed(Context.User);
            var message = await Context.Channel.SendMessageAsync(embed: embed.Build());

            var emoji = new Emoji("✋");
            await message.AddReactionAsync(emoji);
        }
    }
}
