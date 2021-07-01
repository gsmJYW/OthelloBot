using Discord;
using Discord.Rest;
using Discord.WebSocket;
using OthelloBot.src.embed;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Game = OthelloBot.src.Game;

namespace OthelloBot
{
    internal class GameEventHandler
    {
        protected static DataTable GameRoomTable = new DataTable(), GameTable = new DataTable();

        protected async Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> entity, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = channel.GetMessageAsync(reaction.MessageId).Result;

            if (reaction.User.Value.IsBot || message.Embeds.Count == 0)
            {
                return;
            }

            var embeds = message.Embeds.GetEnumerator();
            embeds.MoveNext();

            if (embeds.Current.Title != null && embeds.Current.Title.Contains("게임"))
            {
                var hostId = embeds.Current.Footer.Value.Text[3..];

                var gameRoom = GameRoomTable.Select($"host_id={hostId}").FirstOrDefault();
                var host = gameRoom["host"] as SocketUser;

                if (reaction.Emote.Name == "✋" && reaction.UserId != host.Id)
                {
                    try
                    {
                        var guest = reaction.User.Value as SocketUser;
                        var gameRow = GameTable.NewRow();

                        if (new Random().Next(0, 2) == 0)
                        {
                            gameRow["red_id"] = host.Id;
                            gameRow["blue_id"] = guest.Id;
                            gameRow["game"] = new Game(host, guest);
                        }
                        else
                        {
                            gameRow["red_id"] = guest.Id;
                            gameRow["blue_id"] = host.Id;
                            gameRow["game"] = new Game(guest, host);
                        }
                        GameTable.Rows.Add(gameRow);

                        var game = gameRow["game"] as Game;
                        var gameName = $"{host.Username} vs {guest.Username}";

                        game.channel = await (channel as SocketTextChannel).Guild.CreateTextChannelAsync(gameName, properties =>
                        {
                            properties.SlowModeInterval = 3;
                        });

                        await message.DeleteAsync();
                        await channel.SendMessageAsync($"{game.channel.Mention} 게임이 시작됩니다.");

                        var guild = Program._client.GetGuild(game.channel.GuildId);
                        game.role = await guild.CreateRoleAsync(gameName, isMentionable: false);

                        await (game.red as IGuildUser).AddRoleAsync(game.role.Id);
                        await (game.blue as IGuildUser).AddRoleAsync(game.role.Id);

                        await game.channel.AddPermissionOverwriteAsync(game.role, permissions: new OverwritePermissions(sendMessages: PermValue.Allow));
                        await game.channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permissions: new OverwritePermissions(sendMessages: PermValue.Deny));

                        gameRow["channel_id"] = game.channel.Id;

                        game.hostId = host.Id;
                        game.roomChannel = channel as SocketTextChannel;

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
                    await RemoveGame(host.Id);
                }
            }
            else if (embeds.Current.Footer.HasValue && embeds.Current.Footer.Value.Text.Contains("기권"))
            {
                if (reaction.Emote.Name == "🙌")
                {
                    var gameRows = GameTable.Select($"channel_id={channel.Id}");

                    if (gameRows.Length > 0)
                    {
                        var gameRow = gameRows.FirstOrDefault();
                        var red_id = Convert.ToUInt64(gameRow["red_id"]);
                        var blue_id = Convert.ToUInt64(gameRow["blue_id"]);

                        if (reaction.UserId == red_id || reaction.UserId == blue_id)
                        {
                            var game = gameRow["game"] as Game;
                            game.turn = Game.Piece.Empty;

                            var embed = new GameEmbed(game);
                            
                            SocketUser winner;
                            int redWin = 0, blueWin = 0;

                            if (reaction.UserId == blue_id)
                            {
                                winner = game.red;
                                redWin = 1;
                            }
                            else
                            {
                                winner = game.blue;
                                blueWin = 1;
                            }

                            embed.Title = $"{winner.Username}님이 불계승";
                            embed.Footer.Text = "";

                            game.UpdateStat(redWin, blueWin, (int)(DateTime.Now - game.startTime).TotalSeconds);
                            await RemoveGame(game.hostId);

                            try
                            {
                                await game.roomChannel.SendMessageAsync(embed: embed.Build());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
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

        public static void CreateGameRoom(SocketTextChannel channel, SocketUser host)
        {
            var game = GameTable.Select($"red_id={host.Id} or blue_id={host.Id}");
            if (game.Length > 0)
            {
                throw new Exception();
            }

            try
            {
                var gameRoom = GameRoomTable.NewRow();
                gameRoom["channel"] = channel;
                gameRoom["host_id"] = host.Id;
                gameRoom["host"] = host;
                GameRoomTable.Rows.Add(gameRoom);
            }
            catch
            {
                throw;
            }
        }

        protected async static Task GameStart(Game game)
        {
            try
            {
                var embed = new GameEmbed(game);
                var message = await game.channel.SendMessageAsync(embed: embed.Build());
                await message.AddReactionAsync(new Emoji("🙌"));
                
                game.SetMessage(message);
                game.timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                await RemoveGame(game.hostId);
                return;
            }
        }

        public async static Task RemoveGame(ulong host_id)
        {
            try
            {
                var gameRoomRow = GameRoomTable.Select($"host_id={host_id}").FirstOrDefault();
                GameRoomTable.Rows.Remove(gameRoomRow);

                var gameRow = GameTable.Select($"red_id={host_id} or blue_id={host_id}").FirstOrDefault();
                
                var game = gameRow["game"] as Game;
                game.timer.Dispose();
                
                GameTable.Rows.Remove(gameRow);

                await game.role.DeleteAsync();
                await game.channel.DeleteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}