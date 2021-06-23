using Discord;
using Discord.WebSocket;
using System.Data;

namespace OthelloBot.src.embed
{
    internal class GameEmbed : EmbedBuilder
    {
        public GameEmbed(DataRow game)
        {
            var black = game["black"] as SocketUser;
            var white = game["white"] as SocketUser;

            WithColor(new Color(0xFFFFFF));
            WithTitle($"○ 02 __{black.Username}__ vs {white.Username} 02 ●");
            WithDescription(
              @"`┏━━━━━┳━━━━━┳━━━━━┳━━━━━┳━━━━━┳━━━━━┳━━━━━┳━━━━━┓`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┣━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━╋━━━━━┫`
                `┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃     ┃`
                `┗━━━━━┻━━━━━┻━━━━━┻━━━━━┻━━━━━┻━━━━━┻━━━━━┻━━━━━┛`");
        }
    }
}
