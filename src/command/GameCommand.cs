using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
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
                GameEventHandler.CreateGameRoom(Context.Channel as SocketTextChannel, Context.User);
            }
            catch
            {
                await Context.Channel.SendMessageAsync("이미 다른 게임에 참여 중이십니다.");
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
                Interval = 1000 * 60,
                Message = message,
                hostId = Context.User.Id,
            };
            
            timer.Elapsed += OnTimedEvent;
            timer.Start();
        }

        private class GameRoomTimer : Timer
        {
            public RestUserMessage Message;
            public ulong hostId;
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var GameRoom = (GameRoomTimer)sender;

            try
            {
                await GameRoom.Message.DeleteAsync();
                GameRoom.Stop();

                await GameEventHandler.RemoveGame(GameRoom.hostId);
            }
            catch
            {

            }
            return;
        }
    }
}
