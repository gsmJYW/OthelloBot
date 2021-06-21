using Discord;
using Discord.WebSocket;
using System;
using System.Data;
using System.Threading.Tasks;

namespace OthelloBot
{
    internal class GameEventHandler
    {
        protected static DataTable GameRoomTable = new DataTable();

        protected async Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> entity, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var message = channel.GetMessageAsync(reaction.MessageId).Result;

            try
            {
                var embeds = message.Embeds.GetEnumerator();
                embeds.MoveNext();

                var footerText = embeds.Current.Footer.Value.Text;
                var hostId = Convert.ToUInt64(footerText[3..]);

                var GameRoom = GameRoomTable.NewRow();
                GameRoom["channel_id"] = channel.Id;
                GameRoom["host_id"] = hostId;
                GameRoom["guest_id"] = reaction.UserId;
                GameRoomTable.Rows.Add(GameRoom);

                await channel.DeleteMessageAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected async static void GameStart(object sender, DataRowChangeEventArgs e)
        {

        }
    }
}