using Discord;
using Discord.WebSocket;
using OthelloBot.src.db;
using System;

namespace OthelloBot.src.embed
{
    class StatEmbed : EmbedBuilder
    {
        public StatEmbed(SocketUser user)
        {
            WithColor(new Color(0xFFFFFF));
            WithThumbnailUrl(user.GetAvatarUrl());

            var win = 0;
            var draw = 0;
            var lose = 0;
            var playtime_second = 0;
            var winRate = 0.0;

            try
            {
                var userRow = DB.GetUser(user.Id);

                win = Convert.ToInt32(userRow["win"]);
                draw = Convert.ToInt32(userRow["draw"]);
                lose = Convert.ToInt32(userRow["lose"]);
                playtime_second = Convert.ToInt32(userRow["playtime_second"]);

                winRate = (double)win / (win + draw + lose);
            }
            catch
            {

            }

            AddField("승", win, true);
            AddField("무", draw, true);
            AddField("패", lose, true);

            AddField("승률", $"{winRate * 100: 0.00}%", true);
            AddField("플레이 시간", $"{playtime_second / 60}분", true);
        }
    }
}
