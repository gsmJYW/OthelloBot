using Discord;
using Discord.Commands;
using Discord.Rest;
using OthelloBot.src.embed;
using System.Threading.Tasks;
using System.Timers;

namespace OthelloBot.src.command
{
    public class GameCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("게임")]
        public async Task Game(params string[] args)
        {
            try
            {
                GameEventHandler.CreateGameRoom(Context.Channel.Id, Context.User);
            }
            catch
            {
                await Context.Channel.SendMessageAsync("한 채널에 한 게임만 할 수 있습니다.");
                return;
            }

            var embed = new GameRoomEmbed(Context.User);
            var message = await Context.Channel.SendMessageAsync(embed: embed.Build());

            await message.AddReactionAsync(new Emoji("✋"));
            await message.AddReactionAsync(new Emoji("❎"));

            var timer = new GameRoomTimer()
            {
                Enabled = true,
                AutoReset = true,
                Interval = 1000 * 30,
                Message = message,
            };
            
            timer.Elapsed += OnTimedEvent;
            timer.Start();
        }

        private class GameRoomTimer : Timer
        {
            public RestUserMessage Message;
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var GameRoom = (GameRoomTimer)sender;

            try
            {
                await GameRoom.Message.DeleteAsync();
                GameRoom.Stop();

                GameEventHandler.RemoveGame(GameRoom.Message.Channel.Id);
            }
            catch
            {

            }
            return;
        }
    }
}
