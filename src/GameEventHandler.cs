using Discord;
using Discord.WebSocket;
using OthelloBot.src.embed;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OthelloBot
{
    internal class GameEventHandler
    {
        protected static DataTable
            GameRoomTable = new DataTable(),
            GameTable = new DataTable();

        protected async Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> entity, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = channel.GetMessageAsync(reaction.MessageId).Result;

            if (reaction.User.Value.IsBot || message.Embeds.Count == 0)
            {
                return;
            }

            var embeds = message.Embeds.GetEnumerator();
            embeds.MoveNext();

            if (embeds.Current.Title.Contains("게임"))
            {
                var GameRoom = GameRoomTable.Select($"channel_id={channel.Id}").FirstOrDefault();
                var host = GameRoom["host"] as SocketUser;

                if (reaction.Emote.Name == "✋")
                {
                    try
                    {
                        var guest = reaction.User.Value as SocketUser;

                        var game = GameTable.NewRow();
                        game["channel_id"] = channel.Id;

                        if (new Random().Next(0, 2) == 0)
                        {
                            game["black"] = host;
                            game["white"] = guest;
                        }
                        else
                        {
                            game["black"] = guest;
                            game["white"] = host;
                        }

                        await message.DeleteAsync();

                        GameTable.Rows.Add(game);
                        await GameStart(game);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (reaction.Emote.Name == "❎" && reaction.UserId == host.Id)
                {
                    await message.DeleteAsync();
                    RemoveGame(channel.Id);
                }
            }
            else
            {
                return;
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

        public static void CreateGameRoom(ulong channelId, SocketUser host)
        {
            try
            {
                var GameRoom = GameRoomTable.NewRow();
                GameRoom["channel_id"] = channelId;
                GameRoom["host"] = host;
                GameRoomTable.Rows.Add(GameRoom);
            }
            catch
            {
                throw;
            }
        }

        protected async static Task GameStart(DataRow game)
        {
            try
            {
                var channel = Program._client.GetChannel(Convert.ToUInt64(game["channel_id"])) as SocketTextChannel;
                var embed = new GameEmbed(game);

                var message = await channel.SendMessageAsync(embed: embed.Build());

                await message.AddReactionAsync(new Emoji("🔼"));
                await message.AddReactionAsync(new Emoji("🔽"));
                await message.AddReactionAsync(new Emoji("◀️"));
                await message.AddReactionAsync(new Emoji("▶️"));
                await message.AddReactionAsync(new Emoji("✅"));
            }
            catch (Exception e)
            {
                RemoveGame(Convert.ToUInt64(game["channel_id"]));
                Console.WriteLine(e.Message);
            }
        }

        public static void RemoveGame(ulong channel_id)
        {
            try
            {
                var GameRoom = GameRoomTable.Select($"channel_id={channel_id}").FirstOrDefault();
                var Game = GameTable.Select($"channel_id={channel_id}").FirstOrDefault();

                GameRoomTable.Rows.Remove(GameRoom);
                GameTable.Rows.Remove(Game);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}