﻿using Discord;
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

                if (reaction.Emote.Name == "✋" && reaction.UserId != host.Id)
                {
                    try
                    {
                        var guest = reaction.User.Value as SocketUser;

                        var game = GameTable.NewRow();
                        game["channel_id"] = channel.Id;

                        if (new Random().Next(0, 2) == 0)
                        {
                            game["red_id"] = host.Id;
                            game["blue_id"] = guest.Id;
                            game["game"] = new Game(channel as SocketTextChannel, host, guest);
                        }
                        else
                        {
                            game["red_id"] = guest.Id;
                            game["blue_id"] = host.Id;
                            game["game"] = new Game(channel as SocketTextChannel, guest, host);
                        }

                        await message.DeleteAsync();

                        GameTable.Rows.Add(game);
                        await GameStart(game["game"] as Game);
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
            var game = GameTable.Select($"red_id={host.Id} or blue_id={host.Id}");
            if (game.Length > 0)
            {
                throw new Exception();
            }

            try
            {
                var GameRoom = GameRoomTable.NewRow();
                GameRoom["channel_id"] = channelId;
                GameRoom["host_id"] = host.Id;
                GameRoom["host"] = host;
                GameRoomTable.Rows.Add(GameRoom);
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
            }
            catch
            {
                RemoveGame(game.channel.Id);
                return;
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