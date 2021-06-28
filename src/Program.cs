using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OthelloBot.src.db;
using OthelloBot.src.embed;
using Game = OthelloBot.src.Game;

namespace OthelloBot
{
    internal class Program : GameEventHandler
    {
        internal static DiscordShardedClient _client;

        private CommandService _commands;
        private IServiceProvider _services;

        public static string prefix;
        private static string bot_token, server, port, database, uid, pwd;
        
        private static void Main(string[] args)
        {
            try
            {
                prefix = args[0];
                bot_token = args[1];

                server = args[2];
                port = args[3];
                database = args[4];
                uid = args[5];
                pwd = args[6];

                DB.SetConnStr(server, port, database, uid, pwd);
            }
            catch
            {
                Console.WriteLine(
                    @"프로그램 실행 시 다음 매개변수가 필요합니다:
                    [명령어 접두사] [봇 토큰] [MySQL server] [MySQL port] [MySQL database] [MySQL uid] [MySQL pwd]"
                );
                return;
            }

            GameRoomTable.Columns.Add(new DataColumn()
            {
                ColumnName = "channel",
                DataType = typeof(SocketTextChannel),
            });
            GameRoomTable.Columns.Add(new DataColumn()
            {
                ColumnName = "host_id",
                DataType = typeof(ulong),
                Unique = true,
            });
            GameRoomTable.Columns.Add(new DataColumn()
            {
                ColumnName = "host",
                DataType = typeof(SocketUser),
            });

            GameTable.Columns.Add(new DataColumn()
            {
                ColumnName = "channel_id",
                DataType = typeof(ulong),
                Unique = true,
            });
            GameTable.Columns.Add(new DataColumn()
            {
                ColumnName = "red_id",
                DataType = typeof(ulong),
                Unique = true,
            });
            GameTable.Columns.Add(new DataColumn()
            {
                ColumnName = "blue_id",
                DataType = typeof(ulong),
                Unique = true,
            });
            GameTable.Columns.Add(new DataColumn()
            {
                ColumnName = "game",
                DataType = typeof(Game),
            });

            new Program()
                .RunBotAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordShardedClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += _client_Log;
            _client.JoinedGuild += _client_JoinedGuild;
            _client.ReactionAdded += _client_ReactionAdded;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, bot_token);
            await _client.StartAsync();
            await _client.SetGameAsync($"{prefix}도움말");
            await Task.Delay(-1);
        }

        private async Task _client_JoinedGuild(SocketGuild guild)
        {
            var permission = guild.GetUser(_client.CurrentUser.Id).GuildPermissions;

            try
            {
                if (!permission.Administrator)
                {
                    await guild.DefaultChannel.SendMessageAsync("https://discord.com/api/oauth2/authorize?client_id=855050551669293076&permissions=8&scope=bot");
                    await guild.DefaultChannel.SendMessageAsync(
                        @$"{_client.CurrentUser.Mention}는(은) **관리자 권한**이 필요합니다.
                        권한이 부족해 서버에서 내보냅니다, 다시 초대해주세요."
                    );
                    await guild.LeaveAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new ShardedCommandContext(_client, message);

            if (message.Author.IsBot || message == null || message.Channel is not SocketGuildChannel)
            {
                return;
            }

            if (message.Content.Length == 2)
            {
                var gameRows = GameTable.Select($"channel_id={message.Channel.Id}");
                
                if (gameRows.Length == 0)
                {
                    return;
                }

                var gameRow = gameRows.FirstOrDefault();
                var game = gameRow["game"] as Game;
                
                if (message.Author.Id == game.TurnUser().Id)
                {
                    try
                    {
                        var row = message.Content.ToLower()[0] - 'a';
                        var col = Convert.ToByte(message.Content[1].ToString()) - 1;

                        if (row >= 0 && row < 8 && col >= 0 && col < 8)
                        {
                            if (game.IsAvailable(game.turn, row, col))
                            {
                                game.PlacePiece(game.turn, row, col);
                                
                                if (game.HasAvailablePlace(Game.Opponent(game.turn)))
                                {
                                    game.turn = Game.Opponent(game.turn);
                                }
                                else if (!game.HasAvailablePlace(game.turn))
                                {
                                    game.turn = Game.Piece.Empty;
                                }

                                var embed = new GameEmbed(game);

                                if (game.turn == Game.Piece.Empty)
                                {
                                    var redCount = game.CountPiece(Game.Piece.Red);
                                    var blueCount = game.CountPiece(Game.Piece.Blue);

                                    if (redCount == blueCount)
                                    {
                                        embed.Title = "무승부";
                                    }
                                    else
                                    {
                                        var winner = redCount > blueCount ? game.red : game.blue;
                                        var countDifference = Math.Abs(redCount - blueCount);
                                        embed.Title = $"{winner.Username}님이\n{countDifference}점 차이로 승리";
                                    }

                                    embed.Footer.Text = $"🔴 {game.red.Username} vs {game.blue.Username} 🔵";
                                    
                                    var gameRoomRow = GameRoomTable.Select($"host_id={game.hostId}").FirstOrDefault();
                                    var gameRoomChannel = gameRoomRow["channel"] as SocketTextChannel;

                                    await RemoveGame(game.hostId);
                                    await gameRoomChannel.SendMessageAsync(embed: embed.Build());
                                }
                                else
                                {
                                    await (await game.GetMessage()).ModifyAsync(msg =>
                                    {
                                        msg.Embed = embed.Build();
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                return;
            }

            var argPos = 0;

            if (message.HasStringPrefix(prefix, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    var err = result.ErrorReason;
                    Console.WriteLine(err);
                }
            }

            try
            {
                var gameRows = GameTable.Select($"channel_id={message.Channel.Id}");
                if (gameRows.Length > 0)
                {
                    await message.DeleteAsync();
                }
            }
            catch
            {

            }
        }
    }
}
